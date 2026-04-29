namespace InsureTrust.Web.Models
{
    public class RenewalHistoryViewModel
    {
        public int TotalRenewals { get; set; }
        public int SuccessfulRenewals { get; set; }
        public int FailedRenewals { get; set; }

        public List<PaymentHistoryItemViewModel> Renewals { get; set; } = new();
    }
}