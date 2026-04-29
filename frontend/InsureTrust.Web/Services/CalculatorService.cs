using System.Net.Http.Json;

namespace InsureTrust.Web.Services;

public record CalculatorRequestDto(int Age, decimal PackageAmount, int Tenure, string? PolicyCategory);
public record CalculatorResultDto(decimal EstimatedPremium, decimal TotalInvestment, decimal MaturityAmount, string Breakup);

public interface ICalculatorService
{
    Task<CalculatorResultDto?> EstimateAsync(CalculatorRequestDto request);
}

public class CalculatorService : ICalculatorService
{
    private readonly HttpClient _httpClient;

    public CalculatorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CalculatorResultDto?> EstimateAsync(CalculatorRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/calculator/estimate", request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<CalculatorResultDto>>();
                return apiResponse?.Data;
            }
        }
        catch { }

        return null;
    }
}
