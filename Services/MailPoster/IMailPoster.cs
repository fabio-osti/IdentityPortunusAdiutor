using Microsoft.AspNetCore.Identity;

namespace PortunusAdiutor.Services.MailPoster;

/// <summary>
/// 	Interface to define a class that sends messages to a <typeparamref name="TUser"/>.
/// </summary>
/// <typeparam name="TUser">The type of user to whom the messages will be sent.</typeparam>
/// <typeparam name="TKey">The type used for the primary key for the <typeparamref name="TUser"/>.</typeparam>
public interface IMailPoster<TUser, TKey>
where TUser : IdentityUser<TKey>
where TKey : IEquatable<TKey>
{
	/// <summary>
	/// 	Sends a message for email confirmation to this <paramref name="user"/>.
	/// </summary>
	/// <param name="user">The user to whom the message will be sent.</param>
	void SendEmailConfirmationMessage(TUser user);
	/// <summary>
	/// 	Sends a message for password redefinition to this <paramref name="user"/>.
	/// </summary>
	/// <param name="user">The user to whom the message will be sent.</param>

	void SendPasswordRedefinitionMessage(TUser user);
	/// <summary>
	/// 	Sets this message <paramref name="content"/> as consumed for this <paramref name="user"/>.
	/// </summary>
	/// <param name="user">The user to whom this message was sent.</param>
	/// <param name="content">The custom content (token or code, for example) sent to this user.</param>
	/// <param name="messageType">Validates this message intent.</param>
	void MessageConsumed(TUser user, string content, MessageType messageType);
}

/// <summary>
/// 	The message intent.
/// </summary>
public enum MessageType
{
	/// <summary>
	/// 	Email confirmation.
	/// </summary>
	EmailConfirmation,
	/// <summary>
	/// 	Password redefinition.
	/// </summary>
	PasswordRedefinition
}