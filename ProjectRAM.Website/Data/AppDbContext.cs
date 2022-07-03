using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectRAM.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectRAM.Website.Data
{
	//Contains all user tables
	public class AppDbContext : IdentityDbContext<User>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<UserInTask>()
				.HasKey(e => new { e.TaskId, e.UserId });
		}

		public DbSet<Task> Tasks { get; set; }

		public DbSet<Test> Tests { get; set; }

		public DbSet<Report> Reports { get; set; }

		public DbSet<ReportRow> ReportRows { get; set; }
	}
}
