using System.Net;

using Microsoft.Extensions.Configuration;

using MimeKit;

using MessageBuilder = System.Func<string, string, MimeKit.MimeMessage>;

namespace PortunusAdiutor.Services.MessagePoster;

/// <summary>
/// 	Parameters necessary for the link message posting.
/// </summary>
public class LinkMessagePosterParams
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
	/// 	App endpoint for email validation.
	/// </summary>
	public string EmailConfirmationEndpoint { get; set; } =
		DefaultEmailConfirmationEndpoint;

	/// <summary>
	/// 	App endpoint for password redefinition.
	/// </summary>
	public string PasswordRedefinitionEndpoint { get; set; } =
		DefaultPasswordRedefinitionEndpoint;

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
	/// 	Initialize an instance of <see cref="LinkMessagePosterParams"/>
	/// 	with only the defaults as base.
	/// </summary>
	public LinkMessagePosterParams() {	}

	/// <summary>
	/// 	Initialize an instance of <see cref="LinkMessagePosterParams"/> 
	/// 	using an <see cref="IConfiguration"/> object and
	/// 	the defaults as base.
	/// </summary>
	/// <param name="config">
	/// 	An <see cref="IConfiguration"/> instance that 
	/// 	have the section "SMTP" defined.
	/// </param>
	public LinkMessagePosterParams(IConfiguration config)
	{
		var sect = config.GetSection("SMTP");
		var smtpUri = sect["URI"];
		if (smtpUri is not null) {
			SmtpUri = new Uri(smtpUri);
		}

		var smtpUser = sect["USERNAME"];
		if (smtpUser is not null) {
			var smtpPassword = sect["PSWRD"];
			SmtpCredentials =
				new NetworkCredential(smtpUser, smtpPassword);
		}

		var emailConfirmationEndpoint = sect["ECE"];
		if (emailConfirmationEndpoint is not null) {
			EmailConfirmationEndpoint = emailConfirmationEndpoint;
		}

		var passwordRedefinitionEndpoint = sect["PRE"];
		if (passwordRedefinitionEndpoint is not null) {
			PasswordRedefinitionEndpoint = passwordRedefinitionEndpoint;
		}
	}

	// DEFAULT VALUES
	private const string DefaultSmtpUriString = "smtp://localhost:2525";

	private const string DefaultEmailConfirmationEndpoint =
		"http://localhost:8080/Authorization/ConfirmEmail?token=";

	private const string DefaultPasswordRedefinitionEndpoint =
		"http://localhost:8080/Authorization/RedefinePassword?token=";

	private static ICredentials DefaultCredentials => new NetworkCredential();

	private static MimeMessage DefaultPasswordRedefinitionMessageBuilder(
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

				Please confirm that it was you by opening this link: 
				
				{link}

				If you didn't make this request, then you can ignore this email.
				"""
		};

		return message;
	}

	private static MimeMessage DefaultEmailConfirmationMessageBuilder(
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

				Please confirm that it was you by opening this link: 

				{link}

				If you didn't make this request, then you can ignore this email.
				"""
		};

		return message;
	}
}
