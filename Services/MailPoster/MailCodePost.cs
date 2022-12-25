using Microsoft.AspNetCore.Identity;

/// <summary>
/// 	Class representing a code sent to the user for some special action.
/// </summary>
/// <typeparam name="TUser">Type of <see cref="IdentityUser{TKey}"/> used by the identity system.</typeparam>
/// <typeparam name="TKey">The type used for the primary key for the <typeparamref name="TUser"/>.</typeparam>
public class MailCodePost<TUser, TKey>
where TUser : IdentityUser<TKey>
where TKey : IEquatable<TKey>
{
	/// <summary>
	/// 	The <typeparamref name="TUser"/> to whom this <see cref="Code"/> was sent.
	/// </summary>
	public TUser? User { get; init; }
	/// <summary>
	/// 	The <typeparamref name="TUser"/> primary key.
	/// </summary>
	public required TKey UserId { get; init; }
	/// <summary>
	/// 	The <see cref="string"/> this <see cref="User"/> should use.
	/// </summary>
	public required string Code { get; init; }
	/// <summary>
	/// 	The <see cref="MessageType"/> in which this code was sent.  
	/// </summary>
	public required MessageType Type { get; init; }
	/// <summary>
	/// 	Should not be consumed after this <see cref="DateTime"/>.
	/// </summary>
	public required DateTime ExpiresOn { get; init; }
	/// <summary>
	/// 	If this code has already been consumed.
	/// </summary>
	public bool Consumed { get; set; } = false;
}