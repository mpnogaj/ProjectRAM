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
        [Route("login")]
        public async Task<string> Login(PartialUser u)
        {
            User user = await _userManager.FindByNameAsync(u.UserName);
            if(user != null)
            {
                var res = await _signInManager.PasswordSignInAsync(user, u.Password, u.RememberMe, false);
                if(res.Succeeded)
                {
                    return JsonConvert.SerializeObject(new Status
                    {
                        Success = true,
                        Message = "Pomyślnie zalogowano"
                    });
                }
                return JsonConvert.SerializeObject(new Status
                {
                    Success = false,
                    Message = "Złe hasło"
                });
            }
            return JsonConvert.SerializeObject(new Status
            {
                Success = false,
                Message = "Nazwa użytkownika nie została znaleziona"
            });
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

        [HttpPost]
        [Route("logout")]
        public async Task<string> Logout()
        {
            bool isSignedIn = _signInManager.IsSignedIn(User);
            if (isSignedIn)
            {
                await _signInManager.SignOutAsync();
                return JsonConvert.SerializeObject(new Status
                {
                    Success = true,
                    Message = "Pomyślnie wylogowano"
                });
            }
            return JsonConvert.SerializeObject(new Status
            {
                Success = false,
                Message = "Coś poszło nie tak"
            });
        }
    }
}
