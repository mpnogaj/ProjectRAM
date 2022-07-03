using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectRAM.Website.Models
{
	public class PartialUser
	{
		[Required(ErrorMessage = "To pole jest wymagane")]
		[DisplayName("Login")]
		[Remote(action: "UsernameDoesNotExists", controller: "User")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "To pole jest wymagane")]
		[DisplayName("Hasło")]
		public string Password { get; set; }

		[DisplayName("Zapamiętaj mnie")]
		public bool RememberMe { get; set; } = false;
	}
}
