using Microsoft.AspNetCore.Identity;

using MimeKit;
using MailKit.Net.Smtp;
using PortunusAdiutor.Services.MailPoster;
using PortunusAdiutor.Services.TokenBuilder;

/// <summary>
/// 	Service for sending a link with a jwt token to the user for some special action.
/// </summary>
/// <typeparam name="TUser">Type of <see cref="IdentityUser{TKey}"/> used by the identity system.</typeparam>
/// <typeparam name="TKey">The type used for the primary key for the <typeparamref name="TUser"/>.</typeparam>
public class MailLinkPoster<TUser, TKey> : IMailPoster<TUser, TKey>
where TUser : IdentityUser<TKey>
where TKey : IEquatable<TKey>
{
	private readonly ITokenBuilder _tokenBuilder;
	private readonly MailLinkPosterParams _posterParams;

	///	<summary>
	///		Initialize a new instance of <see cref="MailLinkPoster{TUser, TKey}"/>.
	///	</summary>
	/// <param name="posterParams">Parameters necessary for the link message posting.</param>
	/// <param name="tokenBuilder">Service to build the tokens sent on the messages.</param>
	public MailLinkPoster(MailLinkPosterParams posterParams, ITokenBuilder tokenBuilder)
	{
		_posterParams = posterParams;
		_tokenBuilder = tokenBuilder;
	}
	
	///	<inheritdoc/>
	public void MessageConsumed(TUser user, string content, MessageType messageType)
	{
		var target = _tokenBuilder
			.ValidateCustomTypeToken(
				content,
				messageType switch
				{
					MessageType.EmailConfirmation => JwtCustomTypes.EmailConfirmation,
					MessageType.PasswordRedefinition => JwtCustomTypes.PasswordRedefinition,
					_ => throw new ArgumentOutOfRangeException(nameof(messageType))
				}
			);
		if (target == null) {
			throw new ArgumentException($"Invalid {nameof(content)}.");
		}
		if (user.Id.ToString() != target) {
			throw new UnauthorizedAccessException(
				$"The target of {nameof(content)} and {nameof(user)} are not the same."
			);
		}
	}

	///	<inheritdoc/>
	public void SendEmailConfirmationMessage(TUser user)
	{
		var message = _posterParams.EmailConfirmationMessageBuilder(
			user.Email!,
			_posterParams.EmailConfirmationEndpoint
				+ _tokenBuilder.BuildCustomTypeToken<TUser, TKey>(
					user,
					JwtCustomTypes.EmailConfirmation
				)
		);

		SendMessage(message);
	}

	///	<inheritdoc/>
	public void SendPasswordRedefinitionMessage(TUser user)
	{
		var message = _posterParams.PasswordRedefinitionMessageBuilder(
			user.Email!,
			_posterParams.PasswordRedefinitionEndpoint
				+ _tokenBuilder.BuildCustomTypeToken<TUser, TKey>(
					user,
					JwtCustomTypes.PasswordRedefinition
				)
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