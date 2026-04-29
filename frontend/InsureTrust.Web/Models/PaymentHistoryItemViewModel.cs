namespace InsureTrust.Web.Models
{
    public class PaymentHistoryItemViewModel
    {
        public int Id { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        public int? PolicyId { get; set; }
        public int? UserPolicyId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public int UserId { get; set; }
        public string? Remarks { get; set; }
    }
}