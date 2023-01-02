using System.Security.Cryptography;

using Microsoft.AspNetCore.Identity;

using PortunusAdiutor.Data;
using PortunusAdiutor.Models;

namespace PortunusAdiutor.Services.MessagePoster;

/// <summary>
/// 	Defines a common method for SUT generation and consumption.
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TUser"></typeparam>
/// <typeparam name="TRole"></typeparam>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TUserClaim"></typeparam>
/// <typeparam name="TUserRole"></typeparam>
/// <typeparam name="TUserLogin"></typeparam>
/// <typeparam name="TRoleClaim"></typeparam>
/// <typeparam name="TUserToken"></typeparam>
public class MessagePosterBase<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
where TContext : ManagedIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
where TUser : IdentityUser<TKey>, IManagedUser<TUser, TKey>
where TRole : IdentityRole<TKey>
where TKey : IEquatable<TKey>
where TUserClaim : IdentityUserClaim<TKey>
where TUserRole : IdentityUserRole<TKey>
where TUserLogin : IdentityUserLogin<TKey>
where TRoleClaim : IdentityRoleClaim<TKey>
where TUserToken : IdentityUserToken<TKey>
{
	TContext _context;

	/// <summary>
	/// 	Initializes an instance of the class.
	/// </summary>
	/// <param name="context">Database context</param>
	public MessagePosterBase(TContext context)
	{
		_context = context;
	}

	/// <summary>
	/// 	Generates an SUT for an <see cref="IdentityUser{TKey}"/> for an access of type <paramref name="type"/> and saves it on the database.
	/// </summary>
	/// <param name="userId">Id of the <see cref="IdentityUser{TKey}"/>.</param>
	/// <param name="type">Type of access granted by the the returning SUT.</param>
	/// <param name="xdc"></param>
	/// <returns>The SUT.</returns>
	protected SingleUseToken<TUser, TKey> GenAndSave(TKey userId, string type, out string xdc)
	{
		xdc = RandomNumberGenerator.GetInt32(1000000).ToString("000000");

		var userSut = new SingleUseToken<TUser, TKey>(userId, xdc, type);

		_context.UserSingleUseTokens.Add(userSut);
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
		var type = messageType.ToJwtTypeString();
		var userSut =
			_context.UserSingleUseTokens.Find(token);

		if (userSut is null)
		{
			throw new UnauthorizedAccessException("Single Token not found.");
		}

		if (userSut.ExpiresOn < DateTime.UtcNow)
		{
			throw new UnauthorizedAccessException("Token already expired.");
		}

		var code = _context.UserSingleUseTokens.Remove(userSut);
		_context.SaveChanges();

		return userSut.UserId;
	}
}