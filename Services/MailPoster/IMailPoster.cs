using Microsoft.AspNetCore.Identity;
using PortunusAdiutor.Models;

namespace PortunusAdiutor.Services.MailPoster;

public interface IMailPoster<TUser, TKey>
where TUser : IdentityUser<TKey>
where TKey : IEquatable<TKey>
{
	void SendEmailConfirmationMessage(TUser user);
	void SendPasswordRedefinitionMessage(TUser user);
	TUser ConsumeMessage(
		string message, 
		MessageType messageType,
		TUser? user
	);
}

public enum MessageType
{
	EmailConfirmation,
	PasswordRedefinition
}