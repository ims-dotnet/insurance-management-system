using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureTrust.Web.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<T?> PostAsync<T>(string url, object data)
        {
            AddJwtTokenIfAvailable();

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var friendlyError = await GetFriendlyErrorMessage(response);
                throw new Exception(friendlyError);
            }

            var responseJson = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            AddJwtTokenIfAvailable();

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var friendlyError = await GetFriendlyErrorMessage(response);
                throw new Exception(friendlyError);
            }

            var responseJson = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<T?> PostMultipartAsync<T>(string url, MultipartFormDataContent content)
        {
            AddJwtTokenIfAvailable();

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var friendlyError = await GetFriendlyErrorMessage(response);
                throw new Exception(friendlyError);
            }

            var responseJson = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<T?> PutAsync<T>(string url, object data)
        {
            AddJwtTokenIfAvailable();

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var friendlyError = await GetFriendlyErrorMessage(response);
                throw new Exception(friendlyError);
            }

            var responseJson = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private void AddJwtTokenIfAvailable()
        {
            // The key used by the Auth team to store the token
            const string SessionKey = "JWToken";

            var context = _httpContextAccessor.HttpContext;
            if (context == null) return;

            var token = context.Session.GetString(SessionKey);

            // Clear the header to prevent using a token from a previous request
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task<string> GetFriendlyErrorMessage(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                // 1. Check for standard ValidationProblemDetails "errors" object
                if (root.TryGetProperty("errors", out var errors))
                {
                    var messages = new List<string>();
                    foreach (var prop in errors.EnumerateObject())
                    {
                        foreach (var err in prop.Value.EnumerateArray())
                        {
                            messages.Add(err.GetString() ?? "");
                        }
                    }
                    return string.Join(" ", messages);
                }

                // 2. Check for custom "message" property (often used in custom Middlewares)
                if (root.TryGetProperty("message", out var message))
                {
                    return message.GetString() ?? "An unexpected error occurred.";
                }
            }
            catch
            {
                // If parsing fails, fall back to status code
            }

            return $"API Error: {response.StatusCode}";
        }
    }
}