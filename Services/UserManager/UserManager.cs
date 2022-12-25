using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using PortunusAdiutor.Models;
using PortunusAdiutor.Services.MailPoster;
using PortunusAdiutor.Services.TokenBuilder;
using PortunusAdiutor.Services.UserManager;

/// <summary>
/// 	Default implementaion of <see cref="IUserManager{TUser, TRole, TKey}"/>.
/// </summary>
/// <typeparam name="TContext">
/// 	Type of <see cref="IdentityDbContext{TUser, TRole, TKey}"/> used by the identity system.
/// </typeparam>
/// <typeparam name="TUser">Type of <see cref="IdentityUser{TKey}"/> used by the identity system.</typeparam>
/// <typeparam name="TRole">Type of <see cref="IdentityRole{TKey}"/> used by the identity system</typeparam>
/// <typeparam name="TKey">The type used for the primary key for the <typeparamref name="TUser"/>.</typeparam>
public class UserManager<TContext, TUser, TRole, TKey> : IUserManager<TUser, TRole, TKey>
where TContext : IdentityDbContext<TUser, TRole, TKey>
where TUser : IdentityUser<TKey>, IManagedUser
where TRole : IdentityRole<TKey>
where TKey : IEquatable<TKey>
{
	ITokenBuilder _tokenBuilder;
	IMailPoster<TUser, TKey> _mailPoster;
	TContext _context;
	/// <summary>
	/// 	Initiazlize a new instance of <see cref="UserManager{TContext, TUser, TRole, TKey}"/>
	/// </summary>
	/// <param name="tokenBuilder">Service to build the tokens needed.</param>
	/// <param name="mailPoster">Service to send messages.</param>
	/// <param name="context">Context service for database interactions.</param>
	public UserManager(
		ITokenBuilder tokenBuilder,
		IMailPoster<TUser, TKey> mailPoster,
		TContext context
	)
	{
		_tokenBuilder = tokenBuilder;
		_mailPoster = mailPoster;
		_context = context;
	}
	///	<inheritdoc/>
	public TUser? SendEmailConfirmation(Func<TUser, bool> userFinder)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		if (user == null) {
			return null;
		}
		_mailPoster.SendPasswordRedefinitionMessage(user);

		return user;
	}
	///	<inheritdoc/>
	public TUser? ConfirmEmail(Func<TUser, bool> userFinder)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		if (user == null) {
			return null;
		}

		user.EmailConfirmed = true;
		_context.SaveChanges();

		return user;
	}
	///	<inheritdoc/>
	public TUser? SendPasswordRedefinition(Func<TUser, bool> userFinder)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		if (user == null) {
			return null;
		}
		_mailPoster.SendPasswordRedefinitionMessage(user);

		return user;
	}
	///	<inheritdoc/>
	public TUser? RedefinePassword(Func<TUser, bool> userFinder, string newPassword)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		if (user == null) {
			return null;
		}

		user.SetPassword(newPassword);
		_context.SaveChanges();

		return user;
	}
	///	<inheritdoc/>
	public TUser? CreateUser(Func<TUser, bool> userFinder, Func<TUser> userBuilder)
	{
		if (_context.Users.FirstOrDefault(userFinder) != null) {
			return null;
		}
		var user = _context.Users.Add(userBuilder()).Entity;
		_mailPoster.SendEmailConfirmationMessage(user);
		_context.SaveChanges();
		return user;
	}
	///	<inheritdoc/>
	public TUser? ValidateUser(Func<TUser, bool> userFinder, string userPassword)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		if (user == null || !user.ValidatePassword(userPassword)) {
			return null;
		}

		return user;
	}

}