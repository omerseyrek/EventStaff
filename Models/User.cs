namespace EventStaf.Models
{
	public class AppUser : EntityBase
	{
		public string Username { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		// Add other properties as needed
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime DateOfBirth { get; set; }
		public DateTime RegistrationDate { get; set; }
		public bool IsActive { get; set; }
	}
}
