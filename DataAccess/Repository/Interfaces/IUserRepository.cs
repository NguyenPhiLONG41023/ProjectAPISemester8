using AutoMapper.Execution;
using BusinessObject.Models;
using BusinessObject.ResourceModel.Common;
using BusinessObject.ResourceModel.ViewModel.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<UserVM> Login(LoginVM model);
        Task<UserVM> Register(RegisterVM model);
        Task<IEnumerable<UserVM>> GetUsers();
        Task<UserVM> GetUserByID(string id);
        Task<UserVM> UpdateUser(UserUpdateVM model);
    }
}
