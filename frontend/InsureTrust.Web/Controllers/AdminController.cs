using InsureTrust.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace InsureTrust.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Dashboard()
        {
            var client = _httpClientFactory.CreateClient();
            var model = new AdminDashboardViewModel();

            try
            {
                var statsResponse = await client.GetAsync("http://localhost:5067/api/admin/dashboard");
                if (statsResponse.IsSuccessStatusCode)
                {
                    var content = await statsResponse.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (apiResponse.TryGetProperty("data", out var dataProperty))
                    {
                        model.Stats = JsonSerializer.Deserialize<DashboardStatsViewModel>(dataProperty.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new DashboardStatsViewModel();
                    }
                }

                var usersResponse = await client.GetAsync("http://localhost:5067/api/admin/users");
                if (usersResponse.IsSuccessStatusCode)
                {
                    var content = await usersResponse.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (apiResponse.TryGetProperty("data", out var dataProperty))
                    {
                        model.Users = JsonSerializer.Deserialize<List<AdminUserViewModel>>(dataProperty.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AdminUserViewModel>();
                    }
                }

                var transResponse = await client.GetAsync("http://localhost:5067/api/admin/transactions");
                if (transResponse.IsSuccessStatusCode)
                {
                    var content = await transResponse.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (apiResponse.TryGetProperty("data", out var dataProperty))
                    {
                        model.Transactions = JsonSerializer.Deserialize<List<AdminTransactionViewModel>>(dataProperty.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AdminTransactionViewModel>();
                    }
                }
            }
            catch
            {
                // Handle API unreachability during development
            }

            model.PolicyTypes = new List<AdminPolicyTypeViewModel>();

            return View(model);
        }

        [HttpGet]
        public IActionResult CreatePolicyType(int? id) => RedirectToAction(nameof(Dashboard));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePolicyType(AdminPolicyTypeFormViewModel model, string submitAction = "save") => RedirectToAction(nameof(Dashboard));

        public async Task<IActionResult> Users()
        {
            var client = _httpClientFactory.CreateClient();
            var users = new List<AdminUserViewModel>();
            try
            {
                var response = await client.GetAsync("http://localhost:5067/api/admin/users");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (apiResponse.TryGetProperty("data", out var dataProperty))
                    {
                        users = JsonSerializer.Deserialize<List<AdminUserViewModel>>(dataProperty.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AdminUserViewModel>();
                    }
                }
            }
            catch { }
            return View(users);
        }

        public IActionResult Policies() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePolicyStatus(int id, string action, string? remarks)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(action))
            {
                TempData["PolicyActionError"] = "Invalid policy action request.";
                return RedirectToAction(nameof(Policies));
            }

            var client = _httpClientFactory.CreateClient();

            try
            {
                var payload = new
                {
                    Action = action,
                    AdminRemarks = remarks ?? string.Empty
                };

                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"http://localhost:5067/api/policy/{id}/approve", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["PolicyActionSuccess"] = $"Policy {action} action completed.";
                }
                else
                {
                    TempData["PolicyActionError"] = "Could not update the policy at this time.";
                }
            }
            catch
            {
                TempData["PolicyActionError"] = "Policy service is currently unavailable.";
            }

            return RedirectToAction(nameof(Policies));
        }

        public async Task<IActionResult> Claims()
        {
            var client = _httpClientFactory.CreateClient();
            var claims = new List<AdminClaimViewModel>();
            try
            {
                var response = await client.GetAsync("http://localhost:5067/api/claim/all");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (apiResponse.TryGetProperty("data", out var dataProperty))
                    {
                        claims = JsonSerializer.Deserialize<List<AdminClaimViewModel>>(dataProperty.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AdminClaimViewModel>();
                    }
                }
            }
            catch { }
            return View(claims);
        }

        public Task<IActionResult> ClaimsReview() => Task.FromResult<IActionResult>(RedirectToAction(nameof(Claims)));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateClaimStatus(AdminClaimActionModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ClaimActionError"] = "Invalid data submitted.";
                return RedirectToAction(nameof(Claims));
            }

            // Custom validation: Deny requires remarks
            if (string.Equals(model.Action, "Deny", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(model.Remarks))
            {
                TempData["ClaimActionError"] = "Remarks are mandatory for denied claims.";
                return RedirectToAction(nameof(Claims));
            }

            var client = _httpClientFactory.CreateClient();

            try
            {
                var payload = new
                {
                    Action = model.Action,
                    AdminRemarks = model.Remarks ?? string.Empty
                };

                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"http://localhost:5067/api/claim/{model.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["ClaimActionSuccess"] = $"Claim {model.Action} action completed.";
                }
                else
                {
                    TempData["ClaimActionError"] = "Could not update claim status at this time.";
                }
            }
            catch
            {
                TempData["ClaimActionError"] = "Claim service is currently unavailable.";
            }

            return RedirectToAction(nameof(Claims));
        }

        public async Task<IActionResult> Transactions()
        {
            var client = _httpClientFactory.CreateClient();
            var transactions = new List<AdminTransactionViewModel>();
            try
            {
                var response = await client.GetAsync("http://localhost:5067/api/admin/transactions");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (apiResponse.TryGetProperty("data", out var dataProperty))
                    {
                        transactions = JsonSerializer.Deserialize<List<AdminTransactionViewModel>>(dataProperty.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AdminTransactionViewModel>();
                    }
                }
            }
            catch { }
            return View(transactions);
        }

        public async Task<IActionResult> Support()
        {
            var client = _httpClientFactory.CreateClient();
            var tickets = new List<AdminSupportViewModel>();
            try
            {
                var response = await client.GetAsync("http://localhost:5067/api/support/all");
                if (response.IsSuccessStatusCode)
                {
                    tickets = JsonSerializer.Deserialize<List<AdminSupportViewModel>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AdminSupportViewModel>();
                }
            }
            catch { }

            return View(tickets);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSupportStatus(int id, string status, string? adminResponse)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(status))
            {
                TempData["SupportActionError"] = "Invalid support update request.";
                return RedirectToAction(nameof(Support));
            }

            var client = _httpClientFactory.CreateClient();

            try
            {
                var payload = new
                {
                    Status = status,
                    AdminResponse = adminResponse ?? string.Empty
                };

                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"http://localhost:5067/api/support/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SupportActionSuccess"] = "Support ticket updated successfully.";
                }
                else
                {
                    TempData["SupportActionError"] = "Could not update support ticket at this time.";
                }
            }
            catch
            {
                TempData["SupportActionError"] = "Support service is currently unavailable.";
            }

            return RedirectToAction(nameof(Support));
        }
    }
}
