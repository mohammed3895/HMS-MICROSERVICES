using Fido2NetLib;
using Fido2NetLib.Objects;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HMS.Authentication.Infrastructure.Services
{
    public class WebAuthnService : IWebAuthnService
    {
        private readonly IFido2 _fido2;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WebAuthnService> _logger;
        private readonly IAuditService _auditService;
        private readonly ICacheService _cacheService;
        private readonly AuthenticationDbContext _context;

        public WebAuthnService(
            IFido2 fido2,
            IConfiguration configuration,
            ILogger<WebAuthnService> logger,
            IAuditService auditService,
            ICacheService cacheService,
            AuthenticationDbContext context)
        {
            _fido2 = fido2;
            _configuration = configuration;
            _logger = logger;
            _auditService = auditService;
            _cacheService = cacheService;
            _context = context;
        }

        public async Task<CredentialCreateOptions> InitiateRegistrationAsync(
            ApplicationUser user,
            List<UserCredential> existingCredentials)
        {
            try
            {
                var fidoUser = new Fido2User
                {
                    DisplayName = $"{user.FirstName} {user.LastName}",
                    Name = user.Email!,
                    Id = user.Id.ToByteArray()
                };

                // Existing credentials to prevent re-registration
                var existingKeys = existingCredentials
                    .Where(c => !c.IsRevoked)
                    .Select(c => new PublicKeyCredentialDescriptor(c.CredentialId))
                    .ToList();

                var authenticatorSelection = new AuthenticatorSelection
                {
                    RequireResidentKey = true,
                    UserVerification = UserVerificationRequirement.Required
                };

                var extensions = new AuthenticationExtensionsClientInputs();

                // Use dynamic invocation to avoid compile-time overload issues across Fido2 versions
                var options = ((dynamic)_fido2).RequestNewCredential(
                    fidoUser,
                    existingKeys,
                    authenticatorSelection,
                    AttestationConveyancePreference.Direct,
                    extensions);

                // Store challenge for verification
                await _cacheService.SetAsync(
                    $"webauthn:reg:challenge:{user.Id}",
                    options.Challenge,
                    TimeSpan.FromMinutes(5));

                await _auditService.LogAsync(
                    "WebAuthnRegistrationInitiated",
                    "Authentication",
                    user.Id.ToString(),
                    "WebAuthn registration initiated",
                    null);

                _logger.LogInformation(
                    "WebAuthn registration initiated for user {UserId}", user.Id);

                return options;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error initiating WebAuthn registration for user {UserId}", user.Id);
                throw;
            }
        }

        public async Task<UserCredential> CompleteRegistrationAsync(
            ApplicationUser user,
            AuthenticatorAttestationRawResponse attestationResponse,
            string? deviceName = null)
        {
            try
            {
                // Retrieve stored challenge
                var challenge = await _cacheService.GetAsync<byte[]>(
                    $"webauthn:reg:challenge:{user.Id}");

                if (challenge == null)
                {
                    throw new InvalidOperationException(
                        "No challenge found. Registration may have expired.");
                }

                var options = new CredentialCreateOptions
                {
                    Challenge = challenge,
                    Rp = new PublicKeyCredentialRpEntity(
                        _configuration["WebAuthn:RelyingPartyId"]!,
                        _configuration["WebAuthn:RelyingPartyName"]!,
                        null),
                    User = new Fido2User
                    {
                        DisplayName = $"{user.FirstName} {user.LastName}",
                        Name = user.Email!,
                        Id = user.Id.ToByteArray()
                    },
                    PubKeyCredParams = new List<PubKeyCredParam>
                    {
                        new PubKeyCredParam(COSE.Algorithm.ES256),
                        new PubKeyCredParam(COSE.Algorithm.RS256)
                    },
                    Timeout = 60000,
                    Attestation = AttestationConveyancePreference.Direct,
                    AuthenticatorSelection = new AuthenticatorSelection
                    {
                        RequireResidentKey = true,
                        UserVerification = UserVerificationRequirement.Required
                    }
                };

                // Callback to check if credential already exists
                IsCredentialIdUniqueToUserAsyncDelegate callback = async (args, cancellationToken) =>
                {
                    var exists = await _context.UserCredentials
                        .AnyAsync(c => c.CredentialId == args.CredentialId, cancellationToken);
                    return !exists;
                };

                // Verify attestation (dynamic invocation to match package overloads)
                var success = await ((dynamic)_fido2).MakeNewCredentialAsync(
                    attestationResponse,
                    options,
                    callback,
                    CancellationToken.None);

                if (success.Result == null)
                {
                    throw new InvalidOperationException(
                        "Failed to create credential: " + success.ErrorMessage);
                }

                // Parse flags from authenticator data (byte 32)
                var authData = success.Result.AuthData;
                var flags = authData != null && authData.Length > 32 ? authData[32] : (byte)0;
                var isBackupEligible = (flags & 0x08) != 0; // BE flag (bit 3)
                var isBackedUp = (flags & 0x10) != 0; // BS flag (bit 4)

                // Determine device name if not provided
                var finalDeviceName = deviceName;
                if (string.IsNullOrEmpty(finalDeviceName))
                {
                    finalDeviceName = DetermineDeviceName(
                        success.Result.Aaguid,
                        success.Result.CredType);
                }

                // Create credential record with all fields
                var credential = new UserCredential
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    CredentialId = success.Result.CredentialId,
                    PublicKey = success.Result.PublicKey,
                    UserHandle = success.Result.User.Id,
                    SignatureCounter = success.Result.Counter,
                    CredType = success.Result.CredType,
                    AaGuid = success.Result.Aaguid,
                    DeviceName = finalDeviceName,
                    IsBackupEligible = isBackupEligible,
                    IsBackedUp = isBackedUp,
                    AttestationFormat = success.Result.AttestationFormat,
                    CreatedAt = DateTime.UtcNow,
                    LastUsedAt = DateTime.UtcNow,
                    IsRevoked = false
                };

                // Save to database
                _context.UserCredentials.Add(credential);
                await _context.SaveChangesAsync();

                // Clear challenge
                await _cacheService.RemoveAsync($"webauthn:reg:challenge:{user.Id}");

                await _auditService.LogAsync(
                    "WebAuthnCredentialRegistered",
                    "Authentication",
                    user.Id.ToString(),
                    null,
                    $"WebAuthn credential registered: {finalDeviceName} (Type: {success.Result.CredType}, AAGUID: {success.Result.Aaguid})");

                //_logger.LogInformation(
                //    "WebAuthn credential registered for user {UserId}: {DeviceName} (Type: {CredType})",
                //    user.Id, finalDeviceName, success.Result.CredType);

                return credential;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error completing WebAuthn registration for user {UserId}", user.Id);
                throw;
            }
        }

        public async Task<AssertionOptions> InitiateAuthenticationAsync(
            ApplicationUser user,
            List<UserCredential> credentials)
        {
            try
            {
                var allowedCredentials = credentials
                    .Where(c => !c.IsRevoked)
                    .Select(c => new PublicKeyCredentialDescriptor(c.CredentialId))
                    .ToList();

                if (!allowedCredentials.Any())
                {
                    throw new InvalidOperationException(
                        "No active WebAuthn credentials found for user");
                }

                var extensions = new AuthenticationExtensionsClientInputs();

                // Use dynamic invocation to avoid overload mismatch
                var options = ((dynamic)_fido2).GetAssertionOptions(
                    allowedCredentials,
                    UserVerificationRequirement.Required,
                    extensions);

                // Store challenge for verification
                await _cacheService.SetAsync(
                    $"webauthn:auth:challenge:{user.Id}",
                    options.Challenge,
                    TimeSpan.FromMinutes(5));

                await _auditService.LogAsync(
                    "WebAuthnAuthenticationInitiated",
                    "Authentication",
                    user.Id.ToString(),
                    $"{allowedCredentials.Count} credential(s) available for authentication",
                    null);

                _logger.LogInformation(
                    "WebAuthn authentication initiated for user {UserId} with {Count} credentials",
                    user.Id, allowedCredentials.Count);

                return options;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error initiating WebAuthn authentication for user {UserId}", user.Id);
                throw;
            }
        }

        public async Task<(bool Success, UserCredential? Credential)>
            CompleteAuthenticationAsync(
                ApplicationUser user,
                AuthenticatorAssertionRawResponse assertionResponse,
                List<UserCredential> credentials)
        {
            try
            {
                // Retrieve stored challenge
                var challenge = await _cacheService.GetAsync<byte[]>(
                    $"webauthn:auth:challenge:{user.Id}");

                if (challenge == null)
                {
                    throw new InvalidOperationException(
                        "No challenge found. Authentication may have expired.");
                }

                // Find the credential being used. The incoming Id is base64url encoded string.
                var credentialIdBytes = Base64UrlEncoder.DecodeBytes(assertionResponse.Id);
                var credential = credentials
                    .Where(c => !c.IsRevoked)
                    .FirstOrDefault(c => CompareByteArrays(c.CredentialId, credentialIdBytes));

                if (credential == null)
                {
                    _logger.LogWarning(
                        "Unknown or revoked credential used for user {UserId}", user.Id);

                    await _auditService.LogAsync(
                        "WebAuthnAuthenticationFailed",
                        "Security",
                        user.Id.ToString(),
                        "Unknown or revoked credential used",
                        null);

                    return (false, null);
                }

                var options = new AssertionOptions
                {
                    Challenge = challenge,
                    RpId = _configuration["WebAuthn:RelyingPartyId"]!,
                    AllowCredentials = new List<PublicKeyCredentialDescriptor>
                    {
                        new PublicKeyCredentialDescriptor(credential.CredentialId)
                    },
                    UserVerification = UserVerificationRequirement.Required
                };

                // Callback to verify user handle
                IsUserHandleOwnerOfCredentialIdAsync callback = async (args, cancellationToken) =>
                {
                    return await Task.FromResult(
                        CompareByteArrays(credential.UserHandle, args.UserHandle));
                };

                // Verify assertion (dynamic invocation to match available overloads)
                var result = await ((dynamic)_fido2).MakeAssertionAsync(
                    assertionResponse,
                    options,
                    credential.PublicKey,
                    null,
                    credential.SignatureCounter,
                    callback,
                    CancellationToken.None);

                // Normalize dynamic result to typed locals to avoid dynamic dispatch in logging
                var status = (string)result.Status;
                var errorMessage = result.ErrorMessage != null ? (string)result.ErrorMessage : null;
                var counter = Convert.ToUInt32(result.Counter);

                if (status != "ok")
                {
                    _logger.LogWarning(
                        "WebAuthn authentication failed for user {UserId}: {Error}",
                        user.Id, errorMessage);

                    await _auditService.LogAsync(
                        "WebAuthnAuthenticationFailed",
                        "Security",
                        user.Id.ToString(),
                        $"WebAuthn authentication failed: {errorMessage}",
                        null);

                    return (false, null);
                }

                // Verify signature counter to detect cloned credentials
                if (counter > 0 && counter <= credential.SignatureCounter)
                {
                    //_logger.LogError(
                    //    "Potential credential cloning detected for user {UserId}. " +
                    //    "Counter: {Counter}, Expected: >{Expected}",
                    //    user.Id, counter, credential.SignatureCounter);

                    await _auditService.LogAsync(
                        "WebAuthnCloningDetected",
                        "Security",
                        user.Id.ToString(),
                        $"Potential credential cloning detected. Counter: {counter}, Expected: >{credential.SignatureCounter}",
                        null);

                    // Revoke the credential for security
                    credential.IsRevoked = true;
                    await _context.SaveChangesAsync();

                    return (false, null);
                }

                // Update credential with latest counter and usage timestamp
                credential.SignatureCounter = counter;
                credential.LastUsedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Clear challenge
                await _cacheService.RemoveAsync($"webauthn:auth:challenge:{user.Id}");

                await _auditService.LogAsync(
                    "WebAuthnAuthenticationSuccess",
                    "Authentication",
                    user.Id.ToString(),
                    null,
                    $"WebAuthn authentication successful using device: {credential.DeviceName ?? "Unknown"}");

                _logger.LogInformation(
                    "WebAuthn authentication successful for user {UserId} using {DeviceName}",
                    user.Id, credential.DeviceName ?? "Unknown");

                return (true, credential);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error completing WebAuthn authentication for user {UserId}", user.Id);
                return (false, null);
            }
        }

        public async Task<bool> RevokeCredentialAsync(
            Guid userId,
            Guid credentialId,
            string reason)
        {
            try
            {
                var credential = await _context.UserCredentials
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == credentialId);

                if (credential == null)
                {
                    _logger.LogWarning(
                        "Credential {CredentialId} not found for user {UserId}",
                        credentialId, userId);
                    return false;
                }

                credential.IsRevoked = true;
                await _context.SaveChangesAsync();

                var deviceName = credential.DeviceName ?? "Unknown Device";

                await _auditService.LogAsync(
                    "WebAuthnCredentialRevoked",
                    "Authentication",
                    userId.ToString(),
                    deviceName,
                    $"Credential ID: {credentialId}, Reason: {reason}");

                _logger.LogInformation(
                    "WebAuthn credential {CredentialId} ({DeviceName}) revoked for user {UserId}. Reason: {Reason}",
                    credentialId, deviceName, userId, reason);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error revoking WebAuthn credential {CredentialId} for user {UserId}",
                    credentialId, userId);
                return false;
            }
        }

        private string DetermineDeviceName(Guid aaguid, string credType)
        {
            // Check for known authenticators by AAGUID
            var knownAuthenticators = new Dictionary<string, string>
            {
                // YubiKey variants
                ["f8a011f3-8c0a-4d15-8006-17111f9edc7d"] = "YubiKey 5 Series",
                ["2fc0579f-8113-47ea-b116-bb5a8db9202a"] = "YubiKey 5 NFC",
                ["cb69481e-8ff7-4039-93ec-0a2729a154a8"] = "YubiKey 5Ci",
                ["ee882879-721c-4913-9775-3dfcce97072a"] = "YubiKey Bio",
                ["73bb0cd4-e502-49b8-9c6f-b59445bf720b"] = "YubiKey 5 FIPS",

                // Windows Hello
                ["08987058-cadc-4b81-b6e1-30de50dcbe96"] = "Windows Hello (Hardware)",
                ["9ddd1817-af5a-4672-a2b9-3e3dd95000a9"] = "Windows Hello (Software)",
                ["6028b017-b1d4-4c02-b4b3-afcdafc96bb2"] = "Windows Hello",

                // Apple
                ["dd4ec289-e01d-41c9-bb89-70fa845d4bf2"] = "Touch ID (macOS)",
                ["00000000-0000-0000-0000-000000000000"] = "Face ID / Touch ID (iOS)",

                // Google Titan
                ["ea9b8d66-4d01-1d21-3ce4-b6b48cb575d4"] = "Google Titan Security Key",

                // Feitian
                ["77010bd7-212a-4fc9-b236-d2ca5e9d4084"] = "Feitian BioPass",
                ["833b721a-ff5f-4d00-bb2e-bdda3ec01e29"] = "Feitian ePass",

                // Android
                ["bada5566-a7aa-401f-bd96-45619a55120d"] = "Android SafetyNet",

                // SoloKeys
                ["8876631b-d4a0-427f-5773-0ec71c9e0279"] = "SoloKeys Solo",

                // Nitrokey
                ["a4e9fc6d-4cbe-4758-b8ba-37598bb5bbaa"] = "Nitrokey FIDO2"
            };

            var aaguidStr = aaguid.ToString();
            if (knownAuthenticators.TryGetValue(aaguidStr, out var authenticatorName))
            {
                return authenticatorName;
            }

            // Fallback based on credential type
            return credType switch
            {
                "public-key" => "Security Key",
                _ => "WebAuthn Device"
            };
        }

        private static bool CompareByteArrays(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }
    }
}