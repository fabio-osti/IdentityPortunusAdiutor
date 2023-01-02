using System.Security.Cryptography;
using MailKit.Net.Smtp;
using MimeKit;
using PortunusAdiutor.Data;
using PortunusAdiutor.Exceptions;
using PortunusAdiutor.Models;
using PortunusAdiutor.Static;

namespace PortunusAdiutor.Services.MessagePoster;

/// <summary>
/// 	Defines a common method for SUT generation and consumption.
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
public class MessagePosterBase<TContext, TUser, TKey>
where TContext : ManagedUserDbContext<TUser, TKey>
where TUser : class, IManagedUser<TUser, TKey>
where TKey : IEquatable<TKey>
{
	private readonly TContext _context;

	/// <summary>
	/// 	Initializes an instance of the class.
	/// </summary>
	/// <param name="context">Database context</param>
	public MessagePosterBase(TContext context)
	{
		_context = context;
	}

	/// <summary>
	/// 	Generates an SUT for an <see cref="IManagedUser{TUser, TKey}"/> for an access of type <paramref name="type"/> and saves it on the database.
	/// </summary>
	/// <param name="userId">Id of the <see cref="IManagedUser{TUser, TKey}"/>.</param>
	/// <param name="type">Type of access granted by the the returning SUT.</param>
	/// <param name="xdc"></param>
	/// <returns>The SUT.</returns>
	protected SingleUseToken<TUser, TKey> GenAndSave(
		TKey userId,
		string type,
		out string xdc
	)
	{
		xdc = RandomNumberGenerator.GetInt32(1000000).ToString("000000");

		var userSut = new SingleUseToken<TUser, TKey>(userId, xdc, type);

		_context.SingleUseTokens.Add(userSut);
		_context.SaveChanges();

		return userSut;
	}

	/// <summary>
	/// 	Consumes a sent message.
	/// </summary>
	/// <param name="token">The access key sent by the message.</param>
	/// <param name="messageType">The type of message that was sent.</param>
	/// <returns>The key of the user to whom the token gives access to.</returns>
	public TKey ConsumeSut(
		string token,
		MessageType messageType
	)
	{
		var userSut =
			_context.SingleUseTokens.Find(token);

		if (userSut is null)
		{
			throw new SingleUseTokenNotFoundException();
		}

		var type = messageType.ToJwtTypeString();
		if (userSut.ExpiresOn < DateTime.UtcNow || userSut.Type != type)
		{
			throw new InvalidPasswordException();
		}


		_context.SingleUseTokens.Remove(userSut);
		_context.SaveChanges();

		return userSut.UserId;
	}
}
