using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;

namespace PortunusAdiutor;

///	<summary>
///		An implementation of <see cref="EmailSenderIdentityUser{TKey}"/>
///		that uses the Pbkdf2 salt and hash algorithim to
///		store the password.
///	</summary>
public static class Pbkdf2IdentityUser
{
	private const int DefaultIterCount = 262140;
	private const int DefaultHashedSize = 128;

	///	<summary>
	///		Number of times the <see cref="KeyDerivation.Pbkdf2"/> function should run.
	///	</summary>
	public static int IterationCount { get; set; } = DefaultIterCount;
	///	<summary>
	///		The size of the <see cref="Array"/> 
	///		returned from the <see cref="KeyDerivation.Pbkdf2"/>
	///		that will be set as <see cref="IdentityUser{TKey}.PasswordHash"/>
	///	</summary>
	public static int HashedSize { get; set; } = DefaultHashedSize;
}

///	<summary>
///		An implementation of <see cref="EmailSenderIdentityUser{TKey}"/>
///		that uses the Pbkdf2 salt and hash algorithim to
///		store the password.
///	</summary>
public class Pbkdf2IdentityUser<TKey> : EmailSenderIdentityUser<TKey>
where TKey : IEquatable<TKey>
{
	///	<summary>
	///		Initialize a new instance of <see cref="Pbkdf2IdentityUser{TKey}"/>.
	///	</summary>
	/// <param name="email">This user's email.</param>
	/// <param name="salt">This user's salt</param>
	public Pbkdf2IdentityUser(string email, byte[] salt) 
	: base(email)
	{
		Salt = salt;
	}

	///	<summary>
	///		Initialize a new instance of <see cref="Pbkdf2IdentityUser{TKey}"/>.
	///	</summary>
	/// <param name="email">This user's email.</param>
	/// <param name="password"></param>
	public Pbkdf2IdentityUser(string email, string password) 
	: base(email)
	{
		SetPassword(password);
	}

	///	<summary>
	///		Initialize a new instance of <see cref="Pbkdf2IdentityUser{TKey}"/>.
	///	</summary>
	/// <param name="email">This user's email.</param>
	/// <param name="password"></param>
	///	<param name="username">This user's name.</param>
	public Pbkdf2IdentityUser(string email, string password, string username) 
	: base(email, username)
	{
		SetPassword(password);
	}

	///	<summary>
	///		Sets <see cref="IdentityUser{TKey}.PasswordHash"/> for this
	///		<see cref="Pbkdf2IdentityUser{TKey}"/> with the derived result
	///		of <see cref="KeyDerivation.Pbkdf2"/>.
	///	</summary>
	///	<param name="password">Clear text password.</param>
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

	///	<summary>
	///		Check if <paramref name="password"/> is valid for this 
	///		<see cref="Pbkdf2IdentityUser{TKey}"/>.
	///	</summary>
	///	<param name="password"></param>
	///	<returns></returns>
	public bool ValidatePassword(string password)
	{
		return PasswordHash == DeriveKey(password);
	}

	private string DeriveKey(string pswrd)
	{
		var hashed = KeyDerivation.Pbkdf2(
			pswrd,
			Salt,
			KeyDerivationPrf.HMACSHA256,
			Pbkdf2IdentityUser.IterationCount,
			Pbkdf2IdentityUser.HashedSize
		);
		return Convert.ToBase64String(hashed);
	}

	///	<summary>
	///		Gets the salt for this <see cref="Pbkdf2IdentityUser{TKey}"/>
	///	</summary>
	public byte[] Salt { get; set; }
}
