using BusinessObject.ResourceModel.Common;
using DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Services
{
    public class RoleDataServices : IDataService
    {
        //sử dụng IServiceProvider để yêu cầu DI Container cung cấp đối tượng RoleManager
        private readonly IServiceProvider _app;
        public RoleDataServices(IServiceProvider app)
        {
            _app = app;
        }
        public async Task AddData()
        {
            var scope = _app.CreateScope();
            //láy dịch vụ RoleManager<IdentityRole> của Identity để quản lý các vai trò
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var listRoles = UserRole.Roles;  //lấy danh sách gồm Admin và User
            foreach (var role in listRoles)
            {
                //check xem trong hệ thống đã tồn tại role này chưa
                if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                {
                    //nếu chưa tồn tại thì add
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
