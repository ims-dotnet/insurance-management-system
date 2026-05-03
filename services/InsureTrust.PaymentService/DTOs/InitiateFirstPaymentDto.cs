namespace InsureTrust.PaymentService.DTOs
{
    public class InitiateFirstPaymentDto
    {
        public int PolicyId { get; set; } // This is PolicyTypeId
        public string PaymentMethod { get; set; } = string.Empty;
        public int Tenure { get; set; }
        public decimal PackageAmount { get; set; }
    }
}