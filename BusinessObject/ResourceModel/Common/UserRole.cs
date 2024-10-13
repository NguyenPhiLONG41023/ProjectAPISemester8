using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.ResourceModel.Common
{
    public class UserRole
    {
        public const string Admin = "Admin";
        public const string User = "User";

        public static readonly List<string> Roles = new List<string> { Admin, User };
    }
}
