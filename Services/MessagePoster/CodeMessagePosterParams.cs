using System.Net;

using Microsoft.Extensions.Configuration;

using MimeKit;

using MessageBuilder = System.Func<string, string, MimeKit.MimeMessage>;

namespace PortunusAdiutor.Services.MessagePoster;

/// <summary>
/// 	Parameters necessary for the link message posting.
/// </summary>
public class CodeMessagePosterParams
{
	/// <summary>
	/// 	Uri used for the SMTP server.
	/// </summary>
	public Uri SmtpUri { get; set; } = new Uri(defaultSmtpUriString);

	/// <summary>
	/// 	Credentials used for the SMTP server.
	/// </summary>
	public ICredentials SmtpCredentials{ get; set; } = defaultCredentials;
		
	/// <summary>
	///		Sets or gets the builder of the email that should be sent if the user
	///		forgets his password.
	/// </summary>
	public MessageBuilder PasswordRedefinitionMessageBuilder{ get; set; } = 
		defaultPasswordRedefinitionMessageBuilder;

	/// <summary>
	///		Sets or gets the builder of the email that should be sent when the user 
	///		is registered.
	/// </summary>
	public MessageBuilder EmailConfirmationMessageBuilder{ get; set; } = 
		defaultEmailConfirmationMessageBuilder;

	/// <summary>
	/// 	Initialize an instance of <see cref="CodeMessagePosterParams"/>
	/// 	with only the defaults as base.
	/// </summary>
	public CodeMessagePosterParams() { }

	/// <summary>
	/// 	Iniatialize an instance of <see cref="LinkMessagePosterParams"/> 
	/// 	using an <see cref="IConfiguration"/> object and
	/// 	the defaults as base.
	/// </summary>
	/// <param name="config">
	/// 	An <see cref="IConfiguration"/> instance that 
	/// 	have the section "SMTP" defined.
	/// </param>
	public CodeMessagePosterParams(ConfigurationManager config)
	{
		var sect = config.GetSection("SMTP");
		var smtpUri = config["SMTP_URI"];
		if (smtpUri is not null) {
			SmtpUri = new(smtpUri);
		}

		var smtpUser = config["SMTP_USER"];
		if (smtpUser is not null) {
			var smtpPassword = config["SMTP_PSWRD"];
			SmtpCredentials =
				new NetworkCredential(smtpUser, smtpPassword);
		}
	}

	const string defaultSmtpUriString = "smtp://localhost:2525";
	static ICredentials defaultCredentials => new NetworkCredential();

	static MimeMessage defaultPasswordRedefinitionMessageBuilder(
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

				If you didn't make this request, then you can ignore this email.
				"""
		};

		return message;
	}

	static MimeMessage defaultEmailConfirmationMessageBuilder(
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

				If you didn't make this request, then you can ignore this email.
				"""
		};

		return message;
	}
}