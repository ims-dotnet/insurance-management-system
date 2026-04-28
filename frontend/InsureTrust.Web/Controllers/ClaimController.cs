using InsureTrust.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace InsureTrust.Web.Controllers
{
    public class ClaimController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClaimController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("http://localhost:7004/api/claim/my-claims"); // API Gateway or Service URL
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (apiResponse.TryGetProperty("data", out var dataProperty))
                {
                    var claims = JsonSerializer.Deserialize<List<ClaimViewModel>>(dataProperty.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(claims);
                }
            }
            
            return View(new List<ClaimViewModel>());
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

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.Description), "Description");
            content.Add(new StringContent(model.MaturityAmount.ToString()), "MaturityAmount");

            if (model.Documents != null)
            {
                foreach (var file in model.Documents)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.Add("Content-Type", file.ContentType);
                    content.Add(fileContent, "Documents", file.FileName);
                }
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync($"http://localhost:7004/api/claim/submit/{model.PolicyId}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error submitting claim. Please try again.");
            return View(model);
        }
    }
}
