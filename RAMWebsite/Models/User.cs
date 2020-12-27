using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace RAMWebsite.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "To pole jest wymagane")]
        [DisplayName("Login")]
        [Remote(action: "DuplicateUsername", controller: "User")]
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
        [Compare("Password", ErrorMessage = "Hasła muszą się zgadzać")]
        [DisplayName("Potwierdź hasło")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        [Remote(action: "DuplicateEmail", controller: "User")]
        public override string Email { get; set; }

        [ForeignKey("TaskId")]
        [JsonIgnore]
        public virtual ICollection<UserInTask> SolvedTasks { get; set; }
    }
}
