using Microsoft.AspNetCore.Identity;
namespace PortunusAdiutor.Models;

/// <summary>
/// 	Class representing a single use password for special access.
/// </summary>
/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
public class UserOneTimePassword<TUser, TKey>
where TUser : IdentityUser<TKey>
where TKey : IEquatable<TKey>
{
	/// <summary>
	///  	The user this <see cref="UserOneTimePassword{TUser, TKey}"/> gives access.
	/// </summary>
	public TUser? User { get; init; }
	/// <summary>
	/// 	The primary key of the user this <see cref="UserOneTimePassword{TUser, TKey}"/> gives access.
	/// </summary>
	public required TKey UserId { get; init; }
	/// <summary>
	/// 	The one use password.
	/// </summary>
	public required string Password { get; init; }
	/// <summary>
	///  The type of access given by this <see cref="UserOneTimePassword{TUser, TKey}"/>.
	/// </summary>
	public required string Type { get; init; }
	/// <summary>
	///  Expiration <see cref="DateTime"/>.
	/// </summary>
	public required DateTime ExpiresOn { get; init; }
	/// <summary>
	/// 	If this <see cref="UserOneTimePassword{TUser, TKey}"/> has already been used.
	/// </summary>
	public bool Consumed { get; set; } = false;
}