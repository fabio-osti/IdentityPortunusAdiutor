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

	public UsersManager(
		IMessagePoster<TUser, TKey> mailPoster,
		TContext context
	)
	{
		_mailPoster = mailPoster;
		_context = context;
	}

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

	public TUser ValidateUser(Expression<Func<TUser, bool>> userFinder, string userPassword)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		if (user is null) {
			throw new UserNotFoundException();
		}

		if (!user.ValidatePassword(userPassword)) {
			throw new UnauthorizedAccessException();
		}

		return user;
	}

	public TUser SendEmailConfirmation(Expression<Func<TUser, bool>> userFinder)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		if (user is null) {
			throw new UserNotFoundException();
		}

		if (user.EmailConfirmed) {
			throw new UnauthorizedAccessException();
		}

		_mailPoster.SendPasswordRedefinitionMessage(user);

		return user;
	}

	public TUser ConfirmEmail(
		string token
	)
	{
		var userId = _mailPoster.ConsumeSut(
			token,
			MessageType.EmailConfirmation
		);
		var user = _context.Users.Find(userId);

		user.EmailConfirmed = true;
		_context.SaveChanges();

		return user;
	}

	public TUser SendPasswordRedefinition(Expression<Func<TUser, bool>> userFinder)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		if (user is null) {
			throw new UserNotFoundException();
		}

		_mailPoster.SendPasswordRedefinitionMessage(user);

		return user;
	}

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

		user.SetPassword(newPassword);
		_context.SaveChanges();

		return user;
	}
}