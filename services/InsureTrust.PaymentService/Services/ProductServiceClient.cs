////namespace InsureTrust.PaymentService.Services
////{
////    public class ProductServiceClient : IProductServiceClient
////    {
////        private readonly HttpClient _httpClient;

////        public ProductServiceClient(HttpClient httpClient)
////        {
////            _httpClient = httpClient;
////        }

////        public async Task<bool> RenewPolicyAsync(int policyId)
////        {
////            try
////            {
////                var response = await _httpClient.PostAsync(
////                    $"/api/policy/renew/{policyId}", null);

////                return response.IsSuccessStatusCode;
////            }
////            catch
////            {
////                return false;
////            }
////        }
////    }
////}

//using System.Text.Json;
//using InsureTrust.PaymentService.DTOs;

//namespace InsureTrust.PaymentService.Services
//{
//    //public class ProductServiceClient : IProductServiceClient
//    {
//        private readonly HttpClient _httpClient;
//        private readonly IConfiguration _configuration;

//        public ProductServiceClient(HttpClient httpClient, IConfiguration configuration)
//        {
//            _httpClient = httpClient;
//            _configuration = configuration;
//        }

//        public async Task<ProductPolicyDto> GetPolicyAsync(int userPolicyId)
//        {
//            bool useMockData = _configuration.GetValue<bool>("MockSettings:UseMockPolicyData");

//            if (useMockData)
//            {
//                return GetMockPolicy(userPolicyId);
//            }

//            try
//            {
//                // Keep this endpoint configurable for future merge
//                var endpoint = $"/api/policy/details/{userPolicyId}";
//                var response = await _httpClient.GetAsync(endpoint);

//                if (!response.IsSuccessStatusCode)
//                {
//                    return GetMockPolicy(userPolicyId);
//                }

//                var json = await response.Content.ReadAsStringAsync();

//                var policy = JsonSerializer.Deserialize<ProductPolicyDto>(json, new JsonSerializerOptions
//                {
//                    PropertyNameCaseInsensitive = true
//                });

//                return policy ?? GetMockPolicy(userPolicyId);
//            }
//            catch
//            {
//                return GetMockPolicy(userPolicyId);
//            }
//        }

//        public async Task<bool> RenewPolicyAsync(int policyId)
//        {
//            bool useMockRenewal = _configuration.GetValue<bool>("MockSettings:UseMockRenewalResult");

//            if (useMockRenewal)
//            {
//                return true;
//            }

//            try
//            {
//                var response = await _httpClient.PostAsync($"/api/policy/renew/{policyId}", null);
//                return response.IsSuccessStatusCode;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private static ProductPolicyDto GetMockPolicy(int userPolicyId)
//        {
//            return userPolicyId switch
//            {
//                1 => new ProductPolicyDto
//                {
//                    PolicyType = "Health",
//                    ExpiryDate = DateTime.UtcNow.AddMonths(12)
//                },
//                2 => new ProductPolicyDto
//                {
//                    PolicyType = "TermLife",
//                    ExpiryDate = DateTime.UtcNow.AddMonths(24)
//                },
//                3 => new ProductPolicyDto
//                {
//                    PolicyType = "Vehicle",
//                    ExpiryDate = DateTime.UtcNow.AddDays(-5)
//                },
//                4 => new ProductPolicyDto
//                {
//                    PolicyType = "Property",
//                    ExpiryDate = DateTime.UtcNow.AddDays(-10)
//                },
//                _ => new ProductPolicyDto
//                {
//                    PolicyType = "Health",
//                    ExpiryDate = DateTime.UtcNow.AddMonths(12)
//                }
//            };
//        }
//    }
//}
using InsureTrust.PaymentService.DTOs;

namespace InsureTrust.PaymentService.Services
{
    public class ProductServiceClient : IProductServiceClient
    {
        private readonly HttpClient _httpClient;

        public ProductServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductPolicyDto?> GetPolicyByPolicyIdAsync(int policyId)
        {
            // Real integration later
            // return await _httpClient.GetFromJsonAsync<ProductPolicyDto>($"/api/policy/{policyId}");

            return await Task.FromResult(new ProductPolicyDto
            {
                PolicyId = policyId,
                PolicyNumber = $"POL{policyId:0000}",
                PolicyType = "Health",
                PremiumAmount = 5000m,
                ExpiryDate = DateTime.UtcNow.AddMonths(12)
            });
        }

        public async Task<ProductPolicyDto?> GetPolicyAsync(int userPolicyId)
        {
            // Real integration later
            // return await _httpClient.GetFromJsonAsync<ProductPolicyDto>($"/api/policy/user/{userPolicyId}");

            return await Task.FromResult(new ProductPolicyDto
            {
                PolicyId = 1,
                PolicyNumber = $"POL{userPolicyId:0000}",
                PolicyType = "Health",
                PremiumAmount = 5000m,
                ExpiryDate = DateTime.UtcNow.AddDays(-3)
            });
        }

        public async Task<bool> RenewPolicyAsync(int userPolicyId)
        {
            // Real integration later
            return await Task.FromResult(true);
        }
    }
}