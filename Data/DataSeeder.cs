using EventStaf.Entities;
using EventStaf.Infra.Constants;
using System.Security.Cryptography;
using System.Text;

namespace EventStaf.Data
{
    public static class DataSeeder
	{
		public static void SeedData(ApplicationDbContext context)
		{
			if (!context.AppUsers.Any())
			{
				var users = new List<AppUser>
				{
					new AppUser
					{
						Username = "john_doe",
						Email = "john.doe@example.com",
						FirstName = "John",
						LastName = "Doe",
						PasswordHash = GetHashFromString("john"),
						DateOfBirth = new DateTime(1985, 5, 15),
						RegistrationDate = DateTime.Now.AddYears(-3),
						IsActive = true
					},
					new AppUser
					{
						Username = "jane_smith",
						Email = "jane.smith@example.com",
						FirstName = "Jane",
						LastName = "Smith",
						PasswordHash = GetHashFromString("john"),
						DateOfBirth = new DateTime(1990, 8, 22),
						RegistrationDate = DateTime.Now.AddYears(-2),
						IsActive = true
					},
					new AppUser
					{
						Username = "bob_johnson",
						Email = "bob.johnson@example.com",
						FirstName = "Bob",
						LastName = "Johnson",
						PasswordHash = GetHashFromString("john"),
						DateOfBirth = new DateTime(1978, 3, 10),
						RegistrationDate = DateTime.Now.AddYears(-5),
						IsActive = false
					},
					new AppUser
					{
						Username = "alice_wonderland",
						Email = "alice.wonderland@example.com",
						FirstName = "Alice",
						LastName = "Wonderland",
						PasswordHash = GetHashFromString("john"),
						DateOfBirth = new DateTime(1995, 12, 1),
						RegistrationDate = DateTime.Now.AddMonths(-6),
						IsActive = true
					},
					new AppUser
					{
						Username = "charlie_brown",
						Email = "charlie.brown@example.com",
						FirstName = "Charlie",
						LastName = "Brown",
						PasswordHash = GetHashFromString("john"),
						DateOfBirth = new DateTime(1980, 7, 30),
						RegistrationDate = DateTime.Now.AddYears(-1),
						IsActive = true
					},
					new AppUser
					{
						Username = "emma_watson",
						Email = "emma.watson@example.com",
						FirstName = "Emma",
						LastName = "Watson",
						PasswordHash = GetHashFromString("john"),
						DateOfBirth = new DateTime(1992, 4, 15),
						RegistrationDate = DateTime.Now.AddDays(-45),
						IsActive = true
					},
					new AppUser
					{
						Username = "david_beckham",
						Email = "david.beckham@example.com",
						FirstName = "David",
						LastName = "Beckham",
						PasswordHash = GetHashFromString("john"),
						DateOfBirth = new DateTime(1975, 5, 2),
						RegistrationDate = DateTime.Now.AddYears(-7),
						IsActive = true
					},
					new AppUser
					{
						Username = "sarah_connor",
						Email = "sarah.connor@example.com",
						FirstName = "Sarah",
						LastName = "Connor",
						PasswordHash = GetHashFromString("john"),
						DateOfBirth = new DateTime(1982, 11, 13),
						RegistrationDate = DateTime.Now.AddMonths(-3),
						IsActive = false
					}
				};

				context.AppUsers.AddRange(users);
				context.SaveChanges();
			}
		}


		public static string GetHashFromString(string valueToBeHashed)
		{

			// byte array representation of that string
			byte[] encodedPassword = new UTF8Encoding().GetBytes(valueToBeHashed);

			// need MD5 to calculate the hash
			HashAlgorithm? algorithm = CryptoConfig.CreateFromName(ConstantKeys.MD5) as HashAlgorithm;
			if (algorithm == null)
			{
				return string.Empty;
			}

			byte[] hash = algorithm?.ComputeHash(encodedPassword) ?? Array.Empty<byte>();

			// string representation (similar to UNIX format)
			string encoded = BitConverter.ToString(hash)
			   // without dashes
			   .Replace("-", string.Empty)
			   // make lowercase
			   .ToLower();

			return encoded;
		}
	}
}
