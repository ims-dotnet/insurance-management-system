using System.ComponentModel.DataAnnotations;

namespace InsureTrust.CalculatorService.DTOs
{
    public class CalculatorRequestDto
    {
        [Required]
        [Range(18, 70)]
        public int Age { get; set; }

        [Required]
        [Range(5000, 15000)]
        public decimal PackageAmount { get; set; }

        [Required]
        [Range(12, 36)]
        public int Tenure { get; set; }

        public string? PolicyCategory { get; set; }  // Personal, Business
    }

    public class CalculatorResultDto
    {
        public decimal EstimatedPremium { get; set; }
        public decimal TotalInvestment { get; set; }
        public decimal MaturityAmount { get; set; }
        public string Breakup { get; set; } = string.Empty;
    }

}
