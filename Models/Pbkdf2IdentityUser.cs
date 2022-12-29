using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;

namespace PortunusAdiutor.Models;

public class Pbkdf2IdentityUser<TKey> : IdentityUser<TKey>, IManagedUser
where TKey : IEquatable<TKey>
{
	private const KeyDerivationPrf defaultPrf = KeyDerivationPrf.HMACSHA512;
	private const int defaultIterCount = 262140;
	private const int defaultHashedSize = 128;

	public Pbkdf2IdentityUser(string email, byte[] salt)
	{
		Email = email;
		Salt = salt;
	}

	public Pbkdf2IdentityUser(string email, string password)
	{
		Email = email;
		SetPassword(password);
	}

	[MemberNotNull(nameof(Salt))]
	public void SetPassword(string password)
	{
		using (var sha = SHA256.Create()) {
			Salt =
				sha.ComputeHash(BitConverter.GetBytes(DateTime.UtcNow.ToBinary()));
		}
		PasswordHash = DeriveKey(password);
		return;
	}

	public bool ValidatePassword(string password)
	{
		return PasswordHash == DeriveKey(password);
	}

	private string DeriveKey(string pswrd)
	{
		var hashed = KeyDerivation.Pbkdf2(
			pswrd,
			Salt,
			defaultPrf,
			defaultIterCount,
			defaultHashedSize
		);
		return Convert.ToBase64String(hashed);
	}
	
	public byte[] Salt { get; set; }

	public Claim[] GetClaims()
	{
		var id = Id.ToString();
		ArgumentNullException.ThrowIfNull(id);
		ArgumentNullException.ThrowIfNull(Email);
		return new[] {
			new Claim(ClaimTypes.PrimarySid, id),
			new Claim(ClaimTypes.Email, Email)
		};
	}
}
