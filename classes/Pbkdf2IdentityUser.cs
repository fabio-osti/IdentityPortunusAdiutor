using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;

namespace PortunusAdiutor;

///	<summary>
///		Non-generic container for the static values.
///	</summary>
public static class Pbkdf2IdentityUser
{
	///	<summary>
	///		Number of times the <see cref="KeyDerivation.Pbkdf2"/> function should run.
	///	</summary>
	public static int IterationCount { get; set; } = 262140;
	///	<summary>
	///		The size of the <see cref="Array"/> returned from the <see cref="KeyDerivation.Pbkdf2"/>
	///		that will be set as <see cref="IdentityUser{TKey}.PasswordHash"/>
	///	</summary>
	public static int HashedSize { get; set; } = 128;
}

///	<summary>
///		An implementation of <see cref="IdentityUser{TKey}"/>
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
	public Pbkdf2IdentityUser(string email) : base(email)
	{
		CreationDate = DateTime.UtcNow;
	}

	///	<summary>
	///		Initialize a new instance of <see cref="Pbkdf2IdentityUser{TKey}"/>.
	///	</summary>
	/// <param name="email">This user's email.</param>
	///	<param name="userName">This user's name.</param>
	public Pbkdf2IdentityUser(string email, string userName) : base(email, userName)
	{
		CreationDate = DateTime.UtcNow;
	}

	///	<summary>
	///		UTC <see cref="DateTime"/> of this 
	///		<see cref="Pbkdf2IdentityUser{TKey}"/> creation.
	///	</summary>
	public DateTime CreationDate { get; set; }

	///	<summary>
	///		Sets <see cref="IdentityUser{TKey}.PasswordHash"/> for this
	///		<see cref="Pbkdf2IdentityUser{TKey}"/> with the derived result
	///		of <see cref="KeyDerivation.Pbkdf2"/>.
	///	</summary>
	///	<param name="password">Clear text password.</param>
	public void SetPassword(string password)
	{
		PasswordHash = DeriveKey(password);
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
			GetSalt(),
			KeyDerivationPrf.HMACSHA256,
			Pbkdf2IdentityUser.IterationCount,
			Pbkdf2IdentityUser.HashedSize
		);
		return Convert.ToBase64String(hashed);
	}

	///	<summary>
	///		Gets the salt for this <see cref="Pbkdf2IdentityUser{TKey}"/>
	///	</summary>
	public byte[] GetSalt()
	{
		using (var sha = SHA256.Create()) {
			return sha.ComputeHash(BitConverter.GetBytes(CreationDate.ToBinary()));
		}
	}
}
