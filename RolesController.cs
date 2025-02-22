using Magistri.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Magistri.Controllers {
    [Authorize(Roles ="Admin")]
    public class RolesController : Controller {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<AppUser> userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager) {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public IActionResult Index() {
            return View(roleManager.Roles);
        }

        public IActionResult Create() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(string name) {
            IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
            if (result.Succeeded) {
                return RedirectToAction("Index");
            }
            else {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError("", error.Description);
                }
                return View(name);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id) {
            IdentityRole foundRole = await roleManager.FindByIdAsync(id);
            if (foundRole != null) {
                IdentityResult delete = await roleManager.DeleteAsync(foundRole);
                if (delete.Succeeded) {
                    return RedirectToAction("Index");
                }
                else {
                    foreach (var error in delete.Errors) {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else {
                ModelState.AddModelError("", "No role found");
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id) {
            IdentityRole roleToEdit = await roleManager.FindByIdAsync(id);
            List<AppUser> members = new List<AppUser>();
            List<AppUser> nonmenbers = new List<AppUser>();
            if (roleToEdit != null) {
                foreach (AppUser user in userManager.Users) {
                    var list = await userManager.IsInRoleAsync(user, roleToEdit.Name) ? members : nonmenbers;
                    list.Add(user);
                }
                return View(new RoleEdit {
                    Role = roleToEdit,
                    Members = members,
                    NonMembers = nonmenbers,
                });
            }
            else {
                ModelState.AddModelError("", "Role not found");
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(RoleModification modification) {
            foreach (string userID in modification.AddIds ?? Array.Empty<string>()) {
                AppUser user = await userManager.FindByIdAsync(userID);
                if (user != null) {
                    IdentityResult result= await userManager.AddToRoleAsync(user, modification.RoleName);
                    if (!result.Succeeded) {
                        AddModelErrors(result);
                    }
                }
            }
            foreach (string userId in modification.DeleteIds ?? Array.Empty<string>()) {
                AppUser user = await userManager.FindByIdAsync(userId); 
                if (user != null) {
                IdentityResult result = await userManager.RemoveFromRoleAsync(user, modification.RoleName);
                    if (!result.Succeeded) {
                        AddModelErrors(result);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        private void AddModelErrors(IdentityResult result) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
