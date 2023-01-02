using PortunusAdiutor.Models;

namespace PortunusAdiutor.Exceptions;

/// <summary>
/// 	Represents error that occur when a <see cref="SingleUseToken{TUser, TKey}"/> is not found.
/// </summary>
public class SingleUseTokenNotFoundException : PortunusException
{
	/// <summary>
    /// 	Initializes the exception.
    /// </summary>
	public SingleUseTokenNotFoundException() : base("Token not found.") { }

	/// <summary>
	/// 	Checks if <paramref name="token"/> is null.
	/// </summary>
	/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
	/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
	/// <param name="token">Token to be checked if null.</param>
	/// <returns>Not null asserted <paramref name="token"/>.</returns>
	/// <exception cref="UserNotFoundException"></exception>
	public static SingleUseToken<TUser, TKey> ThrowIfUserNull<TUser, TKey>(SingleUseToken<TUser, TKey>? token)
	where TUser : class, IManagedUser<TUser, TKey>
	where TKey : IEquatable<TKey>
	{
		return token ?? throw new SingleUseTokenNotFoundException();
	}
}