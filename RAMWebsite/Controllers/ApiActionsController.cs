using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RAMWebsite.Models;
using RAMWebsite.Data;
using Newtonsoft.Json;

namespace RAMWebsite.Controllers
{
    [Route("api/actions")]
    [ApiController]
    public class ApiActionsController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _appDbContext;

        public ApiActionsController(SignInManager<User> signInManager,
            UserManager<User> userManager,
            AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Route("duplicate/{username}")]
        public IActionResult CheckDuplicatedUsername(string username)
        {
            bool duplicate =  _appDbContext.Users.Where(u => u.UserName == username).Count() != 0;
            if(duplicate)
            {
                return Content("Ta nazwa użytkownika jest już używana", "application/json");
            }
            return Content(JsonConvert.SerializeObject(true), "application/json");
        }
    }
}
