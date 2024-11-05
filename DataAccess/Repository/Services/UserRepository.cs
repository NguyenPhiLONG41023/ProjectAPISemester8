using AutoMapper;
using AutoMapper.Execution;
using BusinessObject.Models;
using BusinessObject.ResourceModel.Common;
using BusinessObject.ResourceModel.ViewModel;
using BusinessObject.ResourceModel.ViewModel.User;
using DataAccess.DataAccess;
using DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Services
{
    public class UserRepository : IUserRepository
    {
        private UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public readonly SignInManager<User> _signInManager;
        public readonly AdminAccount _adminAccount;
        public readonly NewProjectDBContext _context;
        public readonly IMapper _mapper;

        public UserRepository(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager
            , IOptionsMonitor<AdminAccount> adminAccount, NewProjectDBContext context, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _adminAccount = adminAccount.CurrentValue;
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserVM> Login(LoginVM model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                if (model.UserName == _adminAccount.UserName)
                {
                    var adminAccount = await _userManager.FindByNameAsync(_adminAccount.UserName);
                    if (adminAccount == null)
                    {
                        var admin = new User()
                        {
                            Email = model.UserName,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            UserName = model.UserName,
                        };
                        var result = await _userManager.CreateAsync(admin, _adminAccount.Password);
                        var roleResult = await _userManager.AddToRoleAsync(admin, "Admin");
                    }
                }
                else
                {
                    return null;
                }
            }
            return await GetUserByID(user.Id);
        }

        public async Task<UserVM> Register(RegisterVM model)
        {
            var userExistMail = await _userManager.FindByEmailAsync(model.Email);
            var userExistName = await _userManager.FindByNameAsync(model.Username);
            if (userExistMail != null || userExistName != null)
            {
                return null;
            }

            var user = new User()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Status = 1
            };

            var resultCreateUser = await _userManager.CreateAsync(user, model.Password);
            if (!resultCreateUser.Succeeded)
            {
                return null;
            }

            await _userManager.AddToRoleAsync(user, "User");
            return await GetUserByID(user.Id);
        }

        public async Task<IEnumerable<UserVM>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userVMs = new List<UserVM>();

            foreach (var user in users)
            {
                var userVM = _mapper.Map<UserVM>(user);
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    userVM.RoleName = roles.First(); // Giả sử mỗi user chỉ có một vai trò
                    var role = await _roleManager.FindByNameAsync(userVM.RoleName);
                    userVM.RoleId = role?.Id;
                }
                userVMs.Add(userVM);
            }

            return userVMs;
        }

        public async Task<UserVM> GetUserByID(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return null;

            var userVM = _mapper.Map<UserVM>(user);
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            {
                userVM.RoleName = roles.First(); // Giả sử mỗi user chỉ có một vai trò
                var role = await _roleManager.FindByNameAsync(userVM.RoleName);
                userVM.RoleId = role?.Id;
            }

            return userVM;
        }

        public async Task<UserVM> UpdateUser(UserUpdateVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return null;

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Status = model.Status;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return null;

            var currentRoles = await _userManager.GetRolesAsync(user);
            var newRole = await _roleManager.FindByIdAsync(model.RoleId);
            if (newRole != null && !currentRoles.Contains(newRole.Name))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, newRole.Name);
            }

            return await GetUserByID(user.Id);
        }
    }
}
