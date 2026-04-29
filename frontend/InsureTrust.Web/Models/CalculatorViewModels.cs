namespace InsureTrust.Web.Models
{
    public record CalculatorViewModel(int Age, decimal PackageAmount, int Tenure, string? PolicyCategory);
    public record CalculatorResultViewModel(decimal EstimatedPremium, decimal TotalInvestment, decimal MaturityAmount, string Breakup);
}
