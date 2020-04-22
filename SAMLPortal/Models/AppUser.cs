using System;
namespace SAMLPortal.Models
{
	public interface IAuthenticationService
	{
		AppUser Login(string username, string password);
	}

	public class AppUser
	{
		public string DisplayName { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public bool IsAdmin { get; set; }
		public string[] Memberships { get; set; }
	}
}