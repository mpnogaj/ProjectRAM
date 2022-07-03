using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ProjectRAM.Website.Models
{
	public class Report
	{
		[Required]
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string Id { get; set; }

		[Required]
		public bool Passed { get; set; }

		[Required]
		[JsonIgnore]
		public virtual Task Task { get; set; }

		[Required]
		[JsonIgnore]
		public virtual ICollection<ReportRow> ReportRows { get; set; }

		[Required]
		public string AuthorId { get; set; }

		[Required]
		public DateTime SubmitionDate { get; set; }
	}
}
