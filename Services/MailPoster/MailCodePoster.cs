using System.Security.Cryptography;

using Microsoft.AspNetCore.Identity;

using PortunusAdiutor.Contexts;
using PortunusAdiutor.Services.MailPoster;

/// <summary>
/// 	Service for sending a <see cref="string"/> of length == 6
/// 	to a <typeparamref name="TUser"/> for some special action.
/// </summary>
/// <typeparam name="TUser">The type of user to whom the messages will be sent.</typeparam>
/// <typeparam name="TRole">Type of <see cref="IdentityRole{TKey}"/> used by the identity system</typeparam>
/// <typeparam name="TKey">The type used for the primary key for the <typeparamref name="TUser"/>.</typeparam>
public class MailCodePoster<TUser, TRole, TKey> : IMailPoster<TUser, TKey>
where TUser : IdentityUser<TKey>
where TRole : IdentityRole<TKey>
where TKey : IEquatable<TKey>
{
	CodeBasedIdentityDbContext<TUser, TRole, TKey> _context;
	MailCodePosterParams _posterParams;
	/// <summary>
	/// 	Initialize a new instance of <see cref="MailCodePoster{TUser, TRole, TKey}"/>
	/// </summary>
	/// <param name="posterParams">Parameters necessary for the code message posting.</param>
	/// <param name="context">Context service for keeping the sent codes.</param>
	public MailCodePoster(MailCodePosterParams posterParams, CodeBasedIdentityDbContext<TUser, TRole, TKey> context)
	{
		_posterParams = posterParams;
		_context = context;
	}
	///	<inheritdoc/>
	public void MessageConsumed(TUser user, string content, MessageType messageType)
	{
		var code = _context.EmailCodes.Find(new { @UserId = user.Id, @Code = content });
		if (code?.Type != messageType) {
			throw new Exception();
		}
	}
	///	<inheritdoc/>
	public void SendEmailConfirmationMessage(TUser user)
	{
		_context.EmailCodes.Add(new MailCodePost<TUser, TKey>()
		{
			UserId = user.Id,
			Code = RandomNumberGenerator.GetInt32(1000000).ToString(),
			ExpiresOn = DateTime.UtcNow.AddMinutes(15),
			Type = MessageType.EmailConfirmation
		});
		_context.SaveChanges();
		throw new NotImplementedException();
	}
	///	<inheritdoc/>
	public void SendPasswordRedefinitionMessage(TUser user)
	{
		throw new NotImplementedException();
	}
}