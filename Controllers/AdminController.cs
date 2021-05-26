using IdentityExample.Models;
using IdentityExample.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Policy = "CreateRolePolicy")]
        public async Task<IActionResult> CreateRole(CreateRoleVM model)
        {
            if(ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.role_name
                };

                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if(result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Admin");
                }

                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);   
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpPost]
        [Authorize(Policy ="DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Admin");
                }
                else 
                    return View("Error", "Admin");
            }
            else
                ModelState.AddModelError(string.Empty, "No role found");
            return RedirectToAction("ListRoles", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if(role == null)
            {
                return View("Error", "Admin");
            }

            var model = new EditRoleVM
            {
                role_name = role.Name,
                id = role.Id
            };

            foreach(var i in _userManager.Users)
            {
                if(await _userManager.IsInRoleAsync(i , role.Name)) {
                    model.users.Add(i.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(EditRoleVM model)
        {
            var role = await _roleManager.FindByIdAsync(model.id);

            if(role == null)
            {
                return View("Error", "Admin");
            }
            else
            {
                role.Name = model.role_name;

                var result = await _roleManager.UpdateAsync(role);
                if(result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach(var i in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, i.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddUserInRole(string role_id)
        {
            ViewBag.role_id = role_id;
            var role = await _roleManager.FindByIdAsync(role_id);
            ViewBag.role_name = role.Name;

            if(role == null)
            {
                ViewBag.ErrorMessage = "Role not found!";
                return View("Error", "Admin");
            }

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> AddUserInRole(UserRoleVM model, string role_id)
        {
            var role = await _roleManager.FindByIdAsync(role_id);
            if (role == null)
            {
                ViewBag.ErrorMessage = "Role not found!";
                return View("Error", "Admin");
            }

            var user = await _userManager.FindByNameAsync(model.username);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User not found";
                return View("Error", "Admin");
            }

            if (await _userManager.IsInRoleAsync(user, role.Name))
            {
                ViewBag.ErrorMessage = "That user is already in role!";
                return View("Error", "Admin");
            }

            IdentityResult result = null;
            result = await _userManager.AddToRoleAsync(user, role.Name);

            return RedirectToAction("EditRole", new { id = role_id });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUserFromRole(string username, string role_id)
        {
            var role = await _roleManager.FindByIdAsync(role_id);
            if (role == null)
            {
                ViewBag.ErrorMessage = "Role not found!";
                return View("Error", "Admin");
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User not found";
                return View("Error", "Admin");
            }

            IdentityResult result = null;
            result = await _userManager.RemoveFromRoleAsync(user, role.Name);

            return RedirectToAction("EditRole", new { id = role_id });
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var model = _userManager.Users;
            return View(model);
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers", "Admin");
                }
                else
                {
                    ViewBag.ErrorMessage = "There is an error!";
                    return View("Error", "Admin");
                }
            }
            else
                ModelState.AddModelError(string.Empty, "No user found!");
            return View("ListUsers", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var userClaims = await _userManager.GetClaimsAsync(user);
                EditUserVM model = new EditUserVM
                {
                    user_id = user.Id,
                    username = user.UserName,
                    email = user.Email,
                    phone_number = user.PhoneNumber,
                    roles = userRoles,
                    claims = userClaims.Select(s => s.Value).ToList()
                };
                return View(model);
            }

            ViewBag.ErrorMessage = $"User with ID = {id} do not exist.";
            return View("Error", "Admin");
            
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserVM model)
        {
            var user = await _userManager.FindByIdAsync(model.user_id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with ID = {model.user_id} do not exist.";
                return View("Error", "Admin");
            }
            else
            {
                user.UserName = model.username;
                user.Email = model.email;
                user.PhoneNumber = model.phone_number;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers", "Admin");
                }

                foreach (var i in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, i.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(RegisterVM model)
        {
            IdentityUser newUser = new IdentityUser
            {
                UserName = model.username,
                Email = model.email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newUser, model.password);
            await _userManager.AddToRoleAsync(newUser, "User");
            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers", "Admin");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with ID = {id} do not exist.";
                return View("Error", "Admin");
            }

            ManageUserRolesVM model = new ManageUserRolesVM
            {
                user = new IdentityUser
                {
                    UserName = user.UserName,
                    Id = user.Id
                },
                roles = _roleManager.Roles.Select(s => s.Name).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(string username, string role_name)
        {
            var user = await _userManager.FindByNameAsync(username);
            var role = await _roleManager.FindByNameAsync(role_name);

            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with ID = {username} do not exist.";
                return View("Error", "Admin");
            }
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Name = {role_name} do not exist.";
                return View("Error", "Admin");
            }

            IdentityResult result = null;
            if (await _userManager.IsInRoleAsync(user, role.Name))
                result = await _userManager.RemoveFromRoleAsync(user, role.Name);
            else
                result = await _userManager.AddToRoleAsync(user, role.Name);

            return RedirectToAction("ManageUserRoles", new { id = user.Id });
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with ID = {id} do not exist.";
                return View("Error", "Admin");
            }

            var existingUserClaims = await _userManager.GetClaimsAsync(user);
            ManageUserClaimsVM model = new ManageUserClaimsVM
            {
                user_id = id
            };

            foreach(Claim i in ClaimsStore.AllClaims)
            {
                UserClaim claim = new UserClaim
                {
                    claim_type = i.Type
                };

                if(existingUserClaims.Any(c => c.Type == i.Type))
                {
                    claim.isInClaim = true;
                }
                model.claims.Add(claim);
            }


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(ManageUserClaimsVM model, string user_id, string claim_type)
        {
            var user = await _userManager.FindByIdAsync(user_id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with ID = {user_id} do not exist.";
                return View("Error", "Admin");
            }

            var claims = await _userManager.GetClaimsAsync(user);
            IdentityResult result = null;

            bool hasClaim = false;
            foreach(Claim i in claims)
            {
                if (i.Type == claim_type)
                    hasClaim = true;
            }

            if (hasClaim)
                result = await _userManager.RemoveClaimAsync(user, new Claim(claim_type, claim_type));
            else
                result = await _userManager.AddClaimAsync(user, new Claim(claim_type, claim_type));

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "There is something wrong with managing claims!");
                return View();
            }
            return RedirectToAction("ManageUserClaims", new { id = user.Id });
        }
    }
}
