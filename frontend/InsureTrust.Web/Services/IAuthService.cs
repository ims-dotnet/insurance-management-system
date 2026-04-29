using InsureTrust.Web.Models;

namespace InsureTrust.Web.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginViewModel model);
        Task<UserDto?> RegisterAsync(RegisterViewModel request);
        Task<IEnumerable<UserDto>?> GetAllUsersAsync();
        Task<UserDto?> GetProfileAsync();
        Task<UserDto?> UpdateProfileAsync(UpdateProfileViewModel request);
    }
}
