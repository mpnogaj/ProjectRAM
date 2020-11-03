using System;
using System.Collections.Generic;
using System.Linq;
using RAMWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using RAMWebsite.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using RAMWebsite.Helpers;

using Common;
using System.Runtime.InteropServices;
using System.Collections.Specialized;

namespace RAMWebsite.Controllers
{
    public class TasksController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public TasksController(
            AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IActionResult Index()
        {
            return View(_appDbContext.Tasks);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Models.Task md = new Models.Task();
            if (id.HasValue)
            {
                md = await _appDbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            }
            
            return View(md);
        }

        public async Task<IActionResult> Details(int id)
        {
            Models.Task md = await _appDbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            return View(md);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(Models.Task t)
        {
            if (t.InputFiles != null && t.Id == 0)
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
            else
            {
                //Dupa
            }

            if (t.Id != 0)
            {
                _appDbContext.Tasks.Update(t);
            }
            else
            {
                await _appDbContext.Tasks.AddAsync(t);
            }
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("");
        }

        [HttpPost]
        public async Task<IActionResult> Submit(int id, string code, IFormFile file)
        {
            StringCollection sc;
            if(file == null)
            {
                if(String.IsNullOrWhiteSpace(code))
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
            Models.Task t = await _appDbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            foreach (Test test in t.Tests)
            {
                //Dla karzdego testu zasymyluj działanie programu
            }
            //Wygeneruj raport
            return RedirectToAction("");
        }
    }
}
