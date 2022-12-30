using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using PortunusAdiutor.Models;

public class MailPosterBase<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
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

	public MailPosterBase(TContext context)
	{
		_context = context;
	}


	// OTP stuff
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

		if (userOtp is null)
		{
			throw new UnauthorizedAccessException("One Use Password not found.");
		}

		if (userOtp.ExpiresOn < DateTime.UtcNow)
		{
			throw new UnauthorizedAccessException("Token already expired.");
		}

		var code = _context.UserOtps.Remove(userOtp);
		_context.SaveChanges();

		return true;
	}
}