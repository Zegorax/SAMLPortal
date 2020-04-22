using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SAMLPortal.Models.Setup
{
	public class StepFiveModel
	{
		[Required(ErrorMessage = "Display name attribute cannot be empty")]
		[RegularExpression(@"^[a-zA-Z0-9]*$")]
		[DisplayNameAttribute("Display name")]
		public string DisplayName { get; set; }

		[Required(ErrorMessage = "UID attribute cannot be empty")]
		[RegularExpression(@"^[a-zA-Z0-9]*$")]
		[DisplayNameAttribute("UID")]
		public string UID { get; set; }

		[Required(ErrorMessage = "MemberOf attritbute cannot be empty")]
		[RegularExpression(@"^[a-zA-Z0-9]*$")]
		[DisplayNameAttribute("MemberOf attribute")]
		public string MemberOf { get; set; }

		[Required(ErrorMessage = "Mail attribute cannot be empty")]
		[RegularExpression(@"^[a-zA-Z0-9]*$")]
		[DisplayNameAttribute("Mail attribute")]
		public string Mail { get; set; }
	}
}