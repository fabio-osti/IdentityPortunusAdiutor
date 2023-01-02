using MailKit.Net.Smtp;

using MimeKit;

using PortunusAdiutor.Data;
using PortunusAdiutor.Models;
using PortunusAdiutor.Static;

namespace PortunusAdiutor.Services.MessagePoster;

/// <summary>
/// 	Implementation of <see cref="IMessagePoster{TUser, TKey}"/> with the plain text SUT.
/// </summary>
/// <typeparam name="TContext">Represents an Entity Framework database context used for identity with SUT keeping.</typeparam>
/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
public class LinkMessagePoster<TContext, TUser, TKey> : MessagePosterBase<TContext, TUser, TKey>, IMessagePoster<TUser, TKey>
where TContext : ManagedUserDbContext<TUser, TKey>
where TUser : class, IManagedUser<TUser, TKey>
where TKey : IEquatable<TKey>
{
	private readonly LinkMessagePosterParams _posterParams;
	/// <summary>
	/// 	Initializes an instance of the class.
	/// </summary>
	/// <param name="posterParams">Parameters for sending messages.</param>
	/// <param name="context">Database context.</param>
	public LinkMessagePoster(
		LinkMessagePosterParams posterParams,
		TContext context
	) : base(context)
	{
		_posterParams = posterParams;
	}

	/// <inheritdoc/>
	public void SendEmailConfirmationMessage(TUser user)
	{
		ArgumentException.ThrowIfNullOrEmpty(user.Email);
		// Generates SUT
		var sut = GenAndSave(user.Id, MessageTypes.EmailConfirmation, out _);
		// Builds and sends message
		var message = _posterParams.EmailConfirmationMessageBuilder(
			user.Email,
			_posterParams.EmailConfirmationEndpoint + sut.Token
		);
		SendMessage(message);
	}

	/// <inheritdoc/>
	public void SendPasswordRedefinitionMessage(TUser user)
	{
		ArgumentException.ThrowIfNullOrEmpty(user.Email);
		// Generates SUT
		var sut = GenAndSave(user.Id, MessageTypes.PasswordRedefinition, out _);
		// Builds and sends message
		var message = _posterParams.PasswordRedefinitionMessageBuilder(
			user.Email,
			_posterParams.PasswordRedefinitionEndpoint + sut.Token
		);
		SendMessage(message);
	}
	
	private void SendMessage(MimeMessage message)
	{
		using var client = new SmtpClient();
		client.Connect(_posterParams.SmtpUri);
		client.Authenticate(_posterParams.SmtpCredentials);
		client.Send(message);
		client.Disconnect(true);
	}
}
