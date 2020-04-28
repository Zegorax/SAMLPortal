using System.Data;
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

		public virtual string MetadataURL { get; set; } = "";

		[Required]
		public virtual string Role { get; set; }

		[Required]
		public virtual string Issuer { get; set; }

		[Required]
		public virtual Uri SingleSignOnDestination { get; set; }

		[Required]
		public virtual Uri SingleLogoutResponseDestination { get; set; }
	}
}