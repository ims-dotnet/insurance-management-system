using InsureTrust.PaymentService.DTOs;

namespace InsureTrust.PaymentService.Services
{
    public interface IPaymentService
    {
        Task<InitiatePaymentResponseDto> InitiateFirstPaymentAsync(InitiateFirstPaymentDto dto, int userId);
        Task<InitiatePaymentResponseDto> InitiateRenewalPaymentAsync(InitiateRenewalPaymentDto dto, int userId);
        Task<IEnumerable<PaymentDto>> GetHistoryAsync(int userId);
        Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync();
        Task<bool> ApprovePaymentAsync(int paymentId);
        Task<bool> RejectPaymentAsync(int paymentId, string reason);
    }
}