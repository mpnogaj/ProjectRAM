using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
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

        //[HttpPost]
        //public async Task<IActionResult> Login(User u, string ReturnUrl, bool rememberMe = false)
        //{
        //    var user = await _userManager.FindByNameAsync(u.UserName);

        //    if(user != null)
        //    {
        //        await _signInManager.PasswordSignInAsync(user, u.Password, rememberMe, false);
        //    }
        //    if (!String.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
        //        return Redirect(ReturnUrl);
        //    return RedirectToAction("Index", "Home");
        //}

        //[HttpPost]
        //public async Task<IActionResult> Register(User u, string ReturnUrl)
        //{
        //    var res = await _userManager.CreateAsync(u, u.Password);

        //    if(res.Succeeded)
        //    {
        //        await _signInManager.PasswordSignInAsync(u, u.Password, false, false);
        //    }
        //    else if(res.Errors.Count() > 0 )
        //    {
        //        switch (res.Errors.First())
        //        {

        //        }
        //    }
        //    if (!String.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
        //        return Redirect(ReturnUrl);
        //    return RedirectToAction("Index", "Home");
        //}

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public JsonResult UsernameDoesNotExists(string UserName)
        {
            User u = _userManager.FindByNameAsync(UserName).Result;
            bool contains = u != null;
            if (!contains)
            {
                return Json("Ta nazwa użytkownika nie istnieje");
            }
            return Json(true);
        }

        public JsonResult DuplicateUsername(string UserName)
        {
            User u = _userManager.FindByNameAsync(UserName).Result;
            bool duplicate = u != null;
            if (duplicate)
            {
                return Json("Ta nazwa użytkownika jest już używana");
            }
            return Json(true);
        }

        public JsonResult DuplicateEmail(string Email)
        {
            User u = _userManager.FindByEmailAsync(Email).Result;
            bool duplicate = u != null;
            if (duplicate)
            {
                return Json("Ten adres email jest już używany");
            }
            return Json(true);
        }
    }
}
