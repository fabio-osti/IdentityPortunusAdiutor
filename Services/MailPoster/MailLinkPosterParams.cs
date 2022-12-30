using System.Net;

using Microsoft.Extensions.Configuration;

using MimeKit;

using MessageBuilder = System.Func<string, string, MimeKit.MimeMessage>;

/// <summary>
/// 	Parameters necessary for the link message posting.
/// </summary>
public class MailLinkPosterParams
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
	/// 	App endpoint for email validation.
	/// </summary>
	public string EmailConfirmationEndpoint { get; set; } =
		defaultEmailConfirmationEndpoint;

	/// <summary>
	/// 	App endpoint for password redefinition.
	/// </summary>
	public string PasswordRedefinitionEndpoint { get; set; } =
		defaultPasswordRedefinitionEndpoint;

	/// <summary>
	///		Sets or gets the builder of the email that should be sent if the user
	///		forgets his password.
	/// </summary>
	public MessageBuilder PasswordRedefinitionMessageBuilder
	{ get; set; } = defaultPasswordRedefinitionMessageBuilder;

	/// <summary>
	///		Sets or gets the builder of the email that should be sent when the user 
	///		is registered.
	/// </summary>
	public MessageBuilder EmailConfirmationMessageBuilder
	{ get; set; } = defaultEmailConfirmationMessageBuilder;

	/// <summary>
	/// 	Initialize an instance of <see cref="MailLinkPosterParams"/>
	/// 	with only the defaults as base.
	/// </summary>
	public MailLinkPosterParams()
	{

	}

	/// <summary>
	/// 	Iniatialize an instance of <see cref="MailLinkPosterParams"/> 
	/// 	using an <see cref="IConfiguration"/> object and
	/// 	the defaults as base.
	/// </summary>
	/// <param name="config">
	/// 	An <see cref="IConfiguration"/> instance that 
	/// 	have the section "SMTP" defined.
	/// </param>
	public MailLinkPosterParams(IConfiguration config)
	{
		var sect = config.GetSection("SMTP");
		var smtpUri = sect["URI"];
		if (smtpUri is not null) {
			SmtpUri = new(smtpUri);
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
	const string defaultSmtpUriString = "smtp://localhost:2525";

	const string defaultEmailConfirmationEndpoint =
		"http://localhost:8080/Authorization/ConfirmEmail?token=";

	const string defaultPasswordRedefinitionEndpoint =
		"http://localhost:8080/Authorization/RedefinePassword?token=";

	static ICredentials defaultCredentials => new NetworkCredential();

	static MimeMessage defaultPasswordRedefinitionMessageBuilder(
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

				If you didn’t make this request, then you can ignore this email.
				"""
		};

		return message;
	}

	static MimeMessage defaultEmailConfirmationMessageBuilder(
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

				If you didn’t make this request, then you can ignore this email.
				"""
		};

		return message;
	}
}