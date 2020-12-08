using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RAMWebsite.Models;

namespace RAMWebsite.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserController(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region Views
        public IActionResult Panel()
        {
            return View();
        }


        public IActionResult Login()
        {
            return View();
        }
        
        public IActionResult Register()
        {
            return View(new User());
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> Login(User u, string ReturnUrl, bool rememberMe = false)
        {
            var user = await _userManager.FindByNameAsync(u.UserName);

            if(user != null)
            {
                await _signInManager.PasswordSignInAsync(user, u.Password, rememberMe, false);
            }
            if (!String.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                return Redirect(ReturnUrl);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(User u)
        {
            var res = await _userManager.CreateAsync(u, u.Password);

            if(res.Succeeded)
            {
                await _signInManager.PasswordSignInAsync(u, u.Password, false, false);
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
