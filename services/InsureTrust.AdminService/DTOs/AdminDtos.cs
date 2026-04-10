namespace InsureTrust.ClaimService.DTOs
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
        public int ActivePolicyCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime RegisteredAt { get; set; }
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

}
