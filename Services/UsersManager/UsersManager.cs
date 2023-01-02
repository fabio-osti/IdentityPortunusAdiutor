using System.Linq.Expressions;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using PortunusAdiutor.Data;
using PortunusAdiutor.Exceptions;
using PortunusAdiutor.Models;
using PortunusAdiutor.Services.MessagePoster;
using PortunusAdiutor.Services.TokenBuilder;
using PortunusAdiutor.Services.UsersManager;

/// <summary>
/// 	Default implementation of <see cref="IUsersManager{TUser, TKey}"/>.
/// </summary>
/// <typeparam name="TContext">Represents an Entity Framework database context used for identity.</typeparam>
/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
/// <typeparam name="TRole">Represents a role in the identity system.</typeparam>
/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
/// <typeparam name="TUserClaim">Represents a claim possessed by an user.</typeparam>
/// <typeparam name="TUserRole">Represents the link between an user and a role.</typeparam>
/// <typeparam name="TUserLogin">Represents a login and its associated provider for an user.</typeparam>
/// <typeparam name="TRoleClaim">Represents a claim that is granted to all users within a role.</typeparam>
/// <typeparam name="TUserToken">Represents an authentication token for an user.</typeparam>	
public class UsersManager<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IUsersManager<TUser, TKey>
where TContext : ManagedIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
where TUser : IdentityUser<TKey>, IManagedUser<TUser, TKey>
where TRole : IdentityRole<TKey>
where TKey : IEquatable<TKey>
where TUserClaim : IdentityUserClaim<TKey>
where TUserRole : IdentityUserRole<TKey>
where TUserLogin : IdentityUserLogin<TKey>
where TRoleClaim : IdentityRoleClaim<TKey>
where TUserToken : IdentityUserToken<TKey>
{
	IMessagePoster<TUser, TKey> _mailPoster;
	TContext _context;

	/// <summary>
	/// 	Initializes an instance of the class.
	/// </summary>
	/// <param name="messagePoster">Service for sending the messages.</param>
	/// <param name="context">database context used for identity.</param>
	public UsersManager(
		IMessagePoster<TUser, TKey> messagePoster,
		TContext context
	)
	{
		_mailPoster = messagePoster;
		_context = context;
	}

	/// <inheritdoc/>
	public TUser CreateUser(Expression<Func<TUser, bool>> userFinder, Func<TUser> userBuilder)
	{
		if (_context.Users.FirstOrDefault(userFinder) is not null) {
			throw new UserAlreadyExistsException();
		}

		var user = _context.Users.Add(userBuilder()).Entity;
		_mailPoster.SendEmailConfirmationMessage(user);
		_context.SaveChanges();
		return user;
	}

	/// <inheritdoc/>
	public TUser ValidateUser(Expression<Func<TUser, bool>> userFinder, string userPassword)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		user = UserNotFoundException.ThrowIfUserNull<TUser, TKey>(user);

		if (!user.ValidatePassword(userPassword)) {
			throw new InvalidPasswordException();
		}

		return user;
	}

	/// <inheritdoc/>
	public TUser SendEmailConfirmation(Expression<Func<TUser, bool>> userFinder)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		user = UserNotFoundException.ThrowIfUserNull<TUser, TKey>(user);

		if (user.EmailConfirmed) {
			throw new EmailAlreadyConfirmedException();
		}

		_mailPoster.SendEmailConfirmationMessage(user);

		return user;
	}

	/// <inheritdoc/>
	public TUser ConfirmEmail(
		string token
	)
	{
		var userId = _mailPoster.ConsumeSut(
			token,
			MessageType.EmailConfirmation
		);
		var user = _context.Users.Find(userId);
		user = UserNotFoundException.ThrowIfUserNull<TUser, TKey>(user);
		user.EmailConfirmed = true;
		_context.SaveChanges();

		return user;
	}

	/// <inheritdoc/>
	public TUser SendPasswordRedefinition(Expression<Func<TUser, bool>> userFinder)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		user = UserNotFoundException.ThrowIfUserNull<TUser, TKey>(user);

		_mailPoster.SendPasswordRedefinitionMessage(user);

		return user;
	}

	/// <inheritdoc/>
	public TUser RedefinePassword(
		string token,
		string newPassword
	)
	{
		var userId = _mailPoster.ConsumeSut(
				token,
				MessageType.EmailConfirmation
		);
		var user = _context.Users.Find(userId);
		user = UserNotFoundException.ThrowIfUserNull<TUser, TKey>(user);
		user.SetPassword(newPassword);
		_context.SaveChanges();

		return user;
	}
}