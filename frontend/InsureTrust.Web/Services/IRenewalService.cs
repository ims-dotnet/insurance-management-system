using InsureTrust.Web.Models;

namespace InsureTrust.Web.Services
{
    public interface IRenewalService
    {
        Task<ApiResponse<PaymentResultViewModel>?> InitiateRenewalPaymentAsync(object request);
        Task<ApiResponse<PaymentResultViewModel>?> InitiateFirstPaymentAsync(object request);
        Task<ApiResponse<List<PaymentHistoryItemViewModel>>?> GetPaymentHistoryAsync();
        Task<ApiResponse<List<PaymentHistoryItemViewModel>>?> GetAllPaymentsAsync();
        Task ApprovePaymentAsync(int paymentId);
        Task RejectPaymentAsync(int paymentId, string reason);
        Task<ApiResponse<ProductPolicyViewModel>?> GetPolicyDetailsByNumberAsync(string policyNumber);
        Task<ApiResponse<ProductPolicyViewModel>?> GetPolicyDetailsByIdAsync(int policyId);
    }
}
