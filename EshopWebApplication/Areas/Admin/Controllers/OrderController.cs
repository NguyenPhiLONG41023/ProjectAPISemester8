using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;
using DataAccess.DataAccess;
using EShopAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EshopWebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Policy = "RequireAdminRole")]
	public class OrderController : Controller
    {
        private readonly HttpClient client = null;
        private string MemberApiUrl = "";
        public OrderController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            MemberApiUrl = "http://localhost:5191/api/Order";
        }
        public async Task<IActionResult> Index(string? startDate, string? endDate)
        {
            //if (HttpContext.Session.GetString("email") != "admin@estore.com")
            //{
            //    return Redirect("Login");
            //}
            //ViewBag.Email = HttpContext.Session.GetString("email");
            string url = "http://localhost:5191/api/Order/searchorderbydate";
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                url += $"?start={startDate}&end={endDate}";
            }

            HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                //Lấy message khi api trả về BadRequest
                string errorMessage = await response.Content.ReadAsStringAsync();
                ViewBag.Error = errorMessage;
                return View(new List<OrderVM>());
            }

            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            List<OrderVM> listOrders = JsonSerializer.Deserialize<List<OrderVM>>(strData, options) ?? new List<OrderVM>();
            return View(listOrders);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            string url = $"http://localhost:5191/api/OrderDetail/GetOrderDetailsListByOrderID?id={id}";
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                List<OrderDetailVM> orderDetail = JsonSerializer.Deserialize<List<OrderDetailVM>>(strData, options);
                return View(orderDetail);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid OrderId, OrderUpdateVM order)
        {
            string MemberApiUrlDetail = $"{MemberApiUrl}/{OrderId}";
            //chuyển đối tượng ProductVM sang dạng JSON
            var data = JsonSerializer.Serialize(order);
            //tạo nội dung http với định dạng JSON và mã hóa UTF8
            var httpContent = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(MemberApiUrlDetail, httpContent);

            using (NewProjectDBContext context = new NewProjectDBContext())
            {
                var user = context.Users.FirstOrDefault(x => x.Id == order.UserId);

                if (user != null && response.IsSuccessStatusCode)
                {
                    string mess = null;
                    if (order.Status == "0") mess = "Pending";
                    if (order.Status == "1") mess = "Shipping";
                    if (order.Status == "2") mess = "Done";

                    var emailBody = $@"
                    <h2>Order Status Update</h2>
                    <p>Your order status has been updated to: {mess}</p>
                    <p>If you have any questions, please contact support.</p>
                    ";

                    SendMailSMTP.SendMail(user.Email, "Order Status Update", emailBody);

                    TempData["Message"] = "Edit success";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return NotFound();
                }
            }
        }
    }
}
