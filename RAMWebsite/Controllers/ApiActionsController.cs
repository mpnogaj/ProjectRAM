using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RAMWebsite.Data;
using RAMWebsite.Models;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpPost]
        [Route("register")]
        public async Task<string> Register(User u)
        {
            var res = await _userManager.CreateAsync(u, u.Password);
            if (res.Succeeded)
            {
                return JsonConvert.SerializeObject(new Status
                {
                    Success = true,
                    Message = "Konto zostało zarejestrowane"
                });
            }
            return JsonConvert.SerializeObject(new Status
            {
                Success = false,
                Message = res.Errors.First().Description
            });
        }
    }
}
