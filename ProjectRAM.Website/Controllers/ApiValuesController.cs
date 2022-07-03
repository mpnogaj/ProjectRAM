using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectRAM.Website.Data;
using System.Linq;
using System.Security.Claims;

namespace ProjectRAM.Website.Controllers
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

		[Route("user_id")]
		public string GetUserId()
		{
			if (User.Identity.IsAuthenticated)
			{
				return JsonConvert.SerializeObject(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			}
			return "";
		}

		[Route("reports/{userId}/{taskId}")]
		public string Reports(string userId, string taskId)
		{
			var reports = _appDbContext.Reports.Where(r => r.AuthorId == userId && r.Task.Id == taskId).ToList();
			return JsonConvert.SerializeObject(reports);
		}

		[Route("report_rows/{reportId}")]
		public string ReportRows(string reportId)
		{
			return JsonConvert.SerializeObject(
				_appDbContext.ReportRows
				.Where(rr => rr.ReportId == reportId));
		}
	}
}
