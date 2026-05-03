using InsureTrust.Web.Models;
using InsureTrust.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsureTrust.Web.Controllers
{
    public class ClaimController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly IPolicyService _policyService;

        public ClaimController(IClaimService claimService, IPolicyService policyService)
        {
            _claimService = claimService;
            _policyService = policyService;
        }

        public async Task<IActionResult> Index()
        {
            var claims = await _claimService.GetMyClaimsAsync();
            var policies = await _policyService.GetAllPolicybyid();
            var activePolicies = policies.Where(p => p.Status.Equals("active", StringComparison.OrdinalIgnoreCase) || p.Status.Equals("approved", StringComparison.OrdinalIgnoreCase)).ToList();

            var model = new ClaimIndexViewModel
            {
                Claims = claims,
                ActivePolicies = activePolicies
            };

            return View(model);
        }

        public async Task<IActionResult> SubmitClaim(int policyId = 0)
        {
            var policies = await _policyService.GetAllPolicybyid();
            var activePolicies = policies.Where(p => p.Status.Equals("active", StringComparison.OrdinalIgnoreCase) || p.Status.Equals("approved", StringComparison.OrdinalIgnoreCase)).ToList();
            ViewBag.ActivePolicies = activePolicies;

            var model = new SubmitClaimViewModel { PolicyId = policyId };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitClaim(SubmitClaimViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var policies = await _policyService.GetAllPolicybyid();
                ViewBag.ActivePolicies = policies.Where(p => p.Status.Equals("active", StringComparison.OrdinalIgnoreCase) || p.Status.Equals("approved", StringComparison.OrdinalIgnoreCase)).ToList();
                return View(model);
            }

            var success = await _claimService.SubmitClaimAsync(model.PolicyId, model);

            if (success)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error submitting claim. Please try again.");
            
            var allPolicies = await _policyService.GetAllPolicybyid();
            ViewBag.ActivePolicies = allPolicies.Where(p => p.Status.Equals("active", StringComparison.OrdinalIgnoreCase) || p.Status.Equals("approved", StringComparison.OrdinalIgnoreCase)).ToList();
            
            return View(model);
        }
    }
}
