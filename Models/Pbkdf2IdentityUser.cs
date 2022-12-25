using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;

namespace PortunusAdiutor;

///	<summary>
///		An implementation of <see cref="IdentityUser{TKey}"/>
///		that uses the Pbkdf2 salt and hash algorithim to
///		store the password.
///	</summary>
public class Pbkdf2IdentityUser<TKey> : IdentityUser<TKey>, IManagedUser
where TKey : IEquatable<TKey>
{
	private const KeyDerivationPrf defaultPrf = KeyDerivationPrf.HMACSHA512;
	private const int defaultIterCount = 262140;
	private const int defaultHashedSize = 128;

	///	<summary>
	///		Initialize a new instance of <see cref="Pbkdf2IdentityUser{TKey}"/>.
	///	</summary>
	/// <param name="email">This user's email.</param>
	/// <param name="salt">This user's salt</param>
	public Pbkdf2IdentityUser(string email, byte[] salt)
	{
		Email = email;
		Salt = salt;
	}

	///	<summary>
	///		Initialize a new instance of <see cref="Pbkdf2IdentityUser{TKey}"/>.
	///	</summary>
	/// <param name="email">This user's email.</param>
	/// <param name="password">This user's clear text password.</param>
	public Pbkdf2IdentityUser(string email, string password)
	{
		Email = email;
		SetPassword(password);
	}

	///	<inheritdoc/>
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

	///	<inheritdoc/>
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

	///	<inheritdoc/>
	public byte[] Salt { get; set; }
}
