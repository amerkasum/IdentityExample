using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModel
{
    public class ManageUserRolesVM
    {
        public ManageUserRolesVM()
        {
            roles = new List<string>();
        }
        public IdentityUser user { get; set; }
        public IList<string > roles { get; set; }
    }
}
