using InsureTrust.AdminService.Services;
using InsureTrust.AdminService.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsureTrust.AdminService.Controllers
{
    [ApiController]
    [Route("api/admin")]
    //[Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService service, ILogger<AdminController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // 🔹 Dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            _logger.LogInformation("Admin requested Dashboard statistics.");
            var stats = await _service.GetDashboardAsync();
            return Ok(ApiResponse<object>.SuccessResult(stats));
        }

        // 🔹 Users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            _logger.LogInformation("Admin requested User Management list.");
            var users = await _service.GetUsersAsync();
            return Ok(ApiResponse<object>.SuccessResult(users));
        }

        // 🔹 Transactions
        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions()
        {
            _logger.LogInformation("Admin requested Transaction Logs.");
            var transactions = await _service.GetTransactionsAsync();
            return Ok(ApiResponse<object>.SuccessResult(transactions));
        }
    }
}