using System;
using System.ComponentModel.DataAnnotations;

namespace SAMLPortal.Models
{
    public class App
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        public virtual string Name { get; set; }

        [Required]
        public virtual string Description { get; set; }

        [Required]
        public virtual bool Enabled { get; set; }

        public virtual string MetadataURL { get; set; }
        public virtual string Issuer { get; set; }
        public virtual Uri SingleSignOnDestination { get; set; }
        public virtual Uri SingleLogoutResponseDestination { get; set; }
        public virtual string SignatureValidationCertificate { get; set; }

        public App()
        {
        }
    }
}
