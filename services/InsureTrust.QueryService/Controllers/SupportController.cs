using System.Security.Claims;
using InsureTrust.SupportService.DTOs;
using InsureTrust.SupportService.Services;
using InsureTrust.SupportService.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsureTrust.SupportService.Controllers
{
    [ApiController]
    [Route("api/support")]
    public class SupportController : ControllerBase
    {
        private readonly ISupportService _supportService;
        private readonly IWebHostEnvironment _environment;

        public SupportController(ISupportService supportService, IWebHostEnvironment environment)
        {
            _supportService = supportService;
            _environment = environment;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("submit")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Submit([FromForm] CreateSupportQueryDto dto)
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var result = await _supportService.SubmitQueryAsync(dto, userId, _environment.WebRootPath);
            return Ok(ApiResponse<SupportQueryDto>.SuccessResponse(result, "Support query submitted successfully."));
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("my-queries")]
        public async Task<IActionResult> GetMyQueries()
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var result = await _supportService.GetMyQueriesAsync(userId);
            return Ok(ApiResponse<IEnumerable<SupportQueryDto>>.SuccessResponse(result));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllQueries()
        {
            var result = await _supportService.GetAllQueriesAsync();
            return Ok(ApiResponse<IEnumerable<SupportQueryDto>>.SuccessResponse(result));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update/{ticketId}")]
        public async Task<IActionResult> UpdateStatus(int ticketId, [FromBody] UpdateSupportStatusDto dto)
        {
            var result = await _supportService.UpdateStatusAsync(ticketId, dto);
            return Ok(ApiResponse<SupportQueryDto>.SuccessResponse(result, "Status updated successfully."));
        }
    }
}