using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RAMWebsite.Models
{
    public class ReportRow
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [ForeignKey("Report")]
        public string ReportId { get; set; }

        public virtual Report Report { get; set; }

        [Required]
        public bool Passed { get; set; }

        public string GivenOutput { get; set; }

        public string ExpectedOutput { get; set; }
    }
}
