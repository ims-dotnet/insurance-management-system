namespace InsureTrust.CalculatorService.Options;

public class CalculationOptions
{
    public const string SectionName = "Calculation";

    public decimal AgeFactor18To30 { get; set; } = 0.95m;
    public decimal AgeFactor31To45 { get; set; } = 1.10m;
    public decimal AgeFactor46To70 { get; set; } = 1.25m;

    public decimal PersonalCategoryFactor { get; set; } = 1.00m;
    public decimal BusinessCategoryFactor { get; set; } = 1.08m;

    public decimal AnnualReturnRate { get; set; } = 0.08m;
}