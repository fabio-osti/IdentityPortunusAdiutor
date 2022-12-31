using System.Security.Cryptography;

using Microsoft.AspNetCore.Identity;

using PortunusAdiutor.Data;
using PortunusAdiutor.Models;

namespace PortunusAdiutor.Services.MessagePoster;

/// <summary>
/// 	Defines a common method for OTP generation and consumption.
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TUser"></typeparam>
/// <typeparam name="TRole"></typeparam>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TUserClaim"></typeparam>
/// <typeparam name="TUserRole"></typeparam>
/// <typeparam name="TUserLogin"></typeparam>
/// <typeparam name="TRoleClaim"></typeparam>
/// <typeparam name="TUserToken"></typeparam>
public class MessagePosterBase<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
where TContext : OtpIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
where TUser : IdentityUser<TKey>
where TRole : IdentityRole<TKey>
where TKey : IEquatable<TKey>
where TUserClaim : IdentityUserClaim<TKey>
where TUserRole : IdentityUserRole<TKey>
where TUserLogin : IdentityUserLogin<TKey>
where TRoleClaim : IdentityRoleClaim<TKey>
where TUserToken : IdentityUserToken<TKey>
{
	TContext _context;

	/// <summary>
	/// 	Initializes an instance of the class.
	/// </summary>
	/// <param name="context">Database context</param>
	public MessagePosterBase(TContext context)
	{
		_context = context;
	}

	/// <summary>
	/// 	Generates an OTP for an <see cref="IdentityUser{TKey}"/> for an access of type <paramref name="type"/> and saves it on the database.
	/// </summary>
	/// <param name="userId">Id of the <see cref="IdentityUser{TKey}"/>.</param>
	/// <param name="type">Type of access granted by the the returning OTP.</param>
	/// <returns>The OTP.</returns>
	protected UserOneTimePassword<TUser, TKey> GenAndSave(TKey userId, string type)
	{
		var sixDigitCode = RandomNumberGenerator.GetInt32(1000000).ToString("000000");

		var userOtp = new UserOneTimePassword<TUser, TKey>()
		{
			User = _context.Users.Find(userId),
			UserId = userId,
			Password = sixDigitCode,
			ExpiresOn = DateTime.UtcNow.AddMinutes(15),
			Type = type
		};

		_context.UserOtps.Add(userOtp);
		_context.SaveChanges();

		return userOtp;
	}

	/// <summary>
	/// 	Consumes a sent message.
	/// </summary>
	/// <param name="userId">The <see cref="IdentityUser{TKey}.Id"/> of the message receiver.</param>
	/// <param name="otp">The access key sent by the message.</param>
	/// <param name="messageType">The type of message that was sent.</param>
	/// <returns>True if successful, else false.</returns>
	public bool ConsumeOtp(
		TKey? userId,
		string otp,
		MessageType messageType
	)
	{
		ArgumentNullException.ThrowIfNull(userId);
		var type = messageType.ToJwtTypeString();
		var userOtp =
			_context.UserOtps
				.Where(e => e.UserId.Equals(userId))
				.Where(e => e.Type == type)
				.FirstOrDefault(e => e.Password == otp);

		if (userOtp is null) {
			throw new UnauthorizedAccessException("One Use Password not found.");
		}

		if (userOtp.ExpiresOn < DateTime.UtcNow) {
			throw new UnauthorizedAccessException("Token already expired.");
		}

		var code = _context.UserOtps.Remove(userOtp);
		_context.SaveChanges();

		return true;
	}
}