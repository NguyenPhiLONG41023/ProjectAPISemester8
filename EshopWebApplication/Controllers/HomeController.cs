using BusinessObject.ResourceModel.ViewModel;
using EshopWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EshopWebApplication.Controllers
{
	public class HomeController : Controller
	{
		private readonly HttpClient client = null;
		private string ProductApiUrl = "";
		public HomeController()
		{
			client = new HttpClient();
			var contentType = new MediaTypeWithQualityHeaderValue("application/json");
			client.DefaultRequestHeaders.Accept.Add(contentType);
			ProductApiUrl = "http://localhost:5191/api/Product";
		}

		// GET: Products
		public async Task<IActionResult> Index()
		{
			//if (HttpContext.Session.GetString("email") != "admin@estore.com")
			//{
			//    return Redirect("Login");
			//}
			//ViewBag.Email = HttpContext.Session.GetString("email");
			HttpResponseMessage response = await client.GetAsync("http://localhost:5191/api/Product?$filter=status%20eq%201&$orderby=CreatedDate%20desc&$top=5");
			string strData = await response.Content.ReadAsStringAsync();

			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			List<ProductVM> listProducts = JsonSerializer.Deserialize<List<ProductVM>>(strData, options);
			return View(listProducts);
		}
	}


}