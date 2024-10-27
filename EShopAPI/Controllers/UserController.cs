using AutoMapper;
using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;
using BusinessObject.ResourceModel.ViewModel.User;
using DataAccess.DataAccess;
using DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;

        public UserController(IUserRepository userRepository, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.Login(model);
            if (user == null)
            {
                return Unauthorized("Username or password is incorrect!");
            }

            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.Register(model);
            if (user == null)
            {
                return BadRequest("User has been already existed or error when creating user!");
            }

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var users = await _userRepository.GetUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByID(string id)
        {
            var user = await _userRepository.GetUserByID(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateVM user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedUser = await _userRepository.UpdateUser(user);
            if (updatedUser == null)
            {
                return BadRequest();
            }
            return Ok(updatedUser);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Email is required" });
            }
            using (var _context = new NewProjectDBContext())
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return NotFound(new { message = "This email is not registered" });
                }
                string newPassword = GenerateRandomPassword(10);
                var removePassword = await _userManager.RemovePasswordAsync(user);
                if (removePassword.Succeeded)
                {
                    var result = await _userManager.AddPasswordAsync(user, newPassword);
                    if (result.Succeeded)
                    {
                        var emailBody = $@"
                        <h2>Password Reset</h2>
                        <p>Your new password is: <strong>{newPassword}</strong></p>
                        <p>Please change your password after logging in for security reasons.</p>";

                        SendMailSMTP.SendMail(user.Email, "Password Reset", emailBody);

                        return Ok(new { message = "A new password has been sent to your email" });
                    }
                    else
                    {
                        return StatusCode(500, new { message = "Failed to set new password", errors = result.Errors });
                    }
                }
            }
            return Ok(new { message = "A new password has been sent to your email" });
        }

        private string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                return BadRequest(new { message = "Confirmation password do not match" });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var emailBody = $@"
                <h2>Password Changed Successfully</h2>
                <p>Your password has been changed successfully.</p>
                <p>If you did not make this change, please contact support immediately.</p>";

                SendMailSMTP.SendMail(user.Email, "Password Change Confirmation", emailBody);

                return Ok(new { message = "Password changed successfully" });
            }
            else
            {
                return BadRequest(new { message = "Failed to change password", errors = result.Errors });
            }

        }
    }
}
