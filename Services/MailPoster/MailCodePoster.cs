using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

using MailKit.Net.Smtp;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using PortunusAdiutor.Data;
using PortunusAdiutor.Models;
using PortunusAdiutor.Services.MailPoster;
using PortunusAdiutor.Services.TokenBuilder;

public class MailCodePoster<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : MailPosterBase<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> ,IMailPoster<TUser, TKey>
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
	MailCodePosterParams _posterParams;
	ITokenBuilder _tokenBuilder;

	public MailCodePoster(MailCodePosterParams posterParams, TContext context, ITokenBuilder tokenBuilder) : base(context)
	{
		_posterParams = posterParams;
		_tokenBuilder = tokenBuilder;
	}

	public void SendEmailConfirmationMessage(TUser user)
	{
		ArgumentNullException.ThrowIfNullOrEmpty(user.Email);

		var otp = GenAndSave(user.Id, MessageTypes.EmailConfirmation);

		var message = _posterParams.EmailConfirmationMessageBuilder(
			user.Email,
			otp.Password
		);

		SendMessage(message);
	}

	public void SendPasswordRedefinitionMessage(TUser user)
	{
		ArgumentNullException.ThrowIfNullOrEmpty(user.Email);

		var otp = GenAndSave(user.Id, MessageTypes.PasswordRedefinition);

		var message = _posterParams.PasswordRedefinitionMessageBuilder(
			user.Email,
			otp.Password
		);

		SendMessage(message);
	}

	private void SendMessage(MimeMessage message)
	{
		using (var client = new SmtpClient())
		{
			client.Connect(_posterParams.SmtpUri);
			client.Authenticate(_posterParams.SmtpCredentials);
			client.Send(message);
			client.Disconnect(true);
		}
	}
}