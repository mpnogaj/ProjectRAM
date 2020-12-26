using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RAMWebsite.Data;
using System.Linq;

namespace RAMWebsite.Controllers
{
    [Route("api/values")]
    [ApiController]
    public class ApiValuesController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public ApiValuesController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [Route("tasks")]
        public string Tasks()
        {
            return JsonConvert.SerializeObject(_appDbContext.Tasks);
        }

        [Route("reports/{userId}/{taskId}")]
        public string Reports(string userId, string taskId)
        {
            return JsonConvert.SerializeObject(
                _appDbContext.Reports
                .Where(r => r.AuthorId == userId && r.Task.Id == taskId));
        }

        [Route("/report_rows/{reportId}")]
        public string ReportRows(string reportId)
        {
            return JsonConvert.SerializeObject(
                _appDbContext.ReportRows
                .Where(rr => rr.ReportId == reportId));
        }
    }
}
