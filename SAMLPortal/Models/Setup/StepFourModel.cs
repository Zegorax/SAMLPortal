using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SAMLPortal.Models.Setup
{
	public class StepFourModel
	{
		[Required(ErrorMessage = "LDAP search base cannot be empty")]
		[DisplayNameAttribute("Search base")]
		public string SearchBase { get; set; }

		[Required(ErrorMessage = "Administrator filter cannot be empty")]
		[DisplayNameAttribute("Administrators filter")]
		public string AdminFilter { get; set; }

		[Required(ErrorMessage = "User filter cannot be empty")]
		[DisplayNameAttribute("User filter")]
		public string UserFilter { get; set; }

	}
}