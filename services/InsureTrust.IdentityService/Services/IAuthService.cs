using InsureTrust.IdentityService.DTOs;

namespace InsureTrust.IdentityService.Services;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterDto dto, string uploadPath);
    Task<LoginResponseDto> LoginAsync(LoginDto dto);
    Task<LoginResponseDto> AdminLoginAsync(LoginDto dto);
    Task<UserDto> GetProfileAsync(int userId);
    Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileDto dto, string uploadPath);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
}

