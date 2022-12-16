using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;

namespace TokenAuthenticationHelper;

/// <summary>
/// 	An implementation of <see cref="IdentityUser{TKey}"/>
/// 	that uses the Pbkdf2 salt and hash algorithim to
///		store the password.
/// </summary>
public class IdentityUserPbkdf2<TKey> : IdentityUser<TKey>
	where TKey : IEquatable<TKey>
{
	private const int IterationCount = UInt16.MaxValue;
	private const int HashedSize = 128;

	/// <summary>
	/// 	UTC <see cref="DateTime"/> of this 
	/// 	<see cref="IdentityUserPbkdf2{TKey}"/> creation.
	/// </summary>
	public DateTime CreationDate { get; set; }

	/// <summary>
	/// 	Sets <see cref="IdentityUser{TKey}.PasswordHash"/> for this
	///		<see cref="IdentityUserPbkdf2{TKey}"/> with the derived.
	/// </summary>
	/// <param name="password">Clear text password.</param>
	public void SetPassword(
		string password
	)
	{
		PasswordHash = DeriveKey(password);
	}

	/// <summary>
	/// 	Check if <paramref name="password"/> is valid for this 
	/// 	<see cref="IdentityUserPbkdf2{TKey}"/>.
	/// </summary>
	/// <param name="password"></param>
	/// <returns></returns>
	public bool ValidatePassword(
		string password
	)
	{
		return PasswordHash == DeriveKey(password);
	}

	private string DeriveKey(string pswrd)
	{
		var hashed = KeyDerivation.Pbkdf2(
			pswrd,
			Salt,
			KeyDerivationPrf.HMACSHA256,
			IterationCount,
			HashedSize
		);
		return Convert.ToBase64String(hashed);
	}

	/// <summary>
	/// 	Gets the salt for this <see cref="IdentityUserPbkdf2{TKey}"/>
	/// </summary>
	public byte[] Salt
	{
		get
		{
			using (var sha = SHA256.Create())
			{
				return sha.ComputeHash(
					BitConverter.GetBytes(
						CreationDate.ToBinary()));
			}
		}
	}
}
