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
where TContext : IdentityWithSutDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
where TUser : IdentityUser<TKey>, IManagedUser
where TRole : IdentityRole<TKey>
where TKey : IEquatable<TKey>
where TUserClaim : IdentityUserClaim<TKey>
where TUserRole : IdentityUserRole<TKey>
where TUserLogin : IdentityUserLogin<TKey>
where TRoleClaim : IdentityRoleClaim<TKey>
where TUserToken : IdentityUserToken<TKey>
{
	ITokenBuilder _tokenBuilder;
	IMessagePoster<TUser, TKey> _mailPoster;
	TContext _context;

	public UsersManager(
		ITokenBuilder tokenBuilder,
		IMessagePoster<TUser, TKey> mailPoster,
		TContext context
	)
	{
		_tokenBuilder = tokenBuilder;
		_mailPoster = mailPoster;
		_context = context;
	}

	public Task<TUser> CreateUser(Expression<Func<TUser, bool>> userFinder, Func<TUser> userBuilder)
	{
		if (_context.Users.FirstOrDefault(userFinder) is not null) {
			return Task.FromException<TUser>(new UserAlreadyExistsException());
		}

		var user = _context.Users.Add(userBuilder()).Entity;
		_mailPoster.SendEmailConfirmationMessage(user);
		_context.SaveChanges();
		return Task.FromResult(user);
	}

	public Task<TUser> ValidateUser(Expression<Func<TUser, bool>> userFinder, string userPassword)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		if (user is null) {
			return Task.FromException<TUser>(new UserNotFoundException());
		}

		if (!user.ValidatePassword(userPassword)) {
			return Task.FromException<TUser>(new UnauthorizedAccessException());
		}

		return Task.FromResult(user);
	}

	public Task<TUser> SendEmailConfirmation(Expression<Func<TUser, bool>> userFinder)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		if (user is null) {
			return Task.FromException<TUser>(new UserNotFoundException());
		}

		if (user.EmailConfirmed) {
			return Task.FromException<TUser>(new UnauthorizedAccessException());
		}

		_mailPoster.SendPasswordRedefinitionMessage(user);

		return Task.FromResult(user);
	}

	public Task<TUser> ConfirmEmail(
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

		return Task.FromResult(user);
	}

	public Task<TUser> SendPasswordRedefinition(Expression<Func<TUser, bool>> userFinder)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		if (user is null) {
			return Task.FromException<TUser>(new UserNotFoundException());
		}

		_mailPoster.SendPasswordRedefinitionMessage(user);

		return Task.FromResult(user);
	}

	public Task<TUser> RedefinePassword(
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

		return Task.FromResult(user);
	}
}