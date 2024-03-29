using Microsoft.AspNetCore.Identity;
using PortunusAdiutor.Static;

namespace PortunusAdiutor.Services.MessagePoster;

/// <summary>
/// 	Defines all necessary methods for message posting
/// </summary>
/// <typeparam name="TUser"></typeparam>
/// <typeparam name="TKey"></typeparam>
public interface IMessagePoster<TUser, TKey>
where TUser : IdentityUser<TKey>
where TKey : IEquatable<TKey>
{
	/// <summary>
	/// 	Sends message asking for the confirmation of the <see cref="IdentityUser{TKey}.Email"/> from <paramref name="user"/>.
	/// </summary>
	/// <param name="user">Receiver of the message.</param>
	void SendEmailConfirmationMessage(TUser user);
	
	/// <summary>
	/// 	Sends message asking for the redefinition of the <see cref="IdentityUser{TKey}.PasswordHash"/> from <paramref name="user"/>.
	/// </summary>
	/// <param name="user">Receiver of the message.</param>
	void SendPasswordRedefinitionMessage(TUser user);

	/// <summary>
	/// 	Consumes a sent message.
	/// </summary>
	/// <param name="token">The access key sent by the message.</param>
	/// <param name="messageType">The type of message that was sent.</param>
	/// <returns>True if successful, else false.</returns>
	TKey ConsumeSut(
		string token,
		MessageType messageType
	);
}
