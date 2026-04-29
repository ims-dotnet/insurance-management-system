using System.Net.Http.Json;
using InsureTrust.Web.Models;

namespace InsureTrust.Web.Services;

public interface ICalculatorService
{
    Task<CalculatorResultViewModel?> EstimateAsync(CalculatorViewModel request);
}

public class CalculatorService : ICalculatorService
{
    private readonly HttpClient _httpClient;

    public CalculatorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CalculatorResultViewModel?> EstimateAsync(CalculatorViewModel request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/calculator/estimate", request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<CalculatorResultViewModel>>();
                return apiResponse?.Data;
            }
        }
        catch { }

        return null;
    }
}
