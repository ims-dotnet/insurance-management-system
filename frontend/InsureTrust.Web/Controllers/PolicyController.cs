
using InsureTrust.Productweb.DTOs;
using InsureTrust.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace InsureTrust.Web.Controllers
{
    //[Authorize]
    public class PolicyController : Controller
    {
        private readonly IPolicyService _service;

        public PolicyController(IPolicyService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        public IActionResult SetToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return Content("Token missing");

            HttpContext.Session.SetString("JWT", token);

            return Content("Token Saved Successfully");
        }

        [AllowAnonymous]
        public IActionResult CheckToken()
        {
            var token = HttpContext.Session.GetString("JWT");
            return Content(token ?? "NO TOKEN");
        }

        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllPolicyTypeAsync();
            return View(data);
        }

        public async Task<IActionResult> PolicyTypeById(int id)
        {
            var data = await _service.GetPolicyTypeByIdAsync(id);

            if (data == null)
                return NotFound();

            return View(data);
        }

        public IActionResult Purchase(int id)
        {
            return View(new CreatePolicyDto
            {
                PolicyTypeId = id
            });
        }

        [HttpPost]
        public async Task<IActionResult> Purchase(CreatePolicyDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _service.PurchaseAsync(dto);

            if (result)
            {
                TempData["Success"] = "Policy purchased successfully";
                return RedirectToAction("GetAllPolicybyid");
            }

            TempData["Error"] = "Policy purchase failed";
            return View(dto);
        }

        public async Task<IActionResult> GetAllPolicybyid()
        {
            var data = await _service.GetAllPolicybyid();
            return View(data ?? new List<PolicyDto>());
        }

        public async Task<IActionResult> GetAllPolicy()
        {
            var data = await _service.GetAllPolicy();
            return View(data ?? new List<PolicyDto>());
        }

        public async Task<IActionResult> GetAllPending()
        {
            var data = await _service.GetAllPending();
            return View(data ?? new List<PolicyDto>());
        }

        public IActionResult CreatePolicy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePolicy(CreatePolicyTypeDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _service.CreateAsync(dto);

            if (result)
            {
                TempData["Success"] = "Policy type created successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Policy type creation failed";
            return View(dto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = await _service.GetPolicyTypeByIdAsync(id);

            if (data == null)
                return NotFound();

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _service.Delete(id);

            if (result)
                TempData["Success"] = "Deleted successfully";
            else
                TempData["Error"] = "Delete failed";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> update(int id)
        {
            var data = await _service.GetPolicyTypeByIdAsync(id);

            if (data == null)
                return NotFound();

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> update(PolicyTypeDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _service.UpdatePolicyTypeAsync(dto.Id, dto);

            if (result)
            {
                TempData["Success"] = "Updated successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Update failed";
            return View(dto);
        }

        public async Task<IActionResult> ManageTypes()
        {
            var data = await _service.GetAllPolicyTypeAsync();
            return View(data);
        }

        public IActionResult EditPolicy(int id)
        {
            ViewBag.PolicyId = id;
            return View(new CreatePolicyDto());
        }

        [HttpPost]
        public async Task<IActionResult> EditPolicy(CreatePolicyDto dto, int policyId)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PolicyId = policyId;
                return View(dto);
            }

            var result = await _service.EditPolicy(dto, policyId);

            if (result)
            {
                TempData["Success"] = "Policy updated successfully";
                return RedirectToAction(nameof(GetAllPolicybyid));
            }

            TempData["Error"] = "Policy update failed";
            ViewBag.PolicyId = policyId;
            return View(dto);
        }

        public async Task<IActionResult> Renew(int id)
        {
            var data = await _service.GetAllPolicybyid();
            var policy = data.FirstOrDefault(x => x.Id == id);

            if (policy == null)
                return NotFound();

            return View(policy);
        }

        [HttpPost]
        public async Task<IActionResult> RenewConfirm(int policyId)
        {
            var result = await _service.RenewPolicyAsync(policyId, 0);

            if (result)
                TempData["Success"] = "Policy renewed successfully";
            else
                TempData["Error"] = "Policy renew failed";

            return RedirectToAction(nameof(GetAllPolicybyid));
        }
    }
}

