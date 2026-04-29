
using InsureTrust.Web.Models;
using InsureTrust.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsureTrust.Web.Controllers
{
    public class RenewalController : Controller
    {
        private readonly IRenewalService _renewalService;

        public RenewalController(IRenewalService renewalService)
        {
            _renewalService = renewalService;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout(int? policyId)
        {
            // Default to policy 1 if none provided (e.g. from navbar link)
            policyId ??= 1;


            var response = await _renewalService.GetPolicyDetailsByIdAsync(policyId.Value);

            if (response == null || !response.Success || response.Data == null)
            {
                return View(new PolicyPaymentViewModel 
                { 
                    PolicyId = policyId.Value,
                    ErrorMessage = response?.Message ?? "Could not fetch policy details." 
                });
            }

            var model = new PolicyPaymentViewModel
            {
                PolicyId = response.Data.PolicyId,
                PaymentCategory = "UPI",
                PaymentMethod = "GPay",
                IsRenewal = false,
                Amount = response.Data.PackageAmount
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
        public async Task<IActionResult> RenewPolicy(string? policyNumber)
        {
            // Default to mock policy number for demo/testing
            policyNumber ??= "POL-12345";


            var response = await _renewalService.GetPolicyDetailsByNumberAsync(policyNumber);

            if (response == null || !response.Success || response.Data == null)
            {
                return View(new PolicyPaymentViewModel 
                { 
                    PolicyNumber = policyNumber,
                    ErrorMessage = response?.Message ?? "Policy not found in Product Service." 
                });
            }

            var model = new PolicyPaymentViewModel
            {
                PolicyNumber = response.Data.PolicyNumber,
                PaymentCategory = "UPI",
                PaymentMethod = "GPay",
                IsRenewal = true,
                Amount = response.Data.PackageAmount
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
                policyNumber = model.PolicyNumber,
                paymentCategory = model.PaymentCategory,
                paymentMethod = model.PaymentMethod,
                isRenewal = true,
                amount = model.Amount
            });
        }

        [HttpGet]
        public IActionResult PaymentVerification(
            int policyId,
            string policyNumber,
            string paymentCategory,
            string paymentMethod,
            bool isRenewal,
            decimal amount = 5000)
        {
            var model = new PolicyPaymentViewModel
            {
                PolicyId = policyId,
                PolicyNumber = policyNumber,
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
                        policyNumber = model.PolicyNumber,
                        paymentMethod = model.PaymentMethod
                    };

                    apiResponse = await _renewalService.InitiateRenewalPaymentAsync(request);
                }
                else
                {
                    var request = new
                    {
                        policyId = model.PolicyId,
                        paymentMethod = model.PaymentMethod
                    };

                    apiResponse = await _renewalService.InitiateFirstPaymentAsync(request);
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

                    PolicyNumber = model.PolicyNumber,

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
                    PolicyNumber = model.PolicyNumber,
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
                var apiResponse = await _renewalService.GetPaymentHistoryAsync();

                if (apiResponse == null || !apiResponse.Success)
                {
                    ViewBag.Error = apiResponse?.Message ?? "Could not fetch payment history from the service.";
                    return View(new RenewalHistoryViewModel());
                }

                var payments = apiResponse.Data ?? new List<PaymentHistoryItemViewModel>();

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
                var apiResponse = await _renewalService.GetPaymentHistoryAsync();

                if (apiResponse == null || !apiResponse.Success)
                {
                    ViewBag.Error = apiResponse?.Message ?? "Could not fetch payment history from the service.";
                    return View(new PaymentHistoryViewModel());
                }

                var payments = apiResponse.Data ?? new List<PaymentHistoryItemViewModel>();

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
                var apiResponse = await _renewalService.GetAllPaymentsAsync();

                if (apiResponse == null || !apiResponse.Success)
                {
                    ViewBag.Error = apiResponse?.Message ?? "Could not fetch master ledger from the service.";
                    return View(new MasterPaymentLedgerViewModel());
                }

                var allPayments = apiResponse.Data ?? new List<PaymentHistoryItemViewModel>();

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    allPayments = allPayments
                        .Where(p => 
                            (!string.IsNullOrEmpty(p.TransactionId) && p.TransactionId.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                            (p.UserPolicyId.HasValue && p.UserPolicyId.Value.ToString() == searchQuery)
                        )
                        .ToList();
                }

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
                await _renewalService.ApprovePaymentAsync(paymentId);
            }
            catch
            {
            }

            return RedirectToAction("MasterPaymentLedger");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectPayment(int paymentId, string reason)
        {
            try
            {
                await _renewalService.RejectPaymentAsync(paymentId, reason);
            }
            catch
            {
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

    }
}