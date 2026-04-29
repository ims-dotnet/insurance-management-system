namespace InsureTrust.Web.Models
{
    public class ProductPolicyViewModel
    {
        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public string PolicyType { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public decimal PackageAmount { get; set; }
    }
}
