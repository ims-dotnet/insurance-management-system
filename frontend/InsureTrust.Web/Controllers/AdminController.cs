using InsureTrust.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureTrust.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _gatewayUrl;

        public AdminController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _gatewayUrl = configuration["ApiBaseUrls:Gateway"] ?? throw new InvalidOperationException("Gateway URL not configured.");
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_gatewayUrl);
        }

        private void AddToken()
        {
            var token = Request.Cookies["authToken"];
            _httpClient.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewBag.HideSidebar = true;
            AddToken();
            var model = new AdminDashboardViewModel();

            try
            {
                var statsTask = _httpClient.GetAsync("api/admin/dashboard");
                var usersTask = _httpClient.GetAsync("api/admin/users");
                var transTask = _httpClient.GetAsync("api/admin/transactions");
                var pendingPoliciesTask = _httpClient.GetAsync("api/policy/pending");
                var allClaimsTask = _httpClient.GetAsync("api/claim/all");
                var supportTask = _httpClient.GetAsync("api/queries/all");
                var policyTypesTask = _httpClient.GetAsync("api/policy/types");

                await Task.WhenAll(statsTask, usersTask, transTask, pendingPoliciesTask, allClaimsTask, supportTask, policyTypesTask);

                // Process Stats
                var statsResponse = await statsTask;
                if (statsResponse.IsSuccessStatusCode)
                {
                    var content = await statsResponse.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (apiResponse.TryGetProperty("data", out var dataProperty))
                    {
                        model.Stats = JsonSerializer.Deserialize<DashboardStatsViewModel>(dataProperty.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new DashboardStatsViewModel();
                    }
                }

                // Process Users
                var usersResponse = await usersTask;
                if (usersResponse.IsSuccessStatusCode)
                {
                    var content = await usersResponse.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (apiResponse.TryGetProperty("data", out var dataProperty))
                    {
                        model.Users = JsonSerializer.Deserialize<List<AdminUserViewModel>>(dataProperty.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AdminUserViewModel>();
                    }
                }

                // Process Transactions
                var transResponse = await transTask;
                if (transResponse.IsSuccessStatusCode)
                {
                    var content = await transResponse.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (apiResponse.TryGetProperty("data", out var dataProperty))
                    {
                        model.Transactions = JsonSerializer.Deserialize<List<AdminTransactionViewModel>>(dataProperty.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AdminTransactionViewModel>();
                    }
                }

                // Process Pending Policies
                var pendingPoliciesResponse = await pendingPoliciesTask;
                if (pendingPoliciesResponse.IsSuccessStatusCode)
                {
                    var content = await pendingPoliciesResponse.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (apiResponse.TryGetProperty("data", out var dataProperty))
                    {
                        model.PendingPolicies = JsonSerializer.Deserialize<List<AdminPolicyViewModel>>(dataProperty.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AdminPolicyViewModel>();
                    }
                }

                // Process Claims
                var allClaimsResponse = await allClaimsTask;
                if (allClaimsResponse.IsSuccessStatusCode)
                {
                    var content = await allClaimsResponse.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (apiResponse.TryGetProperty("data", out var dataProperty))
                    {
                        var allClaims = JsonSerializer.Deserialize<List<AdminClaimViewModel>>(dataProperty.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AdminClaimViewModel>();
                        model.PendingClaims = allClaims.Where(c => c.Status?.ToLower() == "pending").ToList();
                    }
                }

                // Process Support Tickets
                var supportResponse = await supportTask;
                if (supportResponse.IsSuccessStatusCode)
                {
                    var content = await supportResponse.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (apiResponse.TryGetProperty("data", out var dataProperty))
                    {
                        var allTickets = JsonSerializer.Deserialize<List<AdminSupportViewModel>>(dataProperty.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AdminSupportViewModel>();
                        model.SupportTickets = allTickets.Where(t => t.Status != "Resolved").ToList();
                        model.Stats.TotalOpenSupportTickets = model.SupportTickets.Count;
                    }
                }

                // Process Policy Types
                var policyTypesResponse = await policyTypesTask;
                if (policyTypesResponse.IsSuccessStatusCode)
                {
                    var content = await policyTypesResponse.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (apiResponse.TryGetProperty("data", out var dataProperty))
                    {
                        model.PolicyTypes = JsonSerializer.Deserialize<List<AdminPolicyTypeViewModel>>(dataProperty.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AdminPolicyTypeViewModel>();
                    }
                }
            }
            catch
            {
                // Handle API unreachability
            }

            return View(model);
        }

        public async Task<IActionResult> Users()
        {
            AddToken();
            var users = new List<AdminUserViewModel>();
            try
            {
                var response = await _httpClient.GetAsync("api/admin/users");
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

            AddToken();
            try
            {
                var payload = new { Action = action, AdminRemarks = remarks ?? string.Empty };
                var response = await _httpClient.PutAsJsonAsync($"api/policy/approve/{id}", payload);

                if (response.IsSuccessStatusCode)
                    TempData["PolicyActionSuccess"] = $"Policy {action} action completed.";
                else
                    TempData["PolicyActionError"] = "Could not update the policy.";
            }
            catch
            {
                TempData["PolicyActionError"] = "Policy service is currently unavailable.";
            }

            return RedirectToAction(nameof(Policies));
        }

        public async Task<IActionResult> Claims()
        {
            AddToken();
            var claims = new List<AdminClaimViewModel>();
            try
            {
                var response = await _httpClient.GetAsync("api/claim/all");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateClaimStatus(AdminClaimActionModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ClaimActionError"] = "Invalid data submitted.";
                return RedirectToAction(nameof(Claims));
            }

            AddToken();
            try
            {
                var payload = new { Action = model.Action, AdminRemarks = model.Remarks ?? string.Empty };
                var response = await _httpClient.PutAsJsonAsync($"api/claim/{model.Id}", payload);

                if (response.IsSuccessStatusCode)
                    TempData["ClaimActionSuccess"] = $"Claim {model.Action} action completed.";
                else
                    TempData["ClaimActionError"] = "Could not update claim status.";
            }
            catch
            {
                TempData["ClaimActionError"] = "Claim service is unavailable.";
            }

            return RedirectToAction(nameof(Claims));
        }

        public async Task<IActionResult> Transactions()
        {
            AddToken();
            var transactions = new List<AdminTransactionViewModel>();
            try
            {
                var response = await _httpClient.GetAsync("api/admin/transactions");
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
            AddToken();
            var tickets = new List<AdminSupportViewModel>();
            try
            {
                var response = await _httpClient.GetAsync("api/queries/all");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (apiResponse.TryGetProperty("data", out var dataProperty))
                    {
                        tickets = JsonSerializer.Deserialize<List<AdminSupportViewModel>>(dataProperty.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AdminSupportViewModel>();
                    }
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

            AddToken();
            try
            {
                var payload = new { Status = status, AdminResponse = adminResponse ?? string.Empty };
                var response = await _httpClient.PutAsJsonAsync($"api/queries/update/{id}", payload);

                if (response.IsSuccessStatusCode)
                    TempData["SupportActionSuccess"] = "Support ticket updated successfully.";
                else
                    TempData["SupportActionError"] = "Could not update support ticket.";
            }
            catch
            {
                TempData["SupportActionError"] = "Support service is unavailable.";
            }

            return RedirectToAction(nameof(Support));
        }
    }
}
