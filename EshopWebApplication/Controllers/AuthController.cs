﻿using AutoMapper.Execution;
using BusinessObject.ResourceModel.ViewModel.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BusinessObject.ResourceModel.ViewModel;

namespace EshopWebApplication.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient client = null;
        public AuthController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
			HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string pass)
        {
            var loginData = new
            {
                userName = username,
                password = pass
            };

            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("http://localhost:5191/api/User/login", content);

            if (response.IsSuccessStatusCode)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                UserVM user = System.Text.Json.JsonSerializer.Deserialize<UserVM>(strData, options);

                if (user != null)
                {
					var claims = new List<Claim>
			        {
				        new Claim(ClaimTypes.Name, username),
				        new Claim(ClaimTypes.Role, user.RoleName),
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                    };
					var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

					await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

					return RedirectToAction("Index", "Home");
				}
            }
            ViewBag.ErrorMessage = "Wrong email or password";
            return View();
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string username, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match!";
                return View();
            }

            var registerData = new
            {
                email = email,
                username = username,
                password = password,
                confirmPassword = confirmPassword
            };

            var json = JsonConvert.SerializeObject(registerData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("http://localhost:5191/api/User/register", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                ViewBag.ErrorMessage = "Registration failed. Please try again.";
            }

            return View();
        }

        public async Task<IActionResult> EditProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string ProductApiUrlDetail = $"http://localhost:5191/api/User/{userId}";
            HttpResponseMessage response = await client.GetAsync(ProductApiUrlDetail);

            if (response.IsSuccessStatusCode)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                UserVM product = System.Text.Json.JsonSerializer.Deserialize<UserVM>(strData, options);
                return View(product);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UserUpdateVM product)
        {
            var data = System.Text.Json.JsonSerializer.Serialize(product);
            var httpContent = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync("http://localhost:5191/api/User/update", httpContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
