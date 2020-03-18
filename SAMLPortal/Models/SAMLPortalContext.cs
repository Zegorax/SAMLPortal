using System;
using Microsoft.EntityFrameworkCore;

namespace SAMLPortal.Models
{
    public class SAMLPortalContext : DbContext
    {
        public DbSet<SAMLApp> SAMLApps { get; set; }

        public SAMLPortalContext(DbContextOptions<SAMLPortalContext> options) : base(options)
        {
        }
    }
}
