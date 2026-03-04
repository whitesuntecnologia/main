using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class UserProfile: IdentityUser
    {
        public int Estado { get; set; }
        public string? NombreyApellido { get; set; }
    }
}
