using MimeKit;
using System.Net;

using MessageBuilder = System.Func<string, string, MimeKit.MimeMessage>;

/// <summary>
/// 	Parameters necessary for the code message posting.
/// </summary>
public class MailCodePosterParams 
{

	/// <summary>
	/// 	Uri used for the SMTP server.
	/// </summary>
	public Uri SmtpUri { get; set; } = new Uri(defaultSmtpUriString);

	/// <summary>
	/// 	Credentials used for the SMTP server.
	/// </summary>
	public ICredentials SmtpCredentials
	{ get; set; } = new NetworkCredential();

	/// <summary>
	///		Sets or gets the builder of the email that should be sent if the user
	///		forgets his password.
	/// </summary>
	public MessageBuilder PasswordRedefinitionMessageBuilder
	{ get; set; } = defaultForgotPasswordMessageBuilder;

	/// <summary>
	///		Sets or gets the builder of the email that should be sent when the user 
	///		is registered.
	/// </summary>
	public MessageBuilder EmailConfirmationMessageBuilder
	{ get; set; } = defaultConfirmEmailMessageBuilder;

	// DEFAULT VALUES
	const string defaultSmtpUriString = "smtp://localhost:2525";

	static ICredentials defaultCredentials => new NetworkCredential();

	static MimeMessage defaultForgotPasswordMessageBuilder(
		string email,
		string link
	)
	{
		var message = new MimeMessage();

		message.From.Add(new MailboxAddress("", ""));
		message.To.Add(new MailboxAddress("", email));
		message.Subject = "Reset your password";
		message.Body = new TextPart("plain")
		{
			Text = $"""
				Hello,

				A new password was requested for your account,

				Please confirm that it was you by entering this code: 
				
				{link}

				If you didn’t make this request, then you can ignore this email.
				"""
		};

		return message;
	}

	static MimeMessage defaultConfirmEmailMessageBuilder(
		string email,
		string link
	)
	{
		var message = new MimeMessage();

		message.From.Add(new MailboxAddress("", ""));
		message.To.Add(new MailboxAddress("", email));
		message.Subject = "Validate your email";
		message.Body = new TextPart("plain")
		{
			Text = $"""
				Hello,

				Your account have been registered, 

				Please confirm that it was you by entering this code: 

				{link}

				If you didn’t make this request, then you can ignore this email.
				"""
		};

		return message;
	}
}