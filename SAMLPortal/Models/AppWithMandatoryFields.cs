using System.ComponentModel.DataAnnotations;

namespace SAMLPortal.Models
{
    public class AppWithMandatoryFields
    {
		[Required]
		public virtual string Name { get; set; }

		[Required]
		public virtual string Description { get; set; }

		[Required]
		public virtual bool Enabled { get; set; }

		[Required]
		public virtual string MetadataURL { get; set; }
    }
}