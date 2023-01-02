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
	public Uri SmtpUri { get; set; } = new(DefaultSmtpUriString);

	/// <summary>
	/// 	Credentials used for the SMTP server.
	/// </summary>
	public ICredentials SmtpCredentials{ get; set; } = DefaultCredentials;
		
	/// <summary>
	///		Sets or gets the builder of the email that should be sent if the user
	///		forgets his password.
	/// </summary>
	public MessageBuilder PasswordRedefinitionMessageBuilder{ get; set; } = 
		DefaultPasswordRedefinitionMessageBuilder;

	/// <summary>
	///		Sets or gets the builder of the email that should be sent when the user 
	///		is registered.
	/// </summary>
	public MessageBuilder EmailConfirmationMessageBuilder{ get; set; } = 
		DefaultEmailConfirmationMessageBuilder;

	/// <summary>
	/// 	Initialize an instance of <see cref="CodeMessagePosterParams"/>
	/// 	with only the defaults as base.
	/// </summary>
	public CodeMessagePosterParams() { }

	/// <summary>
	/// 	Initialize an instance of <see cref="LinkMessagePosterParams"/> 
	/// 	using an <see cref="IConfiguration"/> object and
	/// 	the defaults as base.
	/// </summary>
	/// <param name="config">
	/// 	An <see cref="IConfiguration"/> instance that 
	/// 	have the section "SMTP" defined.
	/// </param>
	public CodeMessagePosterParams(IConfiguration config)
	{
		var sect = config.GetSection("SMTP");
		var smtpUri = sect["SMTP_URI"];
		if (smtpUri is not null) {
			SmtpUri = new(smtpUri);
		}

		var smtpUser = sect["SMTP_USER"];
		if (smtpUser is not null) {
			var smtpPassword = sect["SMTP_PSWRD"];
			SmtpCredentials =
				new NetworkCredential(smtpUser, smtpPassword);
		}
	}

	private const string DefaultSmtpUriString = "smtp://localhost:2525";
	private static ICredentials DefaultCredentials => new NetworkCredential();

	private static MimeMessage DefaultPasswordRedefinitionMessageBuilder(
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

	private static MimeMessage DefaultEmailConfirmationMessageBuilder(
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
