using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RAMWebsite.Controllers
{
    public class DownloadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DownloadItem(string fileName)
        {
            if(fileName == null)
            {
                return NotFound();
            }
            string path = fileName;
            string name = new FileInfo(path).Name;
            byte[] mem = await System.IO.File.ReadAllBytesAsync(path);
            return File(mem, "application/zip", name);
        }
    }
}
