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
        const string DOWNLOADS_PATH = @"wwwroot/downloads";
        const string CODES_PATH = DOWNLOADS_PATH + @"/codes/";

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DownloadItem(string fileName, bool open = false)
        {
            
            if(string.IsNullOrEmpty(fileName))
            {
                return NotFound();
            }
            try
            {
                if (fileName.Contains('.'))
                {
                    string path = DOWNLOADS_PATH + fileName;
                    string name = new FileInfo(path).Name;
                    byte[] mem = await System.IO.File.ReadAllBytesAsync(path);
                    return File(mem, "application/zip", name);
                }
                else
                {
                    if (!open)
                    {
                        byte[] mem = await System.IO.File.ReadAllBytesAsync(CODES_PATH + fileName + ".RAMCode");
                        return File(mem, "text/plain", fileName + ".RAMCode");
                    }
                    else
                    {
                        string content = "";
                        using(StreamReader sr = new StreamReader(CODES_PATH + fileName + ".RAMCode"))
                        {
                            content = await sr.ReadToEndAsync();
                        }
                        return Content(content);
                    }
                }
            }
            catch { return NotFound(); }
        }
    }
}
