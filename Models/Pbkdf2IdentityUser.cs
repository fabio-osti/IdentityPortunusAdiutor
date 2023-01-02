using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;

namespace PortunusAdiutor.Models;

/// <summary>
/// 	Implementation of <see cref="IManagedUser{TUser, TKey}"/> using PBKDF2 as derivation algorithm.
/// </summary>
/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
public class Pbkdf2IdentityUser<TUser, TKey> : IdentityUser<TKey>, IManagedUser<TUser, TKey>
where TUser : Pbkdf2IdentityUser<TUser, TKey>
where TKey : IEquatable<TKey>
{
	private const KeyDerivationPrf DefaultPrf = KeyDerivationPrf.HMACSHA512;
	private const int DefaultIterationCount = 262140;
	private const int DefaultHashedSize = 128;

	/// <summary>
	/// 	Initializes an instance of the class.
	/// </summary>
	/// <param name="email">Email of the user.</param>
	/// <param name="salt">Salt of the user.</param>
	/// <remarks>
	/// 	This constructor should only be used by EF to build an object representing an existing <see cref="Pbkdf2IdentityUser{TUser, TKey}"/>.
	/// 	</remarks>
	public Pbkdf2IdentityUser(string email, byte[] salt)
	{
		Email = email;
		Salt = salt;
	}

	/// <summary>
	/// 	Initializes na instance of the class.
	/// </summary>
	/// <param name="email">Email of the user.</param>
	/// <param name="password">Password of the user.</param>
	public Pbkdf2IdentityUser(string email, string password)
	{
		Email = email;
		SetPassword(password);
	}

	/// <inheritdoc/>
	[MemberNotNull(nameof(Salt))]
	public void SetPassword(string password)
	{
		Salt =
			SHA256.HashData(BitConverter.GetBytes(DateTime.UtcNow.ToBinary()));
		PasswordHash = DeriveKey(password);
		return;
	}

	/// <inheritdoc/>
	public bool ValidatePassword(string password)
	{
		return PasswordHash == DeriveKey(password);
	}

	private string DeriveKey(string password)
	{
		var hashed = KeyDerivation.Pbkdf2(
			password,
			Salt,
			DefaultPrf,
			DefaultIterationCount,
			DefaultHashedSize
		);
		return Convert.ToBase64String(hashed);
	}

	/// <inheritdoc/>
	public byte[] Salt { get; set; }

	/// <inheritdoc/>
	public sealed override string? Email { get; set; }

	/// <inheritdoc/>
	public ICollection<SingleUseToken<TUser, TKey>>? SingleUseTokens { get; set; }

	/// <inheritdoc/>
	public virtual Claim[] GetClaims()
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
