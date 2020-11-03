using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RAMWebsite.Models
{
    public class Test
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        //Numer testu
        public int Number { get; set; }

        [Required]
        /*
        Taśma wejścia 
        Format:
        "1 0 1 2 5 18 0"
        */
        public string Input { get; set; }

        [Required]
        /*
        Oczekiwana taśma wyjścia 
        Format:
        "1 0 1 0 1 0"
        */
        public string Output { get; set; }
    }
}
