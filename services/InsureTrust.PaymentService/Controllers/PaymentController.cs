using InsureTrust.PaymentService.DTOs;
using InsureTrust.PaymentService.Services;
using InsureTrust.PaymentService.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace InsureTrust.PaymentService.Controllers
{
    [Route("api/payment")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost("initiate-first-payment")]
        public async Task<IActionResult> InitiateFirstPayment([FromBody] InitiateFirstPaymentDto dto)
        {
            int userId = GetUserIdOrDefault();

            var result = await _service.InitiateFirstPaymentAsync(dto, userId);

            return Ok(ApiResponse<InitiatePaymentResponseDto>.SuccessResponse(
                result,
                "First payment completed successfully"
            ));
        }

        [HttpPost("initiate-renewal-payment")]
        public async Task<IActionResult> InitiateRenewalPayment([FromBody] InitiateRenewalPaymentDto dto)
        {
            int userId = GetUserIdOrDefault();

            var result = await _service.InitiateRenewalPaymentAsync(dto, userId);

            return Ok(ApiResponse<InitiatePaymentResponseDto>.SuccessResponse(
                result,
                "Renewal payment completed successfully"
            ));
        }

        [HttpGet("history")]
        public async Task<IActionResult> History()
        {
            int userId = GetUserIdOrDefault();

            var result = await _service.GetHistoryAsync(userId);

            return Ok(ApiResponse<IEnumerable<PaymentDto>>.SuccessResponse(
                result,
                "Payment history fetched successfully"
            ));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllPaymentsAsync();

            return Ok(ApiResponse<IEnumerable<PaymentDto>>.SuccessResponse(
                result,
                "All payments fetched successfully"
            ));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            await _service.ApprovePaymentAsync(id);
            return Ok(ApiResponse<object?>.SuccessResponse(null, "Payment approved successfully"));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectionRequestDto request)
        {
            await _service.RejectPaymentAsync(id, request.Reason);
            return Ok(ApiResponse<object?>.SuccessResponse(null, "Payment rejected successfully"));
        }
        
        [HttpGet("policy-details/number/{policyNumber}")]
        public async Task<IActionResult> GetPolicyDetailsByNumber(string policyNumber)
        {
            var result = await _service.GetPolicyDetailsByNumberAsync(policyNumber);
            return Ok(ApiResponse<ProductPolicyDto?>.SuccessResponse(result, "Policy details fetched successfully"));
        }

        [HttpGet("policy-details/{policyId:int}")]
        public async Task<IActionResult> GetPolicyDetailsById(int policyId)
        {
            var result = await _service.GetPolicyDetailsByIdAsync(policyId);
            return Ok(ApiResponse<ProductPolicyDto?>.SuccessResponse(result, "Policy details fetched successfully"));
        }



        private int GetUserIdOrDefault()
        {
            var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(claimValue, out var userId)
                ? userId
                : 1;
        }
    }
}
