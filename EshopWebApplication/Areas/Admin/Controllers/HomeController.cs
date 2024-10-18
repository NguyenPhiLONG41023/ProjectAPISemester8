using BusinessObject.ResourceModel.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using BusinessObject.ResourceModel.ViewModel.User;
using Microsoft.AspNetCore.Authorization;

namespace EshopWebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminRole")]
    public class HomeController : Controller
    {
        private readonly HttpClient client = null;
        private string UserApiUrl = "";
        public HomeController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            UserApiUrl = "http://localhost:5191/api/User";
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            //if (HttpContext.Session.GetString("email") != "admin@estore.com")
            //{
            //    return Redirect("Login");
            //}
            //ViewBag.Email = HttpContext.Session.GetString("email");
            HttpResponseMessage response = await client.GetAsync(UserApiUrl);
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            List<UserVM> listProducts = JsonSerializer.Deserialize<List<UserVM>>(strData, options);
            return View(listProducts);
        }

        // GET: Products/Edit/5
        //public async Task<IActionResult> Edit(Guid? ProductId)
        //{
        //    string ProductApiUrlDetail = $"{UserApiUrl}/{ProductId}";
        //    HttpResponseMessage response = await client.GetAsync(ProductApiUrlDetail);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        string strData = await response.Content.ReadAsStringAsync();
        //        var options = new JsonSerializerOptions
        //        {
        //            PropertyNameCaseInsensitive = true
        //        };
        //        UserVM product = JsonSerializer.Deserialize<UserVM>(strData, options);
        //        return View(product);
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserUpdateVM product)
        {
            var data = JsonSerializer.Serialize(product);
            var httpContent = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync("http://localhost:5191/api/User/update", httpContent);

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
