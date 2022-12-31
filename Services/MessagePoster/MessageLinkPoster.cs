using System.Security.Claims;
using System.Security.Cryptography;

using MailKit.Net.Smtp;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using MimeKit;

using PortunusAdiutor.Data;
using PortunusAdiutor.Models;
using PortunusAdiutor.Services.TokenBuilder;

namespace PortunusAdiutor.Services.MessagePoster;

public class MessageLinkPoster<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : MessagePosterBase<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IMessagePoster<TUser, TKey>
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
	private readonly MessageLinkPosterParams _posterParams;
	private readonly ITokenBuilder _tokenBuilder;
	public MessageLinkPoster(
		MessageLinkPosterParams posterParams,
		TContext context,
		ITokenBuilder tokenBuilder
	) : base(context)
	{
		_posterParams = posterParams;
		_tokenBuilder = tokenBuilder;
	}

	public void SendEmailConfirmationMessage(TUser user)
	{
		// Generates OTP
		var otp = GenAndSave(user.Id, MessageTypes.EmailConfirmation);
		// Builds token containing OTP
		var token = _tokenBuilder.BuildSpecialToken(
			new ClaimsIdentity(new[] {
			new Claim(ClaimTypes.PrimarySid, otp.UserId.ToString()!),
			new Claim(JwtCustomClaims.XDigitsCode, otp.Password)
			}),
			otp.Type,
			otp.ExpiresOn,
			true
		);
		// Builds and sends message
		ArgumentException.ThrowIfNullOrEmpty(user.Email);
		var message = _posterParams.PasswordRedefinitionMessageBuilder(
			user.Email,
			_posterParams.PasswordRedefinitionEndpoint + token
		);
		SendMessage(message);
	}

	public void SendPasswordRedefinitionMessage(TUser user)
	{
		// Generates OTP
		var otp = GenAndSave(user.Id, MessageTypes.PasswordRedefinition);
		// Builds token containing OTP
		var token = _tokenBuilder.BuildSpecialToken(
			new ClaimsIdentity(new[] {
			new Claim(ClaimTypes.PrimarySid, otp.UserId.ToString()!),
			new Claim(JwtCustomClaims.XDigitsCode, otp.Password)
			}),
			otp.Type,
			otp.ExpiresOn,
			true
		);
		// Builds and sends message
		ArgumentException.ThrowIfNullOrEmpty(user.Email);
		var message = _posterParams.PasswordRedefinitionMessageBuilder(
			user.Email,
			_posterParams.PasswordRedefinitionEndpoint + token
		);
		SendMessage(message);
	}

	private void SendMessage(MimeMessage message)
	{
		using (var client = new SmtpClient()) {
			client.Connect(_posterParams.SmtpUri);
			client.Authenticate(_posterParams.SmtpCredentials);
			client.Send(message);
			client.Disconnect(true);
		}
	}
}