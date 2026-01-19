using Fido2NetLib;
using HMS.Authentication.Domain.Entities;

namespace HMS.Authentication.Infrastructure.Interfaces
{
    public interface IWebAuthnService
    {
        Task<CredentialCreateOptions> InitiateRegistrationAsync(
            ApplicationUser user,
            List<UserCredential> existingCredentials);

        Task<UserCredential> CompleteRegistrationAsync(
            ApplicationUser user,
            AuthenticatorAttestationRawResponse attestationResponse,
            string? deviceName = null);

        Task<AssertionOptions> InitiateAuthenticationAsync(
            ApplicationUser user,
            List<UserCredential> credentials);

        Task<(bool Success, UserCredential? Credential)> CompleteAuthenticationAsync(
            ApplicationUser user,
            AuthenticatorAssertionRawResponse assertionResponse,
            List<UserCredential> credentials);

        Task<bool> RevokeCredentialAsync(
            Guid userId,
            Guid credentialId,
            string reason);
    }
}