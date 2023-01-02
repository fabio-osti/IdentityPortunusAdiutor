using System.Runtime.Serialization;
using Microsoft.AspNetCore.Identity;
using PortunusAdiutor.Models;

namespace PortunusAdiutor.Exceptions;

/// <summary>
/// 	Represents error that occur when an user tis not found.
/// </summary>
public class UserNotFoundException : PortunusException
{
	static TUser ThrowIfUserNull<TUser, TKey>(TUser? user) 
	where TUser : IdentityUser<TKey>, IManagedUser<TUser, TKey>
	where TKey : IEquatable<TKey>
	{
		return user ?? throw new UserNotFoundException();
	} 
}