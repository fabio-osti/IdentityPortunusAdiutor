using System.Linq.Expressions;

using Microsoft.AspNetCore.Identity;

using PortunusAdiutor.Models;

namespace PortunusAdiutor.Services.UsersManager;

/// <summary>
/// 	Defines all necessary methods for managing the users.
/// </summary>
/// <typeparam name="TUser">Type of users to be managed.</typeparam>
/// <typeparam name="TKey">Type of key used by the users.</typeparam>
public interface IUsersManager<TUser, TKey>
where TUser : IdentityUser<TKey>, IManagedUser
where TKey : IEquatable<TKey>
{
	/// <summary>
	/// 	Creates an user after checking if it doesn't already exists.
	/// </summary>
	/// <param name="userFinder">Predicate to check for existing user.</param>
	/// <param name="userBuilder">Builder of an user.</param>
	/// <returns>A Task with the created user on success, a Task with the exception causing the validation failure else.</returns>
	Task<TUser> CreateUser(Expression<Func<TUser, bool>> userFinder, Func<TUser> userBuilder);
	/// <summary>
	/// 	Validates the credentials for an user.
	/// </summary>
	/// <param name="userFinder">Predicate to find the user.</param>
	/// <param name="userPassword">Plain text password to be validated.</param>
	/// <returns>A Task with the validate user on success, a Task with the exception causing the validation failure else.</returns>
	Task<TUser> ValidateUser(Expression<Func<TUser, bool>> userFinder, string userPassword);
	/// <summary>
	/// 	Sends an email confirmation message to an user.
	/// </summary>
	/// <param name="userFinder">Predicate to find the user to whom the message will be sent.</param>
	/// <returns>A Task with the user who recieved the message on success, a Task with the exception causing the validation failure else.</returns>
	Task<TUser> SendEmailConfirmation(Expression<Func<TUser, bool>> userFinder);
	/// <summary>
	/// 	Confirms the email of an user.
	/// </summary>
	/// <param name="otp">One-Time Password to validate action.</param>
	/// <param name="userFinder">Predicate to find the user who will have the email confirmed.</param>
	/// <returns>A Task with the user who had his email confirmed on success, a Task with the exception causing the validation failure else.</returns>
	Task<TUser> ConfirmEmail(string otp, Expression<Func<TUser, bool>>? userFinder);
	/// <summary>
	/// 	Sends an password redefiniton message to an user.
	/// </summary>
	/// <param name="userFinder">Predicate to find the user to whom the message will be sent.</param>
	/// <returns>A Task with the user who recieved the message on success, a Task with the exception causing the validation failure else.</returns>
	Task<TUser> SendPasswordRedefinition(Expression<Func<TUser, bool>> userFinder);
	/// <summary>
	/// 	Redefines the email of an user.
	/// </summary>
	/// <param name="otp">One-Time Password to validate action.</param>
	/// <param name="newPassword">Plain text password to be defined.</param>
	/// <param name="userFinder">Predicate to find the user who will have the email confirmed.</param>
	/// <returns>A Task with the user who had his password redefined on success, a Task with the exception causing the validation failure else.</returns>
	Task<TUser> RedefinePassword(string otp, string newPassword, Expression<Func<TUser, bool>>? userFinder);
	/// <summary>
	/// 	Helper that validates a token and retrieves the user id and otp contained in.
	/// </summary>
	/// <param name="token">The token that will be validated.</param>
	/// <param name="tokenType">The type of access the otp contained by this token will provide.</param>
	/// <returns>A <see cref="Tuple"/> with the user id as <see cref="Tuple{T1, T2}.Item1"/> and the OTP as <see cref="Tuple{T1, T2}.Item2"/>.</returns>
	Tuple<string, string> GetOtpCredentialsFromToken(string token, string tokenType);
}