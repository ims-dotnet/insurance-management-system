using InsureTrust.Web.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace InsureTrust.Web.Services;

// DTOs for request/response that aren't in Models
using System.ComponentModel.DataAnnotations;

public class RegisterRequestDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
    [Required]
    public string PhoneNo { get; set; } = string.Empty;
    [Required]
    public string PanCard { get; set; } = string.Empty;
    public IFormFile? KycDocument { get; set; }
}
public record ApiResponse<T>(T Data, string Message, List<string>? Errors);

public class UpdateProfileRequestDto
{
    public string? Name { get; set; }
    public string? PhoneNo { get; set; }
    public IFormFile? KycDocument { get; set; }
}

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginViewModel model);
    Task<UserDto?> RegisterAsync(RegisterRequestDto request);
    Task<IEnumerable<UserDto>?> GetAllUsersAsync(string token);
    Task<UserDto?> GetProfileAsync(string token);
    Task<UserDto?> UpdateProfileAsync(UpdateProfileRequestDto request, string token);
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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

    public async Task<UserDto?> RegisterAsync(RegisterRequestDto request)
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

    public async Task<IEnumerable<UserDto>?> GetAllUsersAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/users");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<UserDto>>>();
                return apiResponse?.Data;
            }
        }
        catch { }
        
        return null;
    }

    public async Task<UserDto?> GetProfileAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/profile");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
                return apiResponse?.Data;
            }
        }
        catch { }

        return null;
    }

    public async Task<UserDto?> UpdateProfileAsync(UpdateProfileRequestDto profileDto, string token)
    {
        try
        {
            using var content = new MultipartFormDataContent();

            if (!string.IsNullOrWhiteSpace(profileDto.Name))
                content.Add(new StringContent(profileDto.Name), "Name");

            if (!string.IsNullOrWhiteSpace(profileDto.PhoneNo))
                content.Add(new StringContent(profileDto.PhoneNo), "PhoneNo");

            if (profileDto.KycDocument != null)
            {
                var streamContent = new StreamContent(profileDto.KycDocument.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(profileDto.KycDocument.ContentType);
                content.Add(streamContent, "KycDocument", profileDto.KycDocument.FileName);
            }

            var request = new HttpRequestMessage(HttpMethod.Put, "/api/auth/profile");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
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
