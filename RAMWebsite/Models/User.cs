using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace RAMWebsite.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "To pole jest wymagane")]
        [DisplayName("Login")]
        public override string UserName { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        [DisplayName("Imię")]
        public string Name { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        [DisplayName("Nazwisko")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        [NotMapped]
        [DisplayName("Hasło")]
        public string Password { get; set; }

        [NotMapped]
        [Compare("Password")]
        [DisplayName("Potwierdź hasło")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        public override string Email { get; set; }

        [ForeignKey("TaskId")]
        [JsonIgnore]
        public virtual ICollection<UserInTask> SolvedTasks { get; set; }
    }
}
