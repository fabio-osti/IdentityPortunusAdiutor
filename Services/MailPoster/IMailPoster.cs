using Microsoft.AspNetCore.Identity;
using PortunusAdiutor.Models;

namespace PortunusAdiutor.Services.MailPoster;

public interface IMailPoster<TUser, TKey>
where TUser : IdentityUser<TKey>
where TKey : IEquatable<TKey>
{
	void SendEmailConfirmationMessage(TUser user);
	void SendPasswordRedefinitionMessage(TUser user);
	bool ConsumeOtp(
		TKey? userId,
		string otp,
		MessageType messageType
	);
}

