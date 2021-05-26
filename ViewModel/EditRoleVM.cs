using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.ViewModel
{
    public class EditRoleVM
    {
        public EditRoleVM()
        {
            users = new List<string>();
        }
        public string id { get; set; }
        [Required(ErrorMessage = "Role Name is required")]
        public string role_name { get; set; }
        public List<string> users { get; set; }
    }
}
