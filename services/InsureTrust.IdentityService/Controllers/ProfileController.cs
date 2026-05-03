using System.Security.Claims;
using InsureTrust.IdentityService.DTOs;
using InsureTrust.IdentityService.Services;
using InsureTrust.IdentityService.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsureTrust.IdentityService.Controllers
{
    [Route("api/auth/profile")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _environment;

        public ProfileController(IAuthService authService, IWebHostEnvironment environment)
        {
            _authService = authService;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetProfileAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ApiResponse<UserDto>("Invalid token."));

            var result = await _authService.GetProfileAsync(userId);
            return Ok(new ApiResponse<UserDto>(result, "Profile retrieved successfully."));
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateProfileAsync([FromForm] UpdateProfileDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ApiResponse<UserDto>("Invalid token."));

            var uploadPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "kyc");
            var result = await _authService.UpdateProfileAsync(userId, dto, uploadPath);
            return Ok(new ApiResponse<UserDto>(result, "Profile updated successfully."));
        }

        [AllowAnonymous]
        [HttpPut("update-balance")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateBalanceAsync([FromBody] UpdateBalanceDto dto)
        {
            if (dto == null || dto.UserId <= 0 || dto.AmountToDeduct <= 0)
                return BadRequest(new ApiResponse<bool>("Invalid user ID or amount."));

            try
            {
                var result = await _authService.UpdateUserBalanceAsync(dto.UserId, dto.AmountToDeduct);
                return Ok(new ApiResponse<bool>(result, "Balance updated successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<bool>(ex.Message));
            }
        }
    }
}
