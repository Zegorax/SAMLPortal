using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SAMLPortal.Models;

namespace SAMLPortal.Models
{
    public partial class SAMLPortalContext : DbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        public virtual DbSet<SAMLPortal.Models.App> App { get; set; }
        public virtual DbSet<SAMLPortal.Models.KeyValue> KeyValue { get; set; }

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
                optionsBuilder.UseMySql("server="+ Environment.GetEnvironmentVariable("SP_MYSQL_HOST") + ";port="+ Environment.GetEnvironmentVariable("SP_MYSQL_PORT") +";user="+ Environment.GetEnvironmentVariable("SP_MYSQL_USER") +";password="+ Environment.GetEnvironmentVariable("SP_MYSQL_PASS") +";database=" + Environment.GetEnvironmentVariable("SP_MYSQL_DB"), x => x.ServerVersion("5.7.26-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }        
    }
}
