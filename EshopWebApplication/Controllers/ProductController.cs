using BusinessObject.ResourceModel.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EshopWebApplication.Controllers
{
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

		// GET: Products
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

        public async Task<IActionResult> Search(Guid? brandId, string? search)
        {
            string ProductApiUrlSearch = $"{ProductApiUrl}?$filter="; 
            List<string> filters = new List<string>();

            if (brandId.HasValue)
            {
                filters.Add($"brandId eq {brandId}");
            }

            if (!string.IsNullOrEmpty(search))
            {
                //filters.Add($"contains(productName, '{search}')");
                filters.Add($"contains(tolower(productName), '{search.ToLower()}')");
            }

            if (filters.Count > 0)
            {
                ProductApiUrlSearch += string.Join(" and ", filters);
            }
            else
            {
                ProductApiUrlSearch += "true";
            }

            HttpResponseMessage response = await client.GetAsync(ProductApiUrlSearch);
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<ProductVM> listProducts = JsonSerializer.Deserialize<List<ProductVM>>(strData, options);

            return View("Index", listProducts);
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid? id)
		{
			string ProductApiUrlDetail = $"{ProductApiUrl}/{id}";
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
	}
}
