using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IdentityExample.ViewModel
{
    public class RegisterVM
    {
        [Required]
        [DataType(DataType.Text)]
        public string username { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Password and cofirm password do not match.")]
        public string confirm_password { get; set; }
    }
}
