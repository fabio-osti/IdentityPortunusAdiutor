using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace PortunusAdiutor.Models;

/// <summary>
/// 	Defines all necessary methods for managing an user.
/// </summary>
public interface IManagedUser<TUser, TKey>
where TUser : IdentityUser<TKey>
where TKey : IEquatable<TKey>
{
	/// <summary>
	/// 	Sets an user password to <paramref name="password"/>.
	/// </summary>
	/// <param name="password">Plain text password.</param>
	[MemberNotNull(nameof(Salt))]
	void SetPassword(string password);
	/// <summary>
	/// 	Validates an user password to <paramref name="password"/>.
	/// </summary>
	/// <param name="password">Plain text password.</param>
	bool ValidatePassword(string password);
	/// <summary>
	/// 	Gets or sets the salt used by <see cref="SetPassword(string)"/> and <see cref="ValidatePassword(string)"/>.
	/// </summary>
	byte[] Salt { get; set; }
	/// <summary>
	/// 	Gets a collection of this user <see cref="Claim"/>.
	/// </summary>
	/// <returns>An <see cref="Array"/> where every element is an user <see cref="Claim"/>.</returns>
	Claim[] GetClaims();
	ICollection<SingleUseToken<TUser, TKey>>? SingleUseTokens { get; set; }
}