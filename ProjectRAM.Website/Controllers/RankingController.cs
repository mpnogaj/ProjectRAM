using Microsoft.AspNetCore.Mvc;
using ProjectRAM.Website.Data;
using System.Linq;

namespace ProjectRAM.Website.Controllers
{
	public class RankingController : Controller
	{
		private readonly AppDbContext _appDbContext;

		public RankingController(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;
		}

		public IActionResult Index()
		{
			var users = (from u in _appDbContext.Users select u).OrderByDescending(u => u.SolvedTasks.Count);
			return View(users);
		}
	}
}
