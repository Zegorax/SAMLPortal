using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SAMLPortal.Models.Setup
{
	public class StepThreeModel
	{
		[Required(ErrorMessage = "LDAP host cannot be empty")]
		[RegularExpression(@"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$", ErrorMessage = "LDAP host is incorrectly formatted")]
		[DisplayNameAttribute("Host")]
		public string Host { get; set; }

		[Required(ErrorMessage = "LDAP port cannot be empty")]
		[RegularExpression(@"^()([1-9]|[1-5]?[0-9]{2,4}|6[1-4][0-9]{3}|65[1-4][0-9]{2}|655[1-2][0-9]|6553[1-5])$", ErrorMessage = "LDAP port is incorrectly formatted")]
		[DisplayNameAttribute("Port")]
		public int Port { get; set; }

		[Required(ErrorMessage = "SSL presence should be specified")]
		[DisplayNameAttribute("SSL")]
		public bool SSL { get; set; }

		[Required(ErrorMessage = "Bind DN cannot be empty")]
		[DisplayNameAttribute("Bind DN")]
		public string BindDN { get; set; }

		[Required(ErrorMessage = "Bind password cannot be empty")]
		[DisplayNameAttribute("Bind Password")]
		[DataType(DataType.Password)]
		public string BindPassword { get; set; }
	}
}