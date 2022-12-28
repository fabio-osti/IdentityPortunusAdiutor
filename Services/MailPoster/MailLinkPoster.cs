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

	public TUser ConsumeMessage(
		string message, 
		MessageType messageType,
		TUser? user
	)
	{
		// Validates token
		var userId = _tokenBuilder
			.ValidateSpecialToken(
				message,
				messageType.ToJwtString(),
				out _
			)?
			.First(e => e.Type == ClaimTypes.PrimarySid)
			.Value;

		// Gets user referenced by the token
		user = _context.Users.First(e => e.Id.ToString() == userId);

		// Checks if token have been already used.
		var userToken = _context.UserTokens.FirstOrDefault(
			e => 
				e.Value == message 
				&& e.UserId.ToString() == user.Id.ToString()
		);
		if (userToken != null)
		{
			throw new SecurityTokenException("Token already consumed.");
		}

		// Saves token as used on the Db
		userToken = new IdentityUserToken<TKey>
		{
			UserId = user.Id,
			LoginProvider = "token-special-access",
			Name = $"{messageType.ToJwtString()}:{DateTime.UtcNow.ToString()}",
			Value = message
		} as TUserToken;
		if (userToken == null)
		{
			throw new InvalidCastException($"{nameof(IdentityUserToken<TKey>)} is not a {nameof(TUserToken)}.");
		}
		_context.UserTokens.Add(userToken);
		_context.SaveChanges();

		return user;
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