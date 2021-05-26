using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModel
{
    public class ManageUserClaimsVM
    {
        public ManageUserClaimsVM()
        {
            claims = new List<UserClaim>();
        }
        public string user_id { get; set; }
        public IList<UserClaim> claims { get; set; }

    }
}
