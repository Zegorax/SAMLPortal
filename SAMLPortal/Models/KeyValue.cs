using System;
using System.ComponentModel.DataAnnotations;

namespace SAMLPortal.Models
{
	public class KeyValue
	{
		[Key]
		public virtual int Id { get; set; }

		[Required]
		public virtual string Key { get; set; }

		[Required]
		public virtual string Value { get; set; }

		public KeyValue()
		{

		}
	}
}