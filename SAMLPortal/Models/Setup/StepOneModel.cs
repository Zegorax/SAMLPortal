using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SAMLPortal.Models.Setup
{
	public class StepOneModel
	{
		[Required(ErrorMessage = "MySQL host cannot be empty")]
		[RegularExpression(@"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$", ErrorMessage = "MySQL host is incorrectly formatted")]
		[DisplayNameAttribute("Host")]
		public string MySQLHost { get; set; }

		[Required(ErrorMessage = "MySQL port cannot be empty")]
		[RegularExpression(@"^()([1-9]|[1-5]?[0-9]{2,4}|6[1-4][0-9]{3}|65[1-4][0-9]{2}|655[1-2][0-9]|6553[1-5])$", ErrorMessage = "MySQL port is incorrectly formatted")]
		[DisplayNameAttribute("Port")]
		public int MySQLPort { get; set; }

		[Required(ErrorMessage = "Database name cannot be empty")]
		[RegularExpression(@"^[a-zA-Z]{1,20}$", ErrorMessage = "Database name is incorrectly formatted")]
		[DisplayNameAttribute("Database name")]
		public string MySQLDatabaseName { get; set; }

		[Required(ErrorMessage = "MySQL user cannot be empty")]
		[RegularExpression(@"^[a-zA-Z0-9]+(?:[_ -]?[a-zA-Z0-9])*$", ErrorMessage = "User is incorrectly formatted")]
		[DisplayNameAttribute("User")]
		public string MySQLUser { get; set; }

		[Required(ErrorMessage = "MySQL user's password cannot be empty")]
		[RegularExpression(@"^[^"";]*$", ErrorMessage = @"MySQL password may contain forbidden characters. They are : "" ; ")] // " and ; are forbidden
		[DisplayNameAttribute("Password")]
		[DataType(DataType.Password)]
		public string MySQLPassword { get; set; }
	}
}