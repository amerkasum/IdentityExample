using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModel
{
    public class EditUserVM
    {
        public EditUserVM()
        {
            roles = new List<string>();
            claims = new List<string>();
        }
        public string user_id { get; set; }
        [Required]
        public string username { get; set; }
        [EmailAddress][Required]
        public string email { get; set; }
        public string phone_number { get; set; }
        public IList<string> roles { get; set; }
        public IList<string> claims { get; set; }

    }
}
