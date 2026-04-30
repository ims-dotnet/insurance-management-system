using System.ComponentModel.DataAnnotations;

namespace InsureTrust.Web.Models
{
    public class DashboardStatsViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalActivePolicies { get; set; }
        public int TotalPendingPolicies { get; set; }
        public int TotalPendingClaims { get; set; }
        public int TotalOpenSupportTickets { get; set; }
        public int TotalUnreadNotifications { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class AdminUserViewModel
    {
        public int Id { get; set; }
        public string UserNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int ActivePolicyCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime RegisteredAt { get; set; }
    }

    public class AdminTransactionViewModel
    {
        public string PaymentNumber { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string PolicyNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
    }

    public class AdminPolicyViewModel
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public string PolicyType { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal PackageAmount { get; set; }
        public string? AdminRemarks { get; set; }
    }

    public class AdminClaimViewModel
    {
        public int Id { get; set; }
        public string ClaimNumber { get; set; } = string.Empty;
        public string PolicyNumber { get; set; } = string.Empty;
        public string PolicyTypeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal MaturityAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public List<string> DocumentUrls { get; set; } = new();
        public string? AdminRemarks { get; set; }
    }

    public class AdminClaimActionModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Action { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }

    public class AdminSupportViewModel
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? AttachmentUrl { get; set; }
        public string? AdminResponse { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }

    public class AdminPolicyTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BaseMonthlyPremium { get; set; }
        public int MinTenureMonths { get; set; }
        public int MaxTenureMonths { get; set; }
        public string Icon { get; set; } = "🛡️";
        public string CoverageDetails { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }

    public class AdminPolicyTypeFormViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = "Personal";
        public string Description { get; set; } = string.Empty;
        public string CoverageDetails { get; set; } = string.Empty;
        public string Icon { get; set; } = "🛡️";
        public decimal BaseMonthlyPremium { get; set; } = 5000;
        public int MinTenureMonths { get; set; } = 12;
        public int MaxTenureMonths { get; set; } = 120;
    }
    
    public class AdminDashboardViewModel
    {
        public DashboardStatsViewModel Stats { get; set; } = new();
        public List<AdminUserViewModel> Users { get; set; } = new();
        public List<AdminTransactionViewModel> Transactions { get; set; } = new();
        public List<AdminPolicyTypeViewModel> PolicyTypes { get; set; } = new();
        public List<AdminPolicyViewModel> PendingPolicies { get; set; } = new();
        public List<AdminClaimViewModel> PendingClaims { get; set; } = new();
        public List<AdminSupportViewModel> SupportTickets { get; set; } = new();
    }
}
