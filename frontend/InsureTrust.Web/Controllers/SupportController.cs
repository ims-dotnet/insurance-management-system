using System.Net.Http.Headers;
using InsureTrust.Web.Models;
using InsureTrust.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsureTrust.Web.Controllers
{
    public class SupportController : Controller
    {
        private readonly ISupportService _supportService;

        public SupportController(ISupportService supportService)
        {
            _supportService = supportService;
        }

        [HttpGet]
        public IActionResult CreateQuery()
        {
            return View(new CreateSupportQueryViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuery(CreateSupportQueryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using var content = new MultipartFormDataContent
                {
                    { new StringContent(model.Subject), "Subject" },
                    { new StringContent(model.Description), "Description" }
                };

                if (model.Attachment != null)
                {
                    var streamContent = new StreamContent(model.Attachment.OpenReadStream());

                    if (!string.IsNullOrWhiteSpace(model.Attachment.ContentType))
                    {
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(model.Attachment.ContentType);
                    }

                    content.Add(streamContent, "Attachment", model.Attachment.FileName);
                }

                var response = await _supportService.SubmitSupportQueryAsync(content);

                if (response == null || !response.Success)
                {
                    model.ErrorMessage = response?.Message ?? "Failed to create support ticket.";
                    return View(model);
                }

                var createdTicket = response.Data;

                return View(new CreateSupportQueryViewModel
                {
                    SuccessMessage = "Support ticket submitted successfully.",
                    CreatedTicket = createdTicket
                });
            }
            catch (Exception ex)
            {
                model.ErrorMessage = ex.Message;
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> MyQueries()
        {
            var viewModel = new SupportQueryListViewModel();

            try
            {
                var response = await _supportService.GetMyQueriesAsync();

                if (response == null || !response.Success)
                {
                    viewModel.ErrorMessage = response?.Message ?? "Could not fetch queries from the service.";
                }
                else
                {
                    viewModel.Queries = response.Data ?? new List<SupportQueryViewModel>();
                }
            }
            catch (Exception ex)
            {
                viewModel.ErrorMessage = ex.Message;
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ManageQueries()
        {
            var viewModel = new SupportQueryListViewModel();

            try
            {
                var response = await _supportService.GetAllQueriesAsync();

                if (response == null || !response.Success)
                {
                    viewModel.ErrorMessage = response?.Message ?? "Could not fetch queries from the service.";
                }
                else
                {
                    viewModel.Queries = response.Data ?? new List<SupportQueryViewModel>();
                }
            }
            catch (Exception ex)
            {
                viewModel.ErrorMessage = ex.Message;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(UpdateSupportStatusViewModel model)
        {
            try
            {
                var response = await _supportService.UpdateQueryStatusAsync(model.Id, model);

                if (response == null || !response.Success)
                {
                    TempData["Error"] = response?.Message ?? "Failed to update support ticket status.";
                }
                else
                {
                    TempData["Success"] = response.Message;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(ManageQueries));
        }
    }
}