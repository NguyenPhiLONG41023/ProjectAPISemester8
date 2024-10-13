using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class User : IdentityUser
    {
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
