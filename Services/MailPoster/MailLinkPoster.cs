using Microsoft.AspNetCore.Identity;

using MimeKit;
using MailKit.Net.Smtp;
using PortunusAdiutor.Services.MailPoster;
using PortunusAdiutor.Services.TokenBuilder;
using System.Security.Claims;
using PortunusAdiutor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public class MailLinkPoster<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IMailPoster<TUser, TKey>
where TContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
where TUser : IdentityUser<TKey>, IManagedUser
where TRole : IdentityRole<TKey>
where TKey : IEquatable<TKey>
where TUserClaim : IdentityUserClaim<TKey>
where TUserRole : IdentityUserRole<TKey>
where TUserLogin : IdentityUserLogin<TKey>
where TRoleClaim : IdentityRoleClaim<TKey>
where TUserToken : IdentityUserToken<TKey>
{
	private readonly MailLinkPosterParams _posterParams;
	private readonly TContext _context;
	private readonly ITokenBuilder _tokenBuilder;
	public MailLinkPoster(
		MailLinkPosterParams posterParams,
		TContext context,
		ITokenBuilder tokenBuilder
	)
	{
		_posterParams = posterParams;
		_tokenBuilder = tokenBuilder;
		_context = context;
	}

	public void ConsumeMessage(TUser user, string content, MessageType messageType)
	{
		_tokenBuilder.ValidateSpecialToken(
			content,
			messageType.ToJwtString(),
			out _
		);
		if (_context.UserTokens.FirstOrDefault(e => e.Value == content) != null)
		{
			throw new SecurityTokenException("Token already consumed.");
		}

		var userToken = new IdentityUserToken<TKey>
		{
			UserId = user.Id,
			LoginProvider = "token-special-access",
			Name = $"{messageType.ToJwtString()}:{DateTime.UtcNow.ToString()}",
			Value = content
		} as TUserToken;
		if (userToken == null)
		{
			throw new InvalidCastException($"{nameof(IdentityUserToken<TKey>)} is not a {nameof(TUserToken)}.");
		}
		_context.UserTokens.Add(userToken);
		_context.SaveChanges();
	}

	public void SendEmailConfirmationMessage(TUser user)
	{
		string token = _tokenBuilder.BuildSpecialToken(
			new ClaimsIdentity(new[] {
				new Claim(ClaimTypes.PrimarySid, user.Id.ToString()!)
			}),
			JwtCustomTypes.EmailConfirmation,
			DateTime.UtcNow.AddMinutes(15),
			true
		);
		var message = _posterParams.EmailConfirmationMessageBuilder(
			user.Email!,
			_posterParams.EmailConfirmationEndpoint + token
		);

		SendMessage(message);
	}

	public void SendPasswordRedefinitionMessage(TUser user)
	{
		string token = _tokenBuilder.BuildSpecialToken(
			new ClaimsIdentity(new[] {
				new Claim(ClaimTypes.PrimarySid, user.Id.ToString()!)
			}),
			JwtCustomTypes.PasswordRedefinition,
			DateTime.UtcNow.AddMinutes(15),
			true
		);
		var message = _posterParams.PasswordRedefinitionMessageBuilder(
			user.Email!,
			_posterParams.PasswordRedefinitionEndpoint + token
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