using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

using MailKit.Net.Smtp;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MimeKit;

using PortunusAdiutor.Models;
using PortunusAdiutor.Services.MailPoster;
using PortunusAdiutor.Services.TokenBuilder;

public class MailCodePoster<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IMailPoster<TUser, TKey>
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
	IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> _context;
	ITokenBuilder _tokenBuilder;

	public MailCodePoster(MailCodePosterParams posterParams, IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> context, ITokenBuilder tokenBuilder)
	{
		_posterParams = posterParams;
		_context = context;
		_tokenBuilder = tokenBuilder;
	}

	public void ConsumeMessage(TUser user, string message, MessageType messageType)
	{
		var typeFilter = 
			(TUserToken e) =>
				messageType.ToJwtString() 
					==
				new JwtSecurityTokenHandler()
					.ReadJwtToken(e.Value).Header.Typ;
		var messageFinder =
			(TUserToken e) =>
				message 
					== 
				_tokenBuilder
					.ValidateSpecialToken(
						e.Value!,
						messageType.ToJwtString(),
						false
					)?.First(t => t.Type == JwtCustomTypes.XDigitsCode)
					.Value;

		var userToken = _context.UserTokens
			// Gets all tokens of user
			.Where(e => e.UserId.Equals(user.Id))
			// Gets tokens of message type
			.Where(typeFilter)
			.FirstOrDefault(messageFinder);

		if (userToken == null) throw new UnauthorizedAccessException($"No message: \"{message}\" to be consumed");
		var code = _context.UserTokens.Remove(userToken);

	}

	public void SendEmailConfirmationMessage(TUser user)
	{
		var sixDigitsCode = GenAndSave(user.Id, JwtCustomTypes.EmailConfirmation);

		var message = _posterParams.EmailConfirmationMessageBuilder(
			user.Email!, sixDigitsCode
		);

		SendMessage(message);
	}

	public void SendPasswordRedefinitionMessage(TUser user)
	{
		string sixDigitsCode = GenAndSave(user.Id, JwtCustomTypes.PasswordRedefinition);

		var message = _posterParams.PasswordRedefinitionMessageBuilder(
			user.Email!, sixDigitsCode
		);

		SendMessage(message);
	}

	private string GenAndSave(TKey userId, string type)
	{
		var sixDigitCode = RandomNumberGenerator.GetInt32(1000000).ToString("000000");

		var token = _tokenBuilder.BuildSpecialToken(
			new ClaimsIdentity(new[] {
				new Claim(JwtCustomTypes.XDigitsCode, sixDigitCode),
				new Claim("consumed", "false")
			}),
			type,
			DateTime.UtcNow.AddMinutes(15),
			false
		);

		var userToken = new IdentityUserToken<TKey>()
		{
			UserId = userId,
			Value = token
		} as TUserToken;
		if (userToken == null)
			throw new InvalidCastException($"{nameof(IdentityUserToken<TKey>)} is not a {nameof(TUserToken)}, you would think it should be, since it's a speficic type constraint that it should, I'm also confused.");
		var c = _context.UserTokens.Add(userToken);

		_context.SaveChanges();
		return sixDigitCode;
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