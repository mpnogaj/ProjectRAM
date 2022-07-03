using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace ProjectRAM.Website.Models
{
	public class UserInTask
	{
		public string TaskId { get; set; }
		public virtual Task Task { get; set; }

		public string UserId { get; set; }
		public virtual User User { get; set; }
	}
}
