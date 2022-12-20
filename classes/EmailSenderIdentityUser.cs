using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using System.Security.Claims;

/// <summary>
/// 	Non-generic container for the static values.
/// </summary>
public static class EmailSenderIdentityUser
{
	/// <summary>
	/// 
	/// </summary>
	static public Func<string, string, MimeMessage> ForgotPasswordMessageBuilder
	{ get; set; } = defaultForgotPasswordMessageBuilder;

	/// <summary>
	/// 
	/// </summary>
	static public Func<string, string, MimeMessage> ValidateEmailMessageBuilder
	{ get; set; } = defaultValidateEmailMessageBuilder;

	static MimeMessage defaultForgotPasswordMessageBuilder(
		string email,
		string link
	)
	{
		var message = new MimeMessage();
		message.From.Add(new MailboxAddress("", ""));
		message.To.Add(new MailboxAddress("", email));
		message.Subject = "Reset Password";

		message.Body = new TextPart("plain")
		{
			Text = $"""
				Hello,

				A new password was requested for your account,

				Please confirm that it was you by opening this link: 
				
				{link}

				If you didn’t make this request, then you can ignore this email.
				"""
		};
		return message;
	}

	static MimeMessage defaultValidateEmailMessageBuilder(
		string email,
		string link
	)
	{
		var message = new MimeMessage();
		message.From.Add(new MailboxAddress("", ""));
		message.To.Add(new MailboxAddress("", email));
		message.Subject = "Reset Password";

		message.Body = new TextPart("plain")
		{
			Text = $"""
				Hello,

				Your account have been registered, 

				Please confirm that it was you by opening this link: 
				
				{link}

				If you didn’t make this request, then you can ignore this email.
				"""
		};
		return message;
	}
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TKey"></typeparam>
abstract public class EmailSenderIdentityUser<TKey> : IdentityUser<TKey>
where TKey : IEquatable<TKey>
{
	///	<summary>
	///		Initialize a new instance of 
	///		<see cref="EmailSenderIdentityUser{TKey}"/>.
	///	</summary>
	/// <param name="email">This user's email.</param>
	protected EmailSenderIdentityUser(string email) : base() 
	{
		Email = email;
	}

	///	<summary>
	///		Initialize a new instance of 
	///		<see cref="EmailSenderIdentityUser{TKey}"/>.
	///	</summary>
	/// <param name="email">This user's email.</param>
	///	<param name="userName">This user's name.</param>
	protected EmailSenderIdentityUser(string email, string userName)
	: base(userName) 
	{
		Email = email;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="tokenBuilder"></param>
	/// <param name="baseAddress"></param>
	public void SendVerificationEmail(
		ITokenBuilder tokenBuilder,
		string baseAddress
	)
	{
		var message = EmailSenderIdentityUser.ValidateEmailMessageBuilder(
			Email!,
			tokenBuilder.BuildCustomTypeToken(
				new[] { new Claim(ClaimTypes.Email, Email!) }, 
				JwtCustomTypes.EmailValidation
			)
		);

		using (var client = new SmtpClient()) {
			client.Connect("", 0, false);
			client.Authenticate("", "");
			client.Send(message);
			client.Disconnect(true);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="tokenBuilder"></param>
	/// <param name="baseAddress"></param>
	public void SendPasswordResetEmail(
		ITokenBuilder tokenBuilder,
		string baseAddress
	)
	{
		var message = EmailSenderIdentityUser.ForgotPasswordMessageBuilder(
			Email!,
			tokenBuilder.BuildCustomTypeToken(
				new[] { new Claim(ClaimTypes.Email, Email!) },
				JwtCustomTypes.PasswordRedefinition
			)
		);

		using (var client = new SmtpClient()) {
			client.Connect("", 0, false);
			client.Authenticate("", "");
			client.Send(message);
			client.Disconnect(true);
		}
	}
}