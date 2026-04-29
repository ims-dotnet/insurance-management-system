namespace InsureTrust.Web.Models
{
    public class PaymentHistoryViewModel
    {
        public int TotalPayments { get; set; }
        public int SuccessfulPayments { get; set; }
        public int FailedPayments { get; set; }

        public List<PaymentHistoryItemViewModel> Payments { get; set; } = new();
    }
}
