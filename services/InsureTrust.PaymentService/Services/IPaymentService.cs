using InsureTrust.PaymentService.DTOs;

namespace InsureTrust.PaymentService.Services;

public interface IPaymentService
{
    Task<InitiatePaymentResponseDto> InitiateAsync(InitiatePaymentDto dto, int userId);
    Task<IEnumerable<PaymentDto>> GetHistoryAsync(int userId);

}

