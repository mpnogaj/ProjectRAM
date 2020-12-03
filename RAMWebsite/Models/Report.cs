using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RAMWebsite.Models
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
        public virtual Task Task { get; set; }

        [Required]
        public virtual ICollection<ReportRow> ReportRows { get; set; }

        [Required]
        public string AuthorId { get; set; }

        [Required]
        public DateTime SubmitionDate { get; set; }
    }
}
