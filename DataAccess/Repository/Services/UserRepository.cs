using AutoMapper;
using BusinessObject.Models;
using BusinessObject.ResourceModel.Common;
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

        public async Task<User> Login(LoginVM model)
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
            return user;
        }

        public async Task<User> Register(RegisterVM model)
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
            };

            var resultCreateUser = await _userManager.CreateAsync(user, model.Password);
            if (!resultCreateUser.Succeeded)
            {
                return null;
            }

            await _userManager.AddToRoleAsync(user, "User");
            return user;
        }

        public async Task<IEnumerable<User>> GetMembers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetMemberByID(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetMemberByEmailAndPass(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Email == email);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                return user;
            }
            return null;
        }

        public async Task InsertMember(User member)
        {
            await _context.Users.AddAsync(member);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMember(User member)
        {
            _context.Users.Update(member);
            await _context.SaveChangesAsync();
        }
    }
}
