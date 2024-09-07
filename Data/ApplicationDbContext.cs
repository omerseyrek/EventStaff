using EventStaf.Models;
using Microsoft.EntityFrameworkCore;

namespace EventStaf.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<AppUser> AppUsers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// You can add any additional configuration for your entities here
			// For example:

			modelBuilder.Entity<AppUser>().HasIndex(u => u.Email).IsUnique();
		}
	}
}
