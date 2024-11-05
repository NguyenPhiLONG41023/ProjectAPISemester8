using BusinessObject.Models;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json;
using AutoMapper;
using EshopWebApplication.Models;
using DataAccess.Repository.Interfaces;
using DataAccess.Repository.Services;
using BusinessObject.ResourceModel.ViewModel;
using System.Security.Claims;
using DataAccess.DataAccess;
using Microsoft.AspNetCore.Authorization;



namespace EshopWebApplication.Controllers
{
    public class CartController : Controller
    {
        public Cart? Cart { get; set; }

        IProductRepository productRepository;
        IOrderRepository orderRepository;
        IOrderDetailRepository orderDetailRepository;

        public CartController(ProductRepository _productRepository, OrderRepository _orderRepository
            , OrderDetailRepository _orderDetailRepository)
        {
            productRepository = _productRepository;
            orderRepository = _orderRepository;
            orderDetailRepository = _orderDetailRepository;
        }
        public IActionResult Index()
        {
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            decimal total = Cart.ComputeTotalValue();
            ViewBag.Total = total;
            return View(Cart);
        }

        public IActionResult AddToCart(Guid productId, int? ammount)
        {
            ProductVM vm = productRepository.GetProductByID(productId);

            if (vm != null)
            {
                Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
                if (ammount.HasValue)
                {
                    if (ammount > vm.Quantity)
                    {
                        return RedirectToAction("Index", "Product");
                    }
                    else
                    {
                        Cart.AddItem(vm, ammount);
                    }
                }
                else
                {
                    Cart.AddItem(vm, 1);  //nếu không có ammount truyền vào thì input 1
                }

                HttpContext.Session.SetJson("cart", Cart);
            }
            return RedirectToAction("Index", "Product");

        }

        public IActionResult RemoveFromCart(Guid productId)
        {
            ProductVM product = productRepository.GetProductByID(productId);
            if (product != null)
            {
                Cart = HttpContext.Session.GetJson<Cart>("cart");
                Cart.RemoveLine(product);
                HttpContext.Session.SetJson("cart", Cart);
            }
            return RedirectToAction("Index", "Cart");
        }

        [Authorize]
        public IActionResult CheckOut()
        {
            using var context = new NewProjectDBContext();
            Cart = HttpContext.Session.GetJson<Cart>("cart");
            if (Cart != null && Cart.Lines.Count > 0)
            {
                OrderUpdateVM newOrder = new OrderUpdateVM()
                {
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    OrderDate = DateTime.Now,
                    TotalPrice = Cart.ComputeTotalValue(),
                    Status = "0",
                };

                orderRepository.AddOrder(newOrder);
                var newestOrder = context.Orders.OrderByDescending(x => x.OrderDate).First();
                foreach (var cartLine in Cart.Lines)
                {
                    OrderDetail orderDetail = new OrderDetail
                    {
                        OrderId = newestOrder.OrderId,
                        ProductId = cartLine.Product.ProductId,
                        Price = cartLine.Product.Price,
                        Quantity = cartLine.Quantity,
                    };

                    context.OrderDetails.Add(orderDetail);
                    Product pro = context.Products.Find(cartLine.Product.ProductId);
                    if (pro != null)
                    {
                        pro.Quantity -= cartLine.Quantity;
                    }
                    context.SaveChanges();
                }
                Cart.Clear();
                HttpContext.Session.SetJson("cart", Cart);

                return RedirectToAction("Index", "Product");
            }
            else
            {
                return RedirectToAction("Index", "Cart");
            }
        }
    }

    public static class SessionExtensions
    {
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
