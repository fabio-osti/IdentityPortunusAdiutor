using System.Linq.Expressions;



using PortunusAdiutor.Models;

namespace PortunusAdiutor.Services.UsersManager;

/// <summary>
/// 	Manages users on the database context.
/// </summary>
/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
public interface IUsersManager<TUser, TKey>
where TUser : class, IManagedUser<TUser, TKey>
where TKey : IEquatable<TKey>
{
	/// <summary>
	/// 	Creates an user.
	/// </summary>
	/// <param name="userFinder">Predicate for finding duplicate user.</param>
	/// <param name="userBuilder">Builder of the user.</param>
	/// <returns>Created user.</returns>
	TUser CreateUser(Expression<Func<TUser, bool>> userFinder, Func<TUser> userBuilder);
	/// <summary>
	/// 	Validates an user.
	/// </summary>
	/// <param name="userFinder">Predicate for finding the user.</param>
	/// <param name="userPassword">Plain text password to be validated.</param>
	/// <returns>Validated user.</returns>
	TUser ValidateUser(Expression<Func<TUser, bool>> userFinder, string userPassword);
	/// <summary>
	/// 	Sends a message to an user for email confirmation.
	/// </summary>
	/// <param name="userFinder">Predicate for finding the user.</param>
	/// <returns>Validated user.</returns>
	TUser SendEmailConfirmation(Expression<Func<TUser, bool>> userFinder);
	/// <summary>
	/// 	Confirm the email of the user to whom this <paramref name="token"/> belongs to.
	/// </summary>
	/// <param name="token">Token for the action.</param>
	/// <returns>User that had his email confirmed.</returns>
	TUser ConfirmEmail(string token);
	/// <summary>
	/// 	Sends a message to an user for password redefinition.
	/// </summary>
	/// <param name="userFinder">Predicate for finding the user.</param>
	/// <returns>Validated user.</returns>
	TUser SendPasswordRedefinition(Expression<Func<TUser, bool>> userFinder);
	/// <summary>
	/// 	Redefines the password of the user to whom this <paramref name="token"/> belongs to.
	/// </summary>
	/// <param name="token">Token for the action.</param>
	/// <param name="newPassword">Password to be set.</param>
	/// <returns>User that had his password redefined.</returns>
	TUser RedefinePassword(string token, string newPassword);
}