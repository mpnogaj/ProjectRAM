using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using RAMWebsite.Models;

namespace RAMWebsite.Controllers
{ 
    [ApiController]
    public class ApiController : ControllerBase
    {
        [Route("api/test")]
        public string test()
        {
            return JsonConvert.SerializeObject(new Test { Id = "15" });
        }
    }
}
