using ASPNetCoreMVCSample.Entities;
using ASPNetCoreMVCSample.Helpers;
using ASPNetCoreMVCSample.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASPNetCoreMVCSample.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            // Clear the existing cookies to ensure a clear login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            var user = await _userManager.FindByNameAsync(loginModel.Username);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, false, false);
                if (result.Succeeded)
                {
                    await AddClaimsForUser(user);

                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains(RoleHelper.Admin))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (roles.Contains(RoleHelper.Visitor))
                    {
                        return RedirectToAction("Visitor", "Product");
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(loginModel);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerModel);
            }

            var user = new ApplicationUser
            {
                UserName = registerModel.Username,
                Email = registerModel.Username,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(registerModel);
            }

            var role = RoleHelper.Visitor;
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = role });
            }

            await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction("Login");
        }
        private async Task AddClaimsForUser(ApplicationUser user)
        {
            var existingClaims = await _userManager.GetClaimsAsync(user);

            var firstnameClaim = existingClaims.FirstOrDefault(c => c.Type == "firstname");
            if (firstnameClaim == null)
            {
                firstnameClaim = new Claim("firstname", user.FirstName);
                await _userManager.AddClaimAsync(user, firstnameClaim);
            }

            var lastnameClaim = existingClaims.FirstOrDefault(c => c.Type == "lastname");
            if (lastnameClaim == null)
            {
                lastnameClaim = new Claim("lastname", user.LastName);
                await _userManager.AddClaimAsync(user, lastnameClaim);
            }

            await _signInManager.RefreshSignInAsync(user);
        }

    }
}
