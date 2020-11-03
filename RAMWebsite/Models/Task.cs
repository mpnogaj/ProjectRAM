using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.Language.Intermediate;

namespace RAMWebsite.Models
{
    public class Task
    {
        //Auto increment
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [DisplayName("Kod zadania")]
        [StringLength(8, MinimumLength = 3)]
        public string Code { get; set; }

        [Required]
        [DisplayName("Nazwa zadania")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Opis zadania")]
        public string Description { get; set; }

        [Required]
        [DisplayName("Opis wejścia")]
        public string Input { get; set; }

        [Required]
        [DisplayName("Opis wyjścia")]
        public string Output { get; set; }

        [Required]
        [DisplayName("Przykładowe wejście")]
        public string ExampleInput { get; set; }

        [Required]
        [DisplayName("Przykładowe wyjście")]
        public string ExampleOutput { get; set; }

        [Required]
        [DisplayName("Testy - wejście")]
        [NotMapped]
        public IFormFileCollection InputFiles { get; set; }

        [Required]
        [DisplayName("Testy - wyjście")]
        [NotMapped]
        public IFormFileCollection OutputFiles { get; set; }

        [DefaultValue(0)]
        public int SolvedNumber { get; set; }

        [Required]
        public ICollection<Test> Tests { get; set; }
    }
}
