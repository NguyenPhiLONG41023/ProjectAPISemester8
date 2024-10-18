using BusinessObject.ResourceModel.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EshopWebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Policy = "RequireAdminRole")]
	public class ProductController : Controller
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        public ProductController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "http://localhost:5191/api/Product";
        }

        public async Task<IActionResult> Index()
        {
            HttpResponseMessage response = await client.GetAsync(ProductApiUrl);
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            List<ProductVM> listProducts = JsonSerializer.Deserialize<List<ProductVM>>(strData, options);
            return View(listProducts);
        }

        public async Task<IActionResult> Edit(Guid? ProductId)
        {
            string ProductApiUrlDetail = $"{ProductApiUrl}/{ProductId}";
            HttpResponseMessage response = await client.GetAsync(ProductApiUrlDetail);

            if (response.IsSuccessStatusCode)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                ProductVM product = JsonSerializer.Deserialize<ProductVM>(strData, options);
                return View(product);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid ProductId, ProductAddVM product)
        {
            string ProductApiUrlDetail = $"{ProductApiUrl}/{ProductId}";
            var data = JsonSerializer.Serialize(product);
            var httpContent = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(ProductApiUrlDetail, httpContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductAddVM product)
        {
            var data = JsonSerializer.Serialize(product);
            var httpContent = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(ProductApiUrl, httpContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file)
        {
            using (var formData = new MultipartFormDataContent())
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                formData.Add(fileContent, "fileImport", file.FileName);

                HttpResponseMessage response = await client.PostAsync("http://localhost:5191/api/Product/import", formData);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
			string ProductApiUrlDetail = $"{ProductApiUrl}/{id}";
			HttpResponseMessage response = await client.DeleteAsync(ProductApiUrlDetail);

			if (response.IsSuccessStatusCode)
			{
				return RedirectToAction(nameof(Index));
			}
			else
			{
				return NotFound();
			}
		}
    }
}
