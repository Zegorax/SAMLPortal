using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SAMLPortal.Models;

namespace SAMLPortal.Models
{
    public partial class SAMLPortalContext : DbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        public DbSet<SAMLPortal.Models.App> App { get; set; }



        public SAMLPortalContext()
        {
        }

        public SAMLPortalContext(DbContextOptions<SAMLPortalContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=root;database=samlportal", x => x.ServerVersion("5.7.26-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        public DbSet<SAMLPortal.Models.Setup> Setup { get; set; }

        
    }
}
