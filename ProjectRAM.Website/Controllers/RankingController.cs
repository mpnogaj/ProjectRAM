using Microsoft.AspNetCore.Mvc;
using ProjectRAM.Website.Data;
using RAMWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
