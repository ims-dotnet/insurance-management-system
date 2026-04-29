using InsureTrust.Web.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace InsureTrust.Web.Services
{
    public class ClaimService : IClaimService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddToken()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["authToken"];
            _httpClient.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<IEnumerable<ClaimViewModel>> GetMyClaimsAsync()
        {
            AddToken();
            var response = await _httpClient.GetAsync("api/claim/my-claims");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (apiResponse.TryGetProperty("data", out var dataProperty))
                {
                    return JsonSerializer.Deserialize<List<ClaimViewModel>>(dataProperty.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ClaimViewModel>();
                }
            }
            return new List<ClaimViewModel>();
        }

        public async Task<bool> SubmitClaimAsync(int policyId, SubmitClaimViewModel model)
        {
            AddToken();
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.Description), "Description");
            content.Add(new StringContent(model.MaturityAmount.ToString()), "MaturityAmount");

            if (model.Documents != null)
            {
                foreach (var file in model.Documents)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.Add("Content-Type", file.ContentType);
                    content.Add(fileContent, "Documents", file.FileName);
                }
            }

            var response = await _httpClient.PostAsync($"api/claim/submit/{policyId}", content);
            return response.IsSuccessStatusCode;
        }
    }
}
