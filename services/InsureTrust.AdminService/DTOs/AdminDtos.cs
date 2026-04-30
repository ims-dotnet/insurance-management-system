namespace InsureTrust.AdminService.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalActivePolicies { get; set; }
        public int TotalPendingPolicies { get; set; }
        public int TotalPendingClaims { get; set; }
        public int TotalOpenSupportTickets { get; set; }
        public int TotalUnreadNotifications { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class AdminUserDto
    {
        public int Id { get; set; }
        public string UserNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string KycStatus { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public int ActivePolicyCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AdminTransactionDto
    {
        public string PaymentNumber { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string PolicyNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
    }

    public class AdminPolicyDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class AdminClaimDto
    {
        public int Id { get; set; }
        public string ClaimStatus { get; set; } = string.Empty;
    }

    public class CreatePolicyTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BaseMonthlyPremium { get; set; }
        public int MinTenureMonths { get; set; }
        public int MaxTenureMonths { get; set; }
    }
}