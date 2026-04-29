using InsureTrust.Web.Models;
using InsureTrust.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsureTrust.Web.Controllers
{
    public class ClaimController : Controller
    {
        private readonly IClaimService _claimService;

        public ClaimController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        public async Task<IActionResult> Index()
        {
            var claims = await _claimService.GetMyClaimsAsync();
            return View(claims);
        }

        public IActionResult SubmitClaim(int policyId)
        {
            var model = new SubmitClaimViewModel { PolicyId = policyId };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitClaim(SubmitClaimViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _claimService.SubmitClaimAsync(model.PolicyId, model);

            if (success)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error submitting claim. Please try again.");
            return View(model);
        }
    }
}
