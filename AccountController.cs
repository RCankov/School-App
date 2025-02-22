using Magistri.DTO;
using Magistri.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Magistri.Controllers {
    [Authorize]
    public class AccountController : Controller {
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl) {
            LoginDto loginDto = new LoginDto();
            loginDto.ReturnUrl = returnUrl;
            return View(loginDto);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto) {
            if (ModelState.IsValid) {
                AppUser appUser = await userManager.FindByNameAsync(loginDto.UserName);
                if (appUser != null) {
                    Microsoft.AspNetCore.Identity.SignInResult signInResult = await signInManager.PasswordSignInAsync(appUser, loginDto.Password, false, false);
                    if (signInResult.Succeeded) {
                        return Redirect(loginDto.ReturnUrl ?? "/");
                    }
                }
                ModelState.AddModelError("", "Login failed - check username and password");
            }
            return View(loginDto);
        }

        public async Task<IActionResult> Logout() {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied() {
            return View();
        }

    }
}
