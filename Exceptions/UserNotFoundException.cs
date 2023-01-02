using System.Runtime.Serialization;

using PortunusAdiutor.Models;

namespace PortunusAdiutor.Exceptions;

/// <summary>
/// 	Represents error that occur when an user is not found.
/// </summary>
public class UserNotFoundException : PortunusException
{
	/// <summary>
	/// 	Initializes the exception.
	/// </summary>
	public UserNotFoundException() : base("User not found.") { }

	/// <summary>
	/// 	Checks if <paramref name="user"/> is null.
	/// </summary>
	/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
	/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
	/// <param name="user">User to be checked if null.</param>
	/// <returns>Not null asserted <paramref name="user"/>.</returns>
	/// <exception cref="UserNotFoundException"></exception>
	static public TUser ThrowIfUserNull<TUser, TKey>(TUser? user)
	where TUser : class, IManagedUser<TUser, TKey>
	where TKey : IEquatable<TKey>
	{
		return user ?? throw new UserNotFoundException();
	}
}