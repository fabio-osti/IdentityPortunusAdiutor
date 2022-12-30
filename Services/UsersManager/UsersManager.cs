using System.Linq.Expressions;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using PortunusAdiutor.Models;
using PortunusAdiutor.Services.MailPoster;
using PortunusAdiutor.Services.TokenBuilder;
using PortunusAdiutor.Services.UsersManager;

public class UsersManager<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IUsersManager<TUser, TKey>
where TContext : OtpIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
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
	IMailPoster<TUser, TKey> _mailPoster;
	TContext _context;

	public UsersManager(
		ITokenBuilder tokenBuilder,
		IMailPoster<TUser, TKey> mailPoster,
		TContext context
	)
	{
		_tokenBuilder = tokenBuilder;
		_mailPoster = mailPoster;
		_context = context;
	}

	public TUser? CreateUser(Expression<Func<TUser, bool>> userFinder, Func<TUser> userBuilder)
	{
		if (_context.Users.FirstOrDefault(userFinder) is not null)
		{
			return null;
		}
		var user = _context.Users.Add(userBuilder()).Entity;
		_mailPoster.SendEmailConfirmationMessage(user);
		_context.SaveChanges();
		return user;
	}

	public TUser? ValidateUser(Expression<Func<TUser, bool>> userFinder, string userPassword)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		if (user is null || !user.ValidatePassword(userPassword))
		{
			return null;
		}

		return user;
	}

	public TUser? SendEmailConfirmation(Expression<Func<TUser, bool>> userFinder)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		if (user is null || user.EmailConfirmed)
		{
			return null;
		}
		_mailPoster.SendPasswordRedefinitionMessage(user);

		return user;
	}

	public TUser? ConfirmEmail(
		string otp,
		Expression<Func<TUser, bool>>? userFinder
	)
	{
		var user = userFinder is null
			? null
			: _context.Users.FirstOrDefault(userFinder);
		if (user is null)
		{
			return null;
		}
		
		if (
			!_mailPoster.ConsumeOtp(
				user.Id,
				otp,
				MessageType.EmailConfirmation
			)
		)
		{
			throw new UnauthorizedAccessException();
		}

		user.EmailConfirmed = true;
		_context.SaveChanges();

		return user;
	}

	public TUser? SendPasswordRedefinition(Expression<Func<TUser, bool>> userFinder)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		if (user is null)
		{
			return null;
		}
		_mailPoster.SendPasswordRedefinitionMessage(user);

		return user;
	}

	public TUser? RedefinePassword(
		string otp,
		string newPassword,
		Expression<Func<TUser, bool>>? userFinder
	)
	{
		var user = userFinder is null
			? null
			: _context.Users.FirstOrDefault(userFinder);
		if (
			user is null
				|| !_mailPoster.ConsumeOtp(
					user.Id,
					otp,
					MessageType.EmailConfirmation
				)
		)
		{
			return null;
		}

		user.SetPassword(newPassword);
		_context.SaveChanges();

		return user;
	}
}