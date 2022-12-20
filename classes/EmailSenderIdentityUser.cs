using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using System.Net;
using System.Security.Claims;

/// <summary>
///		An implementation of <see cref="IdentityUser{TKey}"/>
///		that sends confirmation and password reseting emails.
/// </summary>
public static class EmailSenderIdentityUser
{
	private const string UriString = "http://localhost:2525";

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

	/// <summary>
	/// 
	/// </summary>
	static public ICredentials SmtpCredentials
	{ get; set; } = new NetworkCredential();

	/// <summary>
	/// 
	/// </summary>
	static public Uri SmtpUri	{ get; set; } = new Uri(UriString);

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
///		An implementation of <see cref="IdentityUser{TKey}"/>
///		that sends confirmation and password reseting emails.
/// </summary>
/// <typeparam name="TKey">IdentityUser primary key type.</typeparam>
public class EmailSenderIdentityUser<TKey> : IdentityUser<TKey>
where TKey : IEquatable<TKey>
{
	///	<summary>
	///		Initialize a new instance of 
	///		<see cref="EmailSenderIdentityUser{TKey}"/>.
	///	</summary>
	/// <param name="email">This user's email.</param>
	public EmailSenderIdentityUser(string email) : base()
	{
		Email = email;
	}

	///	<summary>
	///		Initialize a new instance of 
	///		<see cref="EmailSenderIdentityUser{TKey}"/>.
	///	</summary>
	/// <param name="email">This user's email.</param>
	///	<param name="userName">This user's name.</param>
	public EmailSenderIdentityUser(string email, string userName)
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

		SendMail(message);
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

		SendMail(message);
	}

	private void SendMail(MimeMessage message)
	{
		using (var client = new SmtpClient()) {
			client.Connect(EmailSenderIdentityUser.SmtpUri);
			client.Authenticate(EmailSenderIdentityUser.SmtpCredentials);
			client.Send(message);
			client.Disconnect(true);
		}
	}
}