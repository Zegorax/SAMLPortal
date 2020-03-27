using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAMLPortal.Misc;
using SAMLPortal.Models;

namespace SAMLPortal.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private readonly Models.IAuthenticationService _authService;


        public AuthController(Models.IAuthenticationService authService)
        {
            _authService = authService;

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            LoginViewModel emptyLogin = new LoginViewModel();
            emptyLogin.Username = "";
            emptyLogin.Password = "";

            return View(emptyLogin);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _authService.Login(model.Username, model.Password);
                    if (null != user)
                    {
                        var userClaims = new List<Claim>
                        {
                            new Claim("displayName", user.DisplayName),
                            new Claim("username", user.Username)
                        };

                        if (user.IsAdmin)
                        {
                            userClaims.Add(new Claim(ClaimTypes.Role, UserRoles.Administrator));
                        }
                        else
                        {
                            userClaims.Add(new Claim(ClaimTypes.Role, UserRoles.User));
                            //userClaims.Add(new Claim(ClaimTypes.Role, UserRoles.Administrator));
                        }

                        var principal = new ClaimsPrincipal(new ClaimsIdentity(userClaims, _authService.GetType().Name));
                        await HttpContext.SignInAsync("SAMLPortal", principal);
                        return Redirect("/");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("SAMLPortal");
            return Redirect("/");
        }
    }
}