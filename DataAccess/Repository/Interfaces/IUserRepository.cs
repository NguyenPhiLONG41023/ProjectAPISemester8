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
        Task<User> Login(LoginVM model);
        Task<User> Register(RegisterVM model);
        Task<IEnumerable<User>> GetMembers();
        Task<User> GetMemberByID(string id);
        Task<User> GetMemberByEmailAndPass(string email, string password);
        Task InsertMember(User member);
        Task UpdateMember(User member);
    }
}
