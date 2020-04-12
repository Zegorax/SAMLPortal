using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SAMLPortal.Models.Setup
{
	public class SecondStepModel
	{
		[Required(ErrorMessage = "Your company name cannot be empty")]
		[DisplayNameAttribute("Company name")]
		public string CompanyName { get; set; }

		[Required(ErrorMessage = "Company country cannot be empty")]
		[DisplayNameAttribute("Country")]
		public string CountryCode { get; set; }

		[DisplayNameAttribute("Country")]
		public SelectList CountryList { get; set; }

		[Required(ErrorMessage = "Application host cannot be empty")]
		[DisplayNameAttribute("Application host")]
		[DataType(DataType.Url)]
		public string AppHost { get; set; }
	}
}