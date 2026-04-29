using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using InsureTrust.Web.Models;

namespace InsureTrust.Web.Services
{
    public class SupportService : ISupportService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl;

        public SupportService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            
            var supportServiceUrl = configuration["ServiceUrls:SupportService"] ?? "http://localhost:5135";
            _baseUrl = $"{supportServiceUrl}/api/support";
        }

        public async Task<ApiResponse<SupportQueryViewModel>?> SubmitSupportQueryAsync(MultipartFormDataContent content)
        {
            AddJwtTokenIfAvailable();
            var response = await _httpClient.PostAsync($"{_baseUrl}/submit", content);
            return await HandleResponse<ApiResponse<SupportQueryViewModel>>(response);
        }

        public async Task<ApiResponse<List<SupportQueryViewModel>>?> GetMyQueriesAsync()
        {
            return await GetAsync<ApiResponse<List<SupportQueryViewModel>>>($"{_baseUrl}/my-queries");
        }

        public async Task<ApiResponse<List<SupportQueryViewModel>>?> GetAllQueriesAsync()
        {
            return await GetAsync<ApiResponse<List<SupportQueryViewModel>>>($"{_baseUrl}/all");
        }

        public async Task<ApiResponse<object>?> UpdateQueryStatusAsync(int id, UpdateSupportStatusViewModel model)
        {
            return await PutAsync<ApiResponse<object>>($"{_baseUrl}/update/{id}", model);
        }


        private async Task<T?> GetAsync<T>(string url)
        {
            AddJwtTokenIfAvailable();
            var response = await _httpClient.GetAsync(url);
            return await HandleResponse<T>(response);
        }

        private async Task<T?> PutAsync<T>(string url, object data)
        {
            AddJwtTokenIfAvailable();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(url, content);
            return await HandleResponse<T>(response);
        }

        private async Task<T?> HandleResponse<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var friendlyError = await GetFriendlyErrorMessage(response);
                
                // Instead of throwing, we return a failed ApiResponse object
                // to allow the UI to handle it gracefully.
                try
                {
                    var result = Activator.CreateInstance<T>();
                    var type = typeof(T);
                    type.GetProperty("Success")?.SetValue(result, false);
                    type.GetProperty("Message")?.SetValue(result, friendlyError);
                    return result;
                }
                catch
                {
                    // Fallback for types that can't be instantiated this way
                    return default;
                }
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private void AddJwtTokenIfAvailable()
        {
            const string SessionKey = "JWToken";
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return;

            var token = context.Session.GetString(SessionKey);

            _httpClient.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task<string> GetFriendlyErrorMessage(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                if (root.TryGetProperty("errors", out var errors))
                {
                    var messages = new List<string>();
                    foreach (var prop in errors.EnumerateObject())
                        foreach (var err in prop.Value.EnumerateArray())
                            messages.Add(err.GetString() ?? "");
                    return string.Join(" ", messages);
                }
                if (root.TryGetProperty("message", out var message))
                    return message.GetString() ?? "An unexpected error occurred.";
            }
            catch { }
            return $"API Error: {response.StatusCode}";
        }
    }
}
