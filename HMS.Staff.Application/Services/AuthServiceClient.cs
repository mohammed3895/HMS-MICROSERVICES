using HMS.Staff.Application.DTOs;
using HMS.Staff.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace HMS.Staff.Application.Services
{
    public class AuthServiceClient : IAuthServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthServiceClient> _logger;
        private readonly string _authServiceUrl;

        public AuthServiceClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<AuthServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _authServiceUrl = configuration["ServiceEndpoints:Authentication"]
                ?? "https://localhost:5001";

            _httpClient.BaseAddress = new Uri(_authServiceUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<CreateStaffAuthResponse> CreateStaffUserAsync(CreateStaffAuthRequest request)
        {
            try
            {
                _logger.LogInformation("Creating staff user in Auth service: {Email}", request.Email);

                var response = await _httpClient.PostAsJsonAsync("/api/auth/staff/create", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Auth service returned error: {StatusCode} - {Content}",
                        response.StatusCode, errorContent);

                    return new CreateStaffAuthResponse
                    {
                        Success = false,
                        Message = "Failed to create user account",
                        Errors = new List<string> { $"Status: {response.StatusCode}", errorContent }
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<CreateStaffAuthResponse>();
                return result ?? new CreateStaffAuthResponse
                {
                    Success = false,
                    Message = "Invalid response from Auth service"
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error calling Auth service");
                return new CreateStaffAuthResponse
                {
                    Success = false,
                    Message = "Unable to connect to authentication service",
                    Errors = new List<string> { ex.Message }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff user in Auth service");
                return new CreateStaffAuthResponse
                {
                    Success = false,
                    Message = "An error occurred while creating user account",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<UserInfoResponse?> GetUserInfoAsync(Guid userId)
        {
            try
            {
                _logger.LogDebug("Fetching user info from Auth service: {UserId}", userId);

                var response = await _httpClient.GetAsync($"/api/users/{userId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Auth service returned {StatusCode} for user {UserId}",
                        response.StatusCode, userId);
                    return null;
                }

                var contentString = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(contentString))
                {
                    _logger.LogWarning("Auth service returned 200 but empty content for user {UserId}", userId);
                    return null;
                }

                _logger.LogDebug("Auth service response content: {Content}", contentString);

                // Auth service returns Result<GetUserResponse>, need to unwrap it
                var resultWrapper = await response.Content.ReadFromJsonAsync<AuthServiceResult>();

                if (resultWrapper == null || !resultWrapper.IsSuccess || resultWrapper.Data == null)
                {
                    _logger.LogWarning("Failed to get valid user info for {UserId}. IsSuccess: {IsSuccess}",
                        userId, resultWrapper?.IsSuccess);
                    return null;
                }

                // Map GetUserResponse to UserInfoResponse
                var userInfo = new UserInfoResponse
                {
                    UserId = resultWrapper.Data.UserId,
                    Email = resultWrapper.Data.Email ?? string.Empty,
                    FirstName = resultWrapper.Data.FirstName ?? string.Empty,
                    LastName = resultWrapper.Data.LastName ?? string.Empty,
                    PhoneNumber = resultWrapper.Data.PhoneNumber ?? string.Empty,
                    DateOfBirth = resultWrapper.Data.DateOfBirth,
                    ProfilePictureUrl = resultWrapper.Data.ProfilePictureUrl,
                    IsActive = resultWrapper.Data.IsActive
                };

                return userInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user info for {UserId}", userId);
                return null;
            }
        }

        public async Task<List<UserInfoResponse>> GetUsersInfoAsync(List<Guid> userIds)
        {
            try
            {
                if (!userIds.Any())
                {
                    return new List<UserInfoResponse>();
                }

                _logger.LogDebug("Fetching batch user info from Auth service: {Count} users", userIds.Count);

                // Create a batch request
                var batchRequest = new
                {
                    UserIds = userIds
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(batchRequest),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("/api/users/batch", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Auth service batch request returned {StatusCode}",
                        response.StatusCode);

                    // Fallback: fetch individually
                    return await FetchUsersIndividually(userIds);
                }

                var result = await response.Content.ReadFromJsonAsync<List<UserInfoResponse>>();
                return result ?? new List<UserInfoResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching batch user info");
                // Fallback: fetch individually
                return await FetchUsersIndividually(userIds);
            }
        }

        private async Task<List<UserInfoResponse>> FetchUsersIndividually(List<Guid> userIds)
        {
            var users = new List<UserInfoResponse>();

            foreach (var userId in userIds)
            {
                var user = await GetUserInfoAsync(userId);
                if (user != null)
                {
                    users.Add(user);
                }
            }

            return users;
        }

        public async Task<bool> UpdateUserBasicInfoAsync(Guid userId, UpdateUserBasicInfoRequest request)
        {
            try
            {
                _logger.LogInformation("Updating user basic info in Auth service: {UserId}", userId);

                var response = await _httpClient.PutAsJsonAsync(
                    $"/api/users/{userId}/profile",
                    request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to update user info: {StatusCode}", response.StatusCode);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user info for {UserId}", userId);
                return false;
            }
        }

        // Helper classes to deserialize the Result wrapper from Auth API
        private class AuthServiceResult
        {
            public bool IsSuccess { get; set; }
            public GetUserResponseData? Data { get; set; }
            public string? Message { get; set; }
            public List<string>? Errors { get; set; }
        }

        private class GetUserResponseData
        {
            public Guid UserId { get; set; }
            public string? Email { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? PhoneNumber { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string? ProfilePictureUrl { get; set; }
            public bool IsActive { get; set; }
        }
    }
}