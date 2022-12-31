using System.Linq.Expressions;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using PortunusAdiutor.Data;
using PortunusAdiutor.Exceptions;
using PortunusAdiutor.Models;
using PortunusAdiutor.Services.MessagePoster;
using PortunusAdiutor.Services.UsersManager;
using PortunusAdiutor.Services.TokenBuilder;

/// <summary>
/// 	Implementation of <see cref="IUsersManager{TUser, TKey}"/>.
/// </summary>
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
	IMessagePoster<TUser, TKey> _messagePoster;
	TContext _context;

	/// <summary>
	/// 	Initializes an instance of the class.
	/// </summary>
	/// <param name="messagePoster">Message poster.</param>
	/// <param name="context">Database context.</param>
	/// <param name="tokenBuilder">JWT builder.</param>
	public UsersManager(
		ITokenBuilder tokenBuilder,
		IMessagePoster<TUser, TKey> messagePoster,
		TContext context
	)
	{
		_tokenBuilder = tokenBuilder;
		_messagePoster = messagePoster;
		_context = context;
	}

	/// <inheritdoc/>
	public Task<TUser> CreateUser(Expression<Func<TUser, bool>> userFinder, Func<TUser> userBuilder)
	{
		if (_context.Users.FirstOrDefault(userFinder) is not null) {
			return Task.FromException<TUser>(new UserAlreadyExistsException());
		}

		var user = _context.Users.Add(userBuilder()).Entity;
		_messagePoster.SendEmailConfirmationMessage(user);
		_context.SaveChanges();
		return Task.FromResult(user);
	}

	/// <inheritdoc/>
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

	/// <inheritdoc/>
	public Task<TUser> SendEmailConfirmation(Expression<Func<TUser, bool>> userFinder)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		if (user is null) {
			return Task.FromException<TUser>(new UserNotFoundException());
		}

		if (user.EmailConfirmed) {
			return Task.FromException<TUser>(new UnauthorizedAccessException());
		}

		_messagePoster.SendPasswordRedefinitionMessage(user);

		return Task.FromResult(user);
	}

	/// <inheritdoc/>
	public Task<TUser> ConfirmEmail(
		string otp,
		Expression<Func<TUser, bool>>? userFinder
	)
	{
		var user = userFinder is null
			? null
			: _context.Users.FirstOrDefault(userFinder);
		if (user is null) {
			return Task.FromException<TUser>(new UserNotFoundException());
		}

		if (
			!_messagePoster.ConsumeOtp(
				user.Id,
				otp,
				MessageType.EmailConfirmation
			)
		) {
			return Task.FromException<TUser>(new UnauthorizedAccessException());
		}

		user.EmailConfirmed = true;
		_context.SaveChanges();

		return Task.FromResult(user);
	}

	/// <inheritdoc/>
	public Task<TUser> SendPasswordRedefinition(Expression<Func<TUser, bool>> userFinder)
	{
		var user = _context.Users.FirstOrDefault(userFinder);
		if (user is null) {
			return Task.FromException<TUser>(new UserNotFoundException());
		}

		_messagePoster.SendPasswordRedefinitionMessage(user);

		return Task.FromResult(user);
	}

	/// <inheritdoc/>
	public Task<TUser> RedefinePassword(
		string otp,
		string newPassword,
		Expression<Func<TUser, bool>>? userFinder
	)
	{
		var user = userFinder is null
			? null
			: _context.Users.FirstOrDefault(userFinder);
		if (user is null) {
			return Task.FromException<TUser>(new UserNotFoundException());
		}

		if (
			!_messagePoster.ConsumeOtp(
				user.Id,
				otp,
				MessageType.PasswordRedefinition
			)
		) {
			return Task.FromException<TUser>(new UnauthorizedAccessException());
		}

		user.SetPassword(newPassword);
		_context.SaveChanges();

		return Task.FromResult(user);
	}

	/// <inheritdoc/>
	public Tuple<string, string> GetOtpCredentialsFromToken(
		string token,
		string tokenType
	)
	{
		var validationParameters = new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidTypes = new List<string> { tokenType },
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
		};

		var claims = _tokenBuilder.ValidateToken(token, out var validated, validationParameters);
		ArgumentNullException.ThrowIfNull(claims);
		var id = claims.First(e => e.Type == ClaimTypes.PrimarySid).Value;
		var xdc = claims.First(e => e.Type == JwtCustomClaims.XDigitsCode).Value;
		return new(id, xdc);
	}
}