using InsureTrust.IdentityService.DTOs;
using InsureTrust.IdentityService.Services;
using InsureTrust.IdentityService.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsureTrust.IdentityService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        private readonly IWebHostEnvironment _environment;

        public AuthController(IAuthService service, IWebHostEnvironment environment)
        {
            _service = service;
            _environment = environment;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<UserDto>>> RegisterAsync([FromForm] RegisterDto dto)
        {
            var uploadPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "kyc");
            var result = await _service.RegisterAsync(dto, uploadPath);
            return Ok(new ApiResponse<UserDto>(result, "User registered successfully."));
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> LoginAsync([FromBody] LoginDto dto)
        {
            var result = await _service.LoginAsync(dto);
            return Ok(new ApiResponse<LoginResponseDto>(result, "Login successful."));
        }

        [HttpPost("admin-login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> AdminLoginAsync([FromBody] LoginDto dto)
        {
            var result = await _service.AdminLoginAsync(dto);
            return Ok(new ApiResponse<LoginResponseDto>(result, "Admin login successful."));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAllUsersAsync()
        {
            var result = await _service.GetAllUsersAsync();
            return Ok(new ApiResponse<IEnumerable<UserDto>>(result, "Users retrieved successfully."));
        }
    }
}
