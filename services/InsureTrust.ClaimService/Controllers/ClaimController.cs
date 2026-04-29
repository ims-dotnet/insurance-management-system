using InsureTrust.ClaimService.DTOs;
using InsureTrust.ClaimService.Services;
using InsureTrust.ClaimService.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsureTrust.ClaimService.Controllers
{
    [ApiController]
    [Route("api/claim")]
    [Authorize]
    public class ClaimController : ControllerBase
    {
        private readonly IClaimService _service;
        private readonly IWebHostEnvironment _env;

        public ClaimController(IClaimService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        [HttpPost("submit/{policyId}")]
        public async Task<IActionResult> SubmitClaim(int policyId, [FromForm] SubmitClaimDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = string.IsNullOrEmpty(userIdString) ? 1 : int.Parse(userIdString);

            var uploadPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "claims");
            
            var claim = await _service.SubmitClaimAsync(policyId, dto, userId, uploadPath);
            return Ok(ApiResponse<ClaimDto>.SuccessResult(claim, "Claim submitted successfully."));
        }

        [HttpGet("my-claims")]
        public async Task<IActionResult> GetMyClaims()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = string.IsNullOrEmpty(userIdString) ? 1 : int.Parse(userIdString);

            var claims = await _service.GetMyClaimsAsync(userId);
            return Ok(ApiResponse<IEnumerable<ClaimDto>>.SuccessResult(claims));
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllClaims()
        {
            var claims = await _service.GetAllClaimsAsync();
            return Ok(ApiResponse<IEnumerable<ClaimDto>>.SuccessResult(claims));
        }

        [HttpPut("{claimId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateClaim(int claimId, [FromBody] UpdateClaimDto dto)
        {
            var claim = await _service.UpdateClaimAsync(claimId, dto);
            return Ok(ApiResponse<ClaimDto>.SuccessResult(claim, "Claim status updated."));
        }
    }
}