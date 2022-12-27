using Microsoft.AspNetCore.Identity;
using PortunusAdiutor.Models;

namespace PortunusAdiutor.Services.MailPoster;

public interface IMailPoster<TUser, TKey>
where TUser : IdentityUser<TKey>
where TKey : IEquatable<TKey>
{
	void SendEmailConfirmationMessage(TUser user);
	void SendPasswordRedefinitionMessage(TUser user);
	void ConsumeMessage(TUser user, string message, MessageType messageType);
}

public enum MessageType
{
	EmailConfirmation,
	PasswordRedefinition
}