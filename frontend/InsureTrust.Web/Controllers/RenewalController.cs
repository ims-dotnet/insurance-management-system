
using InsureTrust.Web.Models;
using InsureTrust.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsureTrust.Web.Controllers
{
    public class RenewalController : Controller
    {
        private readonly ApiClient _apiClient;

        public RenewalController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public IActionResult Checkout(int? policyId)
        {
            var model = new PolicyPaymentViewModel
            {
                PolicyId = policyId ?? 1,
                PaymentCategory = "UPI",
                PaymentMethod = "GPay",
                IsRenewal = false,
                Amount = 5000
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(PolicyPaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Please fill all required fields correctly.";
                return View(model);
            }

            return RedirectToAction("PaymentVerification", new
            {
                policyId = model.PolicyId,
                userPolicyId = model.UserPolicyId,
                paymentCategory = model.PaymentCategory,
                paymentMethod = model.PaymentMethod,
                isRenewal = false,
                amount = model.Amount
            });
        }

        [HttpGet]
        public IActionResult RenewPolicy(int? userPolicyId)
        {
            var model = new PolicyPaymentViewModel
            {
                UserPolicyId = userPolicyId ?? 101,
                PaymentCategory = "UPI",
                PaymentMethod = "GPay",
                IsRenewal = true,
                Amount = 5000
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RenewPolicy(PolicyPaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Please fill all required fields correctly.";
                return View(model);
            }

            return RedirectToAction("PaymentVerification", new
            {
                policyId = model.PolicyId,
                userPolicyId = model.UserPolicyId,
                paymentCategory = model.PaymentCategory,
                paymentMethod = model.PaymentMethod,
                isRenewal = true,
                amount = model.Amount
            });
        }

        [HttpGet]
        public IActionResult PaymentVerification(
            int policyId,
            int userPolicyId,
            string paymentCategory,
            string paymentMethod,
            bool isRenewal,
            decimal amount = 5000)
        {
            var model = new PolicyPaymentViewModel
            {
                PolicyId = policyId,
                UserPolicyId = userPolicyId,
                PaymentCategory = paymentCategory,
                PaymentMethod = paymentMethod,
                IsRenewal = isRenewal,
                Amount = amount
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaymentVerification(PolicyPaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Please fill all required fields correctly.";
                return View(model);
            }

            try
            {
                ApiResponse<PaymentResultViewModel>? apiResponse;

                if (model.IsRenewal)
                {
                    var request = new
                    {
                        userPolicyId = model.UserPolicyId,
                        paymentMethod = model.PaymentMethod
                    };

                    apiResponse = await _apiClient.PostAsync<ApiResponse<PaymentResultViewModel>>(
                        "https://localhost:7003/api/payment/initiate-renewal-payment",
                        request);
                }
                else
                {
                    var request = new
                    {
                        policyId = model.PolicyId,
                        paymentMethod = model.PaymentMethod
                    };

                    apiResponse = await _apiClient.PostAsync<ApiResponse<PaymentResultViewModel>>(
                        "https://localhost:7003/api/payment/initiate-first-payment",
                        request);
                }

                var result = apiResponse?.Data;

                if (result == null)
                {
                    model.ErrorMessage = apiResponse?.Message ?? "No response received from payment service.";
                    return View(model);
                }

                var resultModel = new PaymentResultViewModel
                {
                    UserPolicyId = model.IsRenewal
                        ? model.UserPolicyId
                        : result.GeneratedUserPolicyId ?? result.UserPolicyId,

                    GeneratedUserPolicyId = result.GeneratedUserPolicyId,
                    PaymentNumber = result.PaymentNumber ?? string.Empty,
                    Amount = result.Amount,
                    Status = result.Status ?? string.Empty,
                    TransactionId = result.TransactionId ?? string.Empty,
                    IsRenewal = model.IsRenewal,
                    PaymentMethod = model.PaymentMethod,
                    ErrorMessage = apiResponse?.Success == true ? null : apiResponse?.Message
                };

                if (string.Equals(result.Status, "Success", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Success", resultModel);
                }

                resultModel.ErrorMessage ??= "Payment could not be completed.";
                return RedirectToAction("Failed", resultModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Failed", new PaymentResultViewModel
                {
                    UserPolicyId = model.UserPolicyId,
                    IsRenewal = model.IsRenewal,
                    PaymentMethod = model.PaymentMethod,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> RenewalHistory()
        {
            try
            {
                var apiResponse = await _apiClient.GetAsync<ApiResponse<List<PaymentHistoryItemViewModel>>>(
                    "https://localhost:7003/api/payment/history");

                var payments = apiResponse?.Data ?? new List<PaymentHistoryItemViewModel>();

                var renewalItems = payments
                    .Where(x => !string.IsNullOrWhiteSpace(x.Remarks) &&
                               (x.Remarks.Contains("Renew", StringComparison.OrdinalIgnoreCase) ||
                                x.Remarks.Contains("Renewal", StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(x => x.PaymentDate)
                    .ToList();

                var model = new RenewalHistoryViewModel
                {
                    TotalRenewals = renewalItems.Count,
                    SuccessfulRenewals = renewalItems.Count(x =>
                        string.Equals(x.Status, "Success", StringComparison.OrdinalIgnoreCase)),
                    FailedRenewals = renewalItems.Count(x =>
                        string.Equals(x.Status, "Failed", StringComparison.OrdinalIgnoreCase)),
                    Renewals = renewalItems
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new RenewalHistoryViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> PaymentHistory()
        {
            try
            {
                var apiResponse = await _apiClient.GetAsync<ApiResponse<List<PaymentHistoryItemViewModel>>>(
                    "https://localhost:7003/api/payment/history");

                var payments = apiResponse?.Data ?? new List<PaymentHistoryItemViewModel>();

                // Filter for first-time payments (those that don't contain 'Renew')
                var firstTimePayments = payments
                    .Where(x => string.IsNullOrWhiteSpace(x.Remarks) ||
                               (!x.Remarks.Contains("Renew", StringComparison.OrdinalIgnoreCase) &&
                                !x.Remarks.Contains("Renewal", StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(x => x.PaymentDate)
                    .ToList();

                var model = new PaymentHistoryViewModel
                {
                    TotalPayments = firstTimePayments.Count,
                    SuccessfulPayments = firstTimePayments.Count(x =>
                        string.Equals(x.Status, "Success", StringComparison.OrdinalIgnoreCase)),
                    FailedPayments = firstTimePayments.Count(x =>
                        string.Equals(x.Status, "Failed", StringComparison.OrdinalIgnoreCase)),
                    Payments = firstTimePayments
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new PaymentHistoryViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> MasterPaymentLedger(
            string? searchQuery,
            string? filterType,
            string? filterStatus)
        {
            try
            {
                var apiResponse = await _apiClient.GetAsync<ApiResponse<List<PaymentHistoryItemViewModel>>>(
                    "https://localhost:7003/api/payment/all");

                var allPayments = apiResponse?.Data ?? new List<PaymentHistoryItemViewModel>();

                // ── Apply search (Transaction ID or Policy User ID) ───────────
                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    allPayments = allPayments
                        .Where(p => 
                            (!string.IsNullOrEmpty(p.TransactionId) && p.TransactionId.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                            (p.UserPolicyId.HasValue && p.UserPolicyId.Value.ToString() == searchQuery)
                        )
                        .ToList();
                }

                // ── Apply type filter ─────────────────────────────────────────
                if (!string.IsNullOrWhiteSpace(filterType) && filterType != "all")
                {
                    bool isRenewal = filterType == "renewal";
                    allPayments = allPayments.Where(p =>
                    {
                        bool hasRenewalRemark = !string.IsNullOrWhiteSpace(p.Remarks) &&
                            (p.Remarks.Contains("Renew", StringComparison.OrdinalIgnoreCase) ||
                             p.Remarks.Contains("Renewal", StringComparison.OrdinalIgnoreCase));
                        return isRenewal ? hasRenewalRemark : !hasRenewalRemark;
                    }).ToList();
                }

                // ── Apply status filter ───────────────────────────────────────
                if (!string.IsNullOrWhiteSpace(filterStatus) && filterStatus != "all")
                {
                    allPayments = filterStatus switch
                    {
                        "pending" => allPayments.Where(p =>
                            !string.IsNullOrWhiteSpace(p.Remarks) &&
                            p.Remarks.Contains("Pending Admin Approval", StringComparison.OrdinalIgnoreCase)).ToList(),
                        "approved" => allPayments.Where(p =>
                            !string.IsNullOrWhiteSpace(p.Remarks) &&
                            p.Remarks.Contains("Admin Approved", StringComparison.OrdinalIgnoreCase)).ToList(),
                        "rejected" => allPayments.Where(p =>
                            !string.IsNullOrWhiteSpace(p.Remarks) &&
                            p.Remarks.Contains("Admin Rejected", StringComparison.OrdinalIgnoreCase)).ToList(),
                        "failed" => allPayments.Where(p =>
                            string.Equals(p.Status, "Failed", StringComparison.OrdinalIgnoreCase)).ToList(),
                        _ => allPayments
                    };
                }

                // ── Build ViewModel ───────────────────────────────────────────
                var ledgerItems = allPayments.Select(p => new MasterPaymentLedgerItemViewModel
                {
                    Id = p.Id,
                    PaymentNumber = p.PaymentNumber,
                    TransactionId = p.TransactionId,
                    UserId = p.UserId,
                    UserPolicyId = p.UserPolicyId,
                    PolicyId = p.PolicyId,
                    PolicyNumber = p.PolicyNumber,
                    Amount = p.Amount,
                    Status = p.Status,
                    PaymentMethod = p.PaymentMethod,
                    PaymentDate = p.PaymentDate,
                    Remarks = p.Remarks
                }).ToList();

                var model = new MasterPaymentLedgerViewModel
                {
                    TotalPayments = ledgerItems.Count,
                    PendingApprovals = ledgerItems.Count(x => x.IsPendingAdminApproval),
                    ApprovedPayments = ledgerItems.Count(x => x.IsAdminApproved),
                    RenewalPayments = ledgerItems.Count(x => x.PaymentType == "Renewal"),
                    FirstTimePayments = ledgerItems.Count(x => x.PaymentType == "First-Time"),
                    TotalAmount = ledgerItems.Sum(x => x.Amount),
                    Payments = ledgerItems,
                    SearchQuery = searchQuery,
                    FilterType = filterType ?? "all",
                    FilterStatus = filterStatus ?? "all"
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new MasterPaymentLedgerViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApprovePayment(int paymentId)
        {
            try
            {
                await _apiClient.PostAsync<ApiResponse<object>>(
                    $"https://localhost:7003/api/payment/{paymentId}/approve",
                    new { });
            }
            catch
            {
                // log if needed
            }

            return RedirectToAction("MasterPaymentLedger");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectPayment(int paymentId, string reason)
        {
            try
            {
                await _apiClient.PostAsync<ApiResponse<object>>(
                    $"https://localhost:7003/api/payment/{paymentId}/reject",
                    new { Reason = reason });
            }
            catch
            {
                // log if needed
            }

            return RedirectToAction("MasterPaymentLedger");
        }

        [HttpGet]
        public IActionResult Success(PaymentResultViewModel model)
        {
            return View(model);
        }


        [HttpGet]
        public IActionResult Failed(PaymentResultViewModel model)
        {
            return View(model);
        }

        [HttpGet]
        public IActionResult SetMyToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Content("Error: Token cannot be empty.");
            }

            HttpContext.Session.SetString("JWToken", token);
            return Content("Success! Your token is now active on the website session. You can now go to the payment page.");
        }
    }
}