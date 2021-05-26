using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModel
{
    public class CreateRoleVM
    {
        [Required]
        public string role_name { get; set; }
    }
}
