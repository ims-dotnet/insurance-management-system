namespace InsureTrust.Web.Models
{
    public class MasterPaymentLedgerViewModel
    {
        public int TotalPayments { get; set; }
        public int PendingApprovals { get; set; }
        public int ApprovedPayments { get; set; }
        public int RenewalPayments { get; set; }
        public int FirstTimePayments { get; set; }
        public decimal TotalAmount { get; set; }

        public List<MasterPaymentLedgerItemViewModel> Payments { get; set; } = new();

        // Search filter (unified search by TransactionID or UserPolicyID)
        public string? SearchQuery { get; set; }
        public string? FilterType { get; set; }  // "all", "first-time", "renewal"
        public string? FilterStatus { get; set; } // "all", "pending", "approved", "failed"
    }

    public class MasterPaymentLedgerItemViewModel
    {
        public int Id { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int? UserPolicyId { get; set; }
        public int? PolicyId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string? Remarks { get; set; }

        /// <summary>Derived: "First-Time" or "Renewal"</summary>
        public string PaymentType =>
            !string.IsNullOrWhiteSpace(Remarks) &&
            (Remarks.Contains("Renew", StringComparison.OrdinalIgnoreCase) ||
             Remarks.Contains("Renewal", StringComparison.OrdinalIgnoreCase))
                ? "Renewal"
                : "First-Time";

        /// <summary>True when the payment is waiting for admin approval</summary>
        public bool IsPendingAdminApproval =>
            !string.IsNullOrWhiteSpace(Remarks) &&
            Remarks.Contains("Pending Admin Approval", StringComparison.OrdinalIgnoreCase);

        /// <summary>True when the admin has already approved</summary>
        public bool IsAdminApproved =>
            !string.IsNullOrWhiteSpace(Remarks) &&
            Remarks.Contains("Admin Approved", StringComparison.OrdinalIgnoreCase);

        /// <summary>True when the admin has rejected the payment</summary>
        public bool IsRejected =>
            !string.IsNullOrWhiteSpace(Remarks) &&
            Remarks.Contains("Admin Rejected", StringComparison.OrdinalIgnoreCase);

        public string? RejectionReason =>
            IsRejected ? Remarks?.Replace("Admin Rejected: ", "", StringComparison.OrdinalIgnoreCase) : null;
    }
}
