using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SAMLPortal.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SAMLPortal.Models
{
	public partial class SAMLPortalContext : DbContext
	{
		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
		public virtual DbSet<SAMLPortal.Models.App> App { get; set; }
		public virtual DbSet<SAMLPortal.Models.KeyValue> KeyValue { get; set; }

		public SAMLPortalContext() { }

		public SAMLPortalContext(DbContextOptions<SAMLPortalContext> options) : base(options) { }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseMySql("server=" + Environment.GetEnvironmentVariable("SP_MYSQL_HOST") + ";port=" + Environment.GetEnvironmentVariable("SP_MYSQL_PORT") + ";user=" + Environment.GetEnvironmentVariable("SP_MYSQL_USER") + ";password=" + Environment.GetEnvironmentVariable("SP_MYSQL_PASS") + ";database=" + Environment.GetEnvironmentVariable("SP_MYSQL_DB"), builder =>
				{
					builder.ServerVersion("5.7.26-mysql");
				});
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			OnModelCreatingPartial(modelBuilder);

			// Will come later when adding multiple role to one app
			// var stringArrayConverter = new ValueConverter<string[], string>(
			// v => string.Join(";", v),
			// v => v.Split(";", StringSplitOptions.RemoveEmptyEntries));

			// modelBuilder.Entity<App>()
			// 	.Property(e => e.Roles)
			// 	.HasConversion(stringArrayConverter);

		}
	}
}