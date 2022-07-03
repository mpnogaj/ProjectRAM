using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;

using ProjectRAM.Core;
using ProjectRAM.Core.Models;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Identity;
using ProjectRAM.Website.Models;
using ProjectRAM.Website.Helpers;
using ProjectRAM.Website.Data;

namespace ProjectRAM.Website.Controllers
{
	public class TasksController : Controller
	{
		const string CODES_PATH = @"wwwroot/downloads/codes/";

		private readonly AppDbContext _appDbContext;
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		public TasksController(
			AppDbContext appDbContext,
			UserManager<User> userManager,
			SignInManager<User> signInManager)
		{
			_appDbContext = appDbContext;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public IActionResult Index(string sortBy)
		{
			var tasks = from t in _appDbContext.Tasks select t;
			if (!string.IsNullOrEmpty(sortBy))
			{
				switch (sortBy)
				{
					case "Name":
						tasks = tasks.OrderBy(t => t.Name);
						break;
					case "SolvedNumber":
						tasks = tasks.OrderByDescending(t => t.SolvedNumber);
						break;
					default:
						tasks = tasks.OrderBy(t => t.Code);
						break;
				}
			}
			return View(tasks);
		}

		public async Task<IActionResult> Upsert(string id)
		{
			Models.Task md = new Models.Task();
			if (!string.IsNullOrEmpty(id))
			{
				md = await _appDbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
			}

			return View(md);
		}

		public async Task<IActionResult> Details(string id)
		{
			Models.Task md = await _appDbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
			if (User.Identity.IsAuthenticated)
			{
				User u = await _userManager.GetUserAsync(User);
				ViewData["Submissions"] = await _appDbContext.Reports
					.Where(report => report.Task == md && report.AuthorId == u.Id)
					.ToListAsync();
			}
			if (ViewData["Submissions"] == null) ViewData["Submissions"] = new List<Report>();
			return View(md);
		}

		public async Task<IActionResult> Report(string id)
		{
			Report report = await _appDbContext.Reports.FirstOrDefaultAsync(r => r.Id == id);
			report.ReportRows = await _appDbContext.ReportRows.Where(rr => rr.ReportId == report.Id).ToListAsync();
			return View(report);
		}

		[HttpPost]
		public async Task<IActionResult> Upsert(Models.Task t)
		{
			if (t.InputFiles != null)
			{
				List<Test> tests = new List<Test>();

				if (t.InputFiles.Count != t.OutputFiles.Count)
				{
					//dupa
					//powinny być równe
				}
				for (int i = 0; i < t.InputFiles.Count; i++)
				{
					Test test = new Test
					{
						Number = i + 1,
						//Bierzemy tylko pierwszą linijkę
						Input = Converter.IFormFileToStringCollection(t.InputFiles[i])[0],
						Output = Converter.IFormFileToStringCollection(t.OutputFiles[i])[0],
					};
					tests.Add(test);
				}

				t.Tests = tests;
			}
			else if (t.Id == null)
			{
				//Wywal sie
			}

			if (t.Id != null)
			{
				_appDbContext.Tasks.Update(t);
				if (t.InputFiles != null)
				{
					_appDbContext.Tests.RemoveRange(_appDbContext.Tests.Where(test => test.TaskId == t.Id));
				}
			}
			else
			{
				await _appDbContext.Tasks.AddAsync(t);
			}
			await _appDbContext.SaveChangesAsync();
			return RedirectToAction("");
		}

		[HttpPost]
		public async Task<IActionResult> Submit(string id, string code, IFormFile file)
		{
			StringCollection sc = new StringCollection();

			if (file == null)
			{
				if (string.IsNullOrWhiteSpace(code))
				{
					//Dupa
				}
				sc = Converter.StringToStringCollection(code, '\n');
			}
			else
			{
				sc = Converter.IFormFileToStringCollection(file);
			}
			List<Command> CommandList = Creator.CreateCommandList(sc);
			Models.Task task = await _appDbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
			List<Test> tests = await _appDbContext.Tests.Where(t => t.TaskId == task.Id).ToListAsync();
			bool taskSolved = true;
			Report report = new Report
			{
				Task = task,
				ReportRows = new List<ReportRow>(),
				SubmitionDate = DateTime.Now,
				AuthorId = (await _userManager.GetUserAsync(User)).Id
			};
			foreach (Test test in tests)
			{
				//Nie wiem czy działa
				//***
				var token = new System.Threading.CancellationToken();
				var t = System.Threading.Tasks.Task.Run(() =>
					Interpreter.RunCommands(CommandList,
					Creator.CreateInputTapeFromString(test.Input),
					token));
				t.Wait(5000);
				//***
				if (!Interpreter.Executed)
				{
					//niezaliczony
					taskSolved = false;
					report.ReportRows.Add(new ReportRow
					{
						Passed = false,
						ExpectedOutput = test.Output,
						GivenOutput = ""
					});
				}
				else
				{
					string tape = Creator.CreateOutputTapeFromQueue(Interpreter.OutputTape);
					if (tape == test.Output)
					{
						report.ReportRows.Add(new ReportRow
						{
							Passed = true,
						});
					}
					else
					{
						report.ReportRows.Add(new ReportRow
						{
							Passed = false,
							ExpectedOutput = test.Output,
							GivenOutput = tape
						});
						taskSolved = false;
					}
				}
			}
			report.Passed = taskSolved;
			await _appDbContext.Reports.AddAsync(report);
			User user = await _userManager.GetUserAsync(User);
			if (taskSolved == true && !task.SolvedBy.Where(u => u.User == user).Any())
			{
				task.SolvedNumber++;
				task.SolvedBy.Add(new UserInTask
				{
					Task = task,
					TaskId = task.Id,
					User = user,
					UserId = user.Id
				});
				_appDbContext.Tasks.Update(task);
			}
			await _appDbContext.SaveChangesAsync();

			//Pisz kod do pliku
			using (StreamWriter sr = new StreamWriter($"{CODES_PATH}{report.Id}.RAMCode"))
			{
				await sr.WriteAsync(string.Join('\n', sc.Cast<string>()));
			}
			//Wygeneruj raport
			return RedirectToAction("Report", new { id = report.Id });
		}
	}
}