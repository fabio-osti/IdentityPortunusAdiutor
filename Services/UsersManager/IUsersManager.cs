using System.Linq.Expressions;

using Microsoft.AspNetCore.Identity;

using PortunusAdiutor.Models;

namespace PortunusAdiutor.Services.UsersManager;

/// <summary>
/// 	An interface for a service that manages a <typeparamref name="TUser"/>.
/// </summary>
/// <typeparam name="TUser">Type of <see cref="IdentityUser{TKey}"/> used by the identity system.</typeparam>
/// <typeparam name="TRole">Type of <see cref="IdentityRole{TKey}"/> used by the identity system</typeparam>
/// <typeparam name="TKey">The type used for the primary key for the <typeparamref name="TUser"/>.</typeparam>
public interface IUsersManager<TUser, TRole, TKey>
where TUser : IdentityUser<TKey>, IManagedUser
where TRole : IdentityRole<TKey>
where TKey : IEquatable<TKey>
{
	/// <summary>
	/// 	Creates a <typeparamref name="TUser"/> with 
	/// 	<paramref name="userBuilder"/> after checking that it
	/// 	won't collide with any <typeparamref name="TUser"/> 
	/// 	found by the <paramref name="userFinder"/>.
	/// </summary>
	/// <param name="userFinder">Predicate for <typeparamref name="TUser"/> collision.</param>
	/// <param name="userBuilder"><typeparamref name="TUser"/> builder function.</param>
	/// <returns>The user if created, null else.</returns>
	TUser? CreateUser(Expression<Func<TUser, bool>> userFinder, Func<TUser> userBuilder);
	/// <summary>
	/// 	Validates <typeparamref name="TUser"/> found by <paramref name="userFinder"/>
	/// 	with <see cref="IManagedUser.ValidatePassword(string)"/>.
	/// </summary>
	/// <param name="userFinder">Predicate for finding an <typeparamref name="TUser"/>.</param>
	/// <param name="userPassword">Clear text password to be validated.</param>
	/// <returns>The user if validated, null else.</returns>
	TUser? ValidateUser(Expression<Func<TUser, bool>> userFinder, string userPassword);
	/// <summary>
	/// 	Sends a message to the <typeparamref name="TUser"/> found by
	/// 	<paramref name="userFinder"/> for email confirmation.
	/// </summary>
	/// <param name="userFinder">Predicate for finding an <typeparamref name="TUser"/>.</param>
	/// <returns>The user if message sent, null else.</returns>
	TUser? SendEmailConfirmation(Expression<Func<TUser, bool>> userFinder);
	/// <summary>
	/// 	Confirms the email of the <typeparamref name="TUser"/> found by
	/// 	<paramref name="userFinder"/>.
	/// </summary>
	/// <param name="userFinder"></param>
	/// <returns>The user if confirmed, null else</returns>
	TUser? ConfirmEmail(Expression<Func<TUser, bool>> userFinder);
	/// <summary>
	/// 	Sends a message to the <typeparamref name="TUser"/> found by
	/// 	<paramref name="userFinder"/> for password redefinition.
	/// </summary>
	/// <param name="userFinder">Predicate for finding an <typeparamref name="TUser"/>.</param>
	/// <returns>The user if message sent, null else.</returns>
	TUser? SendPasswordRedefinition(Expression<Func<TUser, bool>> userFinder);
	/// <summary>
	/// 	Redefines the password of the <typeparamref name="TUser"/> found by
	/// 	<paramref name="userFinder"/>.
	/// </summary>
	/// <param name="userFinder">Predicate for finding an <typeparamref name="TUser"/>.</param>
	/// <param name="newPassword"><paramref name="userFinder"/> result new password.</param>
	/// <returns>The user if redefined, null else</returns>
	TUser? RedefinePassword(Expression<Func<TUser, bool>> userFinder, string newPassword);
}