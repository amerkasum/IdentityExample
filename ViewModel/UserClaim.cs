using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModel
{
    public class UserClaim
    {
        public UserClaim() { }
        public UserClaim(string x)
        {

        }
        public string claim_type { get; set; }
        public bool isInClaim { get; set; }
    }
}
