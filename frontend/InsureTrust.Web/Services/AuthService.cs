using InsureTrust.Web.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace InsureTrust.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
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

        public async Task<LoginResponseDto?> LoginAsync(LoginViewModel model)
        {
            var endpoint = model.LoginMode == "admin" ? "/api/auth/admin-login" : "/api/auth/login";
            
            try 
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, new { Email = model.Email, Password = model.Password });
                
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();
                    return apiResponse?.Data;
                }
            }
            catch { }

            return null;
        }

        public async Task<UserDto?> RegisterAsync(RegisterViewModel request)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(request.Name), "Name");
                content.Add(new StringContent(request.Email), "Email");
                content.Add(new StringContent(request.Password), "Password");
                content.Add(new StringContent(request.ConfirmPassword), "ConfirmPassword");
                content.Add(new StringContent(request.PhoneNo), "PhoneNo");
                content.Add(new StringContent(request.PanCard), "PanCard");

                if (request.KycDocument != null)
                {
                    var streamContent = new StreamContent(request.KycDocument.OpenReadStream());
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(request.KycDocument.ContentType);
                    content.Add(streamContent, "KycDocument", request.KycDocument.FileName);
                }

                var response = await _httpClient.PostAsync("/api/auth/register", content);
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
                    return apiResponse?.Data;
                }
            }
            catch { }

            return null;
        }

        public async Task<IEnumerable<UserDto>?> GetAllUsersAsync()
        {
            try
            {
                AddToken();
                var response = await _httpClient.GetAsync("/api/auth/users");
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<UserDto>>>();
                    return apiResponse?.Data;
                }
            }
            catch { }
            
            return null;
        }

        public async Task<UserDto?> GetProfileAsync()
        {
            try
            {
                AddToken();
                var response = await _httpClient.GetAsync("/api/auth/profile");
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
                    return apiResponse?.Data;
                }
            }
            catch { }

            return null;
        }

        public async Task<UserDto?> UpdateProfileAsync(UpdateProfileViewModel profileViewModel)
        {
            try
            {
                AddToken();
                using var content = new MultipartFormDataContent();

                if (!string.IsNullOrWhiteSpace(profileViewModel.Name))
                    content.Add(new StringContent(profileViewModel.Name), "Name");

                if (!string.IsNullOrWhiteSpace(profileViewModel.PhoneNo))
                    content.Add(new StringContent(profileViewModel.PhoneNo), "PhoneNo");

                if (profileViewModel.KycDocument != null)
                {
                    var streamContent = new StreamContent(profileViewModel.KycDocument.OpenReadStream());
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(profileViewModel.KycDocument.ContentType);
                    content.Add(streamContent, "KycDocument", profileViewModel.KycDocument.FileName);
                }

                var response = await _httpClient.PutAsync("/api/auth/profile", content);
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
                    return apiResponse?.Data;
                }
            }
            catch { }

            return null;
        }
    }
}
