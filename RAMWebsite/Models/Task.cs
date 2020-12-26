using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Newtonsoft.Json;

namespace RAMWebsite.Models
{
    public class Task
    {
        //Auto increment
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

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
        [JsonIgnore]
        public IFormFileCollection InputFiles { get; set; }

        [Required]
        [DisplayName("Testy - wyjście")]
        [NotMapped]
        [JsonIgnore]
        public IFormFileCollection OutputFiles { get; set; }

        [DefaultValue(0)]
        public int SolvedNumber { get; set; }

        [Required]
        [JsonIgnore]
        public virtual ICollection<Test> Tests { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual ICollection<UserInTask> SolvedBy { get; set; }
    }
}
