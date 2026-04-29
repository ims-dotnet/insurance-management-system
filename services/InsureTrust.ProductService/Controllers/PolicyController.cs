using InsureTrust.ProductService.DTOs;
using InsureTrust.ProductService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsureTrust.ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolicyController : ControllerBase
    {
        private readonly IPolicyService _service;
        private readonly ILogger<PolicyController> _logger;

        public PolicyController(IPolicyService service, ILogger<PolicyController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("userId");

            if (claim == null)
            {
                _logger.LogWarning("UserId not found in token");
                throw new UnauthorizedAccessException("UserId not found in token");
            }

            return int.Parse(claim.Value);
        }

        [AllowAnonymous]
        [HttpGet("types")]
        public async Task<IActionResult> GetPolicyTypes()
        {
            _logger.LogInformation("Fetching all policy types");

            var result = await _service.GetPolicyTypesAsync();

            _logger.LogInformation("Fetched {Count} policy types", result.Count());

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("types/{id}")]
        public async Task<IActionResult> GetPolicyTypeById(int id)
        {
            _logger.LogInformation("Fetching policy type {Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID received: {Id}", id);
                throw new ArgumentException("Invalid Id"); ;
            }

            var result = await _service.GetPolicyTypeByIdAsync(id);

            return Ok(result);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("types")]
        public async Task<IActionResult> CreatePolicyType([FromBody] CreatePolicyTypeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreatePolicyTypeAsync(dto);

            _logger.LogInformation("Policy type created with ID {Id}", result.Id);

            return CreatedAtAction(nameof(GetPolicyTypeById), new { id = result.Id }, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("types/{id}")]
        public async Task<IActionResult> UpdatePolicyType(int id, [FromBody] CreatePolicyTypeDto dto)
        {
            _logger.LogInformation("Updating policy type {Id}", id);

            if (id <= 0)
                throw new ArgumentException("Invalid Id");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed while updating policy type");
                return BadRequest(ModelState);
            }

            var result = await _service.UpdatePolicyTypeAsync(id, dto);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("types/{id}")]
        public async Task<IActionResult> DeletePolicyType(int id)
        {
            _logger.LogInformation("Deleting policy type {Id}", id);

            if (id <= 0)
                throw new ArgumentException("Invalid Id");

            await _service.DeletePolicyTypeAsync(id);

            _logger.LogInformation("Policy type deleted");

            return NoContent();
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyPolicies()
        {
            var userId = GetUserId();

            _logger.LogInformation("Fetching policies for user {UserId}", userId);

            var result = await _service.GetMyPoliciesAsync(userId);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPolicies()
        {
            _logger.LogInformation("Fetching all policies");

            var result = await _service.GetAllPoliciesAsync();

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingPolicies()
        {
            _logger.LogInformation("Fetching pending policies");

            var result = await _service.GetPendingPoliciesAsync();

            return Ok(result);
        }

        //[Authorize(Roles = "Customer")]
        [HttpPost("purchase")]
        public async Task<IActionResult> PurchasePolicy([FromBody] CreatePolicyDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for purchase");
                return BadRequest(ModelState);
            }

            var userId = GetUserId();

            _logger.LogInformation("User {UserId} purchasing policy", userId);

            var result = await _service.PurchaseAsync(dto, userId);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("approve/{policyId}")]
        public async Task<IActionResult> ApprovePolicy(int policyId, [FromBody] ApprovePolicyDto dto)
        {
            _logger.LogInformation("Approving policy {PolicyId}", policyId);

            if (policyId <= 0)
                throw new ArgumentException("Invalid Id");

            var adminId = GetUserId();

            var result = await _service.ApprovePolicyAsync(policyId, dto, adminId);

            return Ok(result);
        }

        [Authorize(Roles = "Customer")]
        [HttpPut("edit/{policyId}")]
        public async Task<IActionResult> EditPolicy(int policyId, [FromBody] EditPolicyDto dto)
        {
            _logger.LogInformation("Editing policy {PolicyId}", policyId);

            if (policyId <= 0)
                throw new ArgumentException("Invalid Policy Id");
            var userId = GetUserId();

            var result = await _service.EditPolicyAsync(policyId, dto, userId);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{policyId}")]
        public async Task<IActionResult> DeletePolicy(int policyId)
        {
            _logger.LogInformation("Deleting policy {PolicyId}", policyId);

            if (policyId <= 0)
                throw new ArgumentException("Invalid Policy Id");

            var adminId = GetUserId();

            await _service.DeletePolicyAsync(policyId, adminId, "Admin");

            return NoContent();
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("renew/{policyId}")]
        public async Task<IActionResult> RenewPolicy(int policyId)
        {
            _logger.LogInformation("Renewing policy {PolicyId}", policyId);

            if (policyId <= 0)
                throw new ArgumentException("Invalid Policy Id");

            var userId = GetUserId();

            var result = await _service.RenewPolicyAsync(policyId, userId);

            return Ok(result);
        }
    }
}