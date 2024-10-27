using BusinessObject.ResourceModel.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace EshopWebApplication.Controllers
{
    public class OrderController : Controller
    {
        private readonly HttpClient client = null;

        public OrderController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            HttpResponseMessage response = await client.GetAsync($"http://localhost:5191/api/Order/getorderbyuserid?uid={id}");
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<OrderVM> listOrders = JsonSerializer.Deserialize<List<OrderVM>>(strData, options) ?? new List<OrderVM>();

            return View(listOrders);
        }
    }
}
