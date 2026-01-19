namespace HMS.Web.Services
{
    using HMS.Web.Interfaces;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;

    public class ApiClientService : IApiClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAuthService _authService;
        private readonly ILogger<ApiClientService> _logger;
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiClientService(
            IHttpClientFactory httpClientFactory,
            IAuthService authService,
            ILogger<ApiClientService> logger,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _authService = authService;
            _logger = logger;
            _configuration = configuration;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };
        }

        public async Task<T> GetAsync<T>(string endpoint) where T : class
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiGateway");
                await AddAuthorizationHeader(client);
                AddClientIdHeader(client);

                _logger.LogInformation("GET request to {Endpoint}", endpoint);

                var response = await client.GetAsync(endpoint);
                return await HandleResponse<T>(response, endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GET request failed for {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task<T> PostAsync<T>(string endpoint, object? data = null) where T : class
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiGateway");
                await AddAuthorizationHeader(client);
                AddClientIdHeader(client);

                _logger.LogInformation("POST request to {Endpoint}", endpoint);

                var content = data != null
                    ? new StringContent(
                        JsonSerializer.Serialize(data, _jsonOptions),
                        Encoding.UTF8,
                        "application/json")
                    : null;

                var response = await client.PostAsync(endpoint, content);
                return await HandleResponse<T>(response, endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST request failed for {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task<T> PutAsync<T>(string endpoint, object? data = null) where T : class
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiGateway");
                await AddAuthorizationHeader(client);
                AddClientIdHeader(client);

                _logger.LogInformation("PUT request to {Endpoint}", endpoint);

                var content = data != null
                    ? new StringContent(
                        JsonSerializer.Serialize(data, _jsonOptions),
                        Encoding.UTF8,
                        "application/json")
                    : null;

                var response = await client.PutAsync(endpoint, content);
                return await HandleResponse<T>(response, endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PUT request failed for {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task<T> DeleteAsync<T>(string endpoint) where T : class
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiGateway");
                await AddAuthorizationHeader(client);
                AddClientIdHeader(client);

                _logger.LogInformation("DELETE request to {Endpoint}", endpoint);

                var response = await client.DeleteAsync(endpoint);
                return await HandleResponse<T>(response, endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DELETE request failed for {Endpoint}", endpoint);
                throw;
            }
        }

        private async Task AddAuthorizationHeader(HttpClient client)
        {
            try
            {
                // Only add token if user is authenticated
                var token = await _authService.GetAccessTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    _logger.LogDebug("Authorization header added for authenticated request");
                }
                else
                {
                    _logger.LogDebug("No token available for authorization header");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to add authorization header");
            }
        }

        private void AddClientIdHeader(HttpClient client)
        {
            try
            {
                var clientId = _configuration.GetValue<string>("ApiGateway:ClientId") ?? "HMS-WebClient";

                // ✅ ADD X-API-Key HEADER (Required by ApiKeyMiddleware)
                if (!client.DefaultRequestHeaders.Contains("X-API-Key"))
                {
                    client.DefaultRequestHeaders.Add("X-API-Key", clientId);
                    _logger.LogDebug("X-API-Key header added: {ClientId}", clientId);
                }

                // Also add ClientId for rate limiting
                if (!client.DefaultRequestHeaders.Contains("ClientId"))
                {
                    client.DefaultRequestHeaders.Add("ClientId", clientId);
                    _logger.LogDebug("ClientId header added: {ClientId}", clientId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to add API headers");
            }
        }

        private async Task<T> HandleResponse<T>(HttpResponseMessage response, string endpoint) where T : class
        {
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("API Response - Endpoint: {Endpoint}, Status: {StatusCode}, ContentLength: {ContentLength}",
                endpoint, response.StatusCode, content.Length);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("API request to {Endpoint} returned {StatusCode}: {Content}",
                    endpoint, response.StatusCode, content);

                // ✅ Handle 401 Unauthorized with token refresh attempt
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Unauthorized response received. Attempting token refresh...");

                    // Try to refresh token
                    var refreshed = await _authService.RefreshTokenAsync();

                    if (refreshed)
                    {
                        _logger.LogInformation("Token refreshed successfully. Retry not implemented - user should retry request.");
                        // Note: In production, you might want to retry the original request here
                    }
                    else
                    {
                        _logger.LogWarning("Token refresh failed. User needs to re-authenticate.");
                    }

                    throw new HttpRequestException(
                        $"Unauthorized access. Please login again. Status: {response.StatusCode}");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new HttpRequestException(
                        $"Access denied. You don't have permission to access this resource. Status: {response.StatusCode}");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    throw new HttpRequestException(
                        $"Service temporarily unavailable (rate limited or down). Please try again later. Status: {response.StatusCode}");
                }

                // Try to parse error response
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                    return errorResponse ?? throw new HttpRequestException(
                        $"API returned {response.StatusCode}");
                }
                catch (JsonException)
                {
                    throw new HttpRequestException(
                        $"API request to {endpoint} failed with status {response.StatusCode}");
                }
            }

            // Successfully deserialize response
            if (string.IsNullOrEmpty(content))
            {
                _logger.LogWarning("Empty response content from {Endpoint}", endpoint);
                throw new InvalidOperationException($"API returned empty response from {endpoint}");
            }

            try
            {
                var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);

                if (result == null)
                {
                    _logger.LogWarning("Deserialized result is null for {Endpoint}", endpoint);
                    throw new JsonException("Deserialized response is null");
                }

                _logger.LogDebug("Successfully deserialized response from {Endpoint}", endpoint);
                return result;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize response from {Endpoint}. Content: {Content}",
                    endpoint, content.Length > 200 ? content.Substring(0, 200) + "..." : content);
                throw new InvalidOperationException(
                    $"Failed to parse API response from {endpoint}", ex);
            }
        }
    }
}