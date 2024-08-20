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

        // Constructor injecting services required for authentication and authorization
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

        // 
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            // Find the user by username
            var user = await _userManager.FindByNameAsync(loginModel.Username);
            if (user != null)
            {
                // Attempt to sign in with the provided username and password
                var result = await _signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, false, false);
                if (result.Succeeded)
                {
                    // Add claims to the user and redirect based on roles
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

            // Add an error if login attempt was not successful
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(loginModel);
        }

        // POST action to handle logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user and redirect to the login page
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
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return View(registerModel);
            }

            // Create a new user with the provided details
            var user = new ApplicationUser
            {
                UserName = registerModel.Username,
                Email = registerModel.Username,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName
            };

            // Attempt to create the user with the provided password
            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
            {
                // Add any errors from the result to the model state
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(registerModel);
            }

            // Ensure the "Visitor" role exists and add the user to the role
            var role = RoleHelper.Visitor;
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = role });
            }

            await _userManager.AddToRoleAsync(user, role);

            // Redirect to the login page after successful registration
            return RedirectToAction("Login");
        }

        // Helper method to add claims for the user
        private async Task AddClaimsForUser(ApplicationUser user)
        {
            var existingClaims = await _userManager.GetClaimsAsync(user);

            // Add first name claim if it does not exist
            var firstnameClaim = existingClaims.FirstOrDefault(c => c.Type == "firstname");
            if (firstnameClaim == null)
            {
                firstnameClaim = new Claim("firstname", user.FirstName);
                await _userManager.AddClaimAsync(user, firstnameClaim);
            }

            // Add last name claim if it does not exist
            var lastnameClaim = existingClaims.FirstOrDefault(c => c.Type == "lastname");
            if (lastnameClaim == null)
            {
                lastnameClaim = new Claim("lastname", user.LastName);
                await _userManager.AddClaimAsync(user, lastnameClaim);
            }

            // Refresh the sign-in to apply new claims
            await _signInManager.RefreshSignInAsync(user);
        }
    }
}
