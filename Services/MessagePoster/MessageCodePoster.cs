using Microsoft.AspNetCore.Identity;

using MailKit.Net.Smtp;
using MimeKit;

using PortunusAdiutor.Data;
using PortunusAdiutor.Models;
using PortunusAdiutor.Services.TokenBuilder;

namespace PortunusAdiutor.Services.MessagePoster;

/// <summary>
/// 	Implementation of <see cref="IMessagePoster{TUser, TKey}"/> with the plain text OTP.
/// </summary>
/// <typeparam name="TContext">Represents an Entity Framework database context used for identity with OTP keeping.</typeparam>
/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
/// <typeparam name="TRole">Represents a role in the identity system.</typeparam>
/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
/// <typeparam name="TUserClaim">Represents a claim possessed by an user.</typeparam>
/// <typeparam name="TUserRole">Represents the link between an user and a role.</typeparam>
/// <typeparam name="TUserLogin">Represents a login and its associated provider for an user.</typeparam>
/// <typeparam name="TRoleClaim">Represents a claim that is granted to all users within a role.</typeparam>
/// <typeparam name="TUserToken">Represents an authentication token for an user.</typeparam>
public class MessageCodePoster<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : MessagePosterBase<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IMessagePoster<TUser, TKey>
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
	MessageCodePosterParams _posterParams;

	/// <summary>
	/// 	Initializes an instance of the class.
	/// </summary>
	/// <param name="posterParams">Parameters for sending messages.</param>
	/// <param name="context">Database context.</param>
	public MessageCodePoster(MessageCodePosterParams posterParams, TContext context) : base(context)
	{
		_posterParams = posterParams;
	}

	/// <inheritdoc/>
	public void SendEmailConfirmationMessage(TUser user)
	{
		ArgumentException.ThrowIfNullOrEmpty(user.Email);

		var otp = GenAndSave(user.Id, MessageTypes.EmailConfirmation);

		var message = _posterParams.EmailConfirmationMessageBuilder(
			user.Email,
			otp.Password
		);

		SendMessage(message);
	}

	/// <inheritdoc/>
	public void SendPasswordRedefinitionMessage(TUser user)
	{
		ArgumentException.ThrowIfNullOrEmpty(user.Email);

		var otp = GenAndSave(user.Id, MessageTypes.PasswordRedefinition);

		var message = _posterParams.PasswordRedefinitionMessageBuilder(
			user.Email,
			otp.Password
		);

		SendMessage(message);
	}

	/// <inheritdoc/>
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