using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Net;

using MessageBuilder = System.Func<string, string, MimeKit.MimeMessage>;




public class MailCodePosterParams
{

	public Uri SmtpUri { get; set; } = new Uri(defaultSmtpUriString);

	public ICredentials SmtpCredentials
	{ get; set; } = new NetworkCredential();

	public MessageBuilder PasswordRedefinitionMessageBuilder
	{ get; set; } = defaultForgotPasswordMessageBuilder;

	public MessageBuilder EmailConfirmationMessageBuilder
	{ get; set; } = defaultConfirmEmailMessageBuilder;

	public MailCodePosterParams() { }

	public MailCodePosterParams(ConfigurationManager config)
	{
		var sect = config.GetSection("SMTP");
		var smtpUri = config["SMTP_URI"];
		if (smtpUri is not null)
		{
			SmtpUri = new(smtpUri);
		}

		var smtpUser = config["SMTP_USER"];
		if (smtpUser is not null)
		{
			var smtpPassword = config["SMTP_PSWRD"];
			SmtpCredentials =
				new NetworkCredential(smtpUser, smtpPassword);
		}
	}

	const string defaultSmtpUriString = "smtp://localhost:2525";
	static ICredentials defaultCredentials => new NetworkCredential();

	static MimeMessage defaultForgotPasswordMessageBuilder(
		string email,
		string code
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
				
				{code}

				If you didn’t make this request, then you can ignore this email.
				"""
		};

		return message;
	}

	static MimeMessage defaultConfirmEmailMessageBuilder(
		string email,
		string code
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

				{code}

				If you didn’t make this request, then you can ignore this email.
				"""
		};

		return message;
	}
}