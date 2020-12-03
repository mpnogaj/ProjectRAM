using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RAMWebsite.Models;

namespace RAMWebsite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Manager()
        {
            ViewBag.manager = _userManager;
            return View(_userManager.Users.ToList());
        }

        public async Task<IActionResult> AddRole(string id, string role)
        {
            User u = await _userManager.FindByIdAsync(id);
            IEnumerable<string> roles = await _userManager.GetRolesAsync(u);
            await _userManager.RemoveFromRolesAsync(u, roles);
            await _userManager.AddToRoleAsync(u, role);
            return RedirectToAction("Manager");
        }

        public async Task<IActionResult> Delete(string id)
        {
            User u = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(u);
            return RedirectToAction("Manager");
        }
    }
}
