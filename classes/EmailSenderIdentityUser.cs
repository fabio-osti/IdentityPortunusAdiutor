using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Security.Claims;
using MimeKit;
using MailKit.Net.Smtp;
using EmailBuilder = System.Func<string, string, MimeKit.MimeMessage>;
using System.Diagnostics.CodeAnalysis;

/// <summary>
///		An implementation of <see cref="IdentityUser{TKey}"/> that sends confirmation
///		and password reseting emails.
/// </summary>
public static class EmailSenderIdentityUser
{

	/// <summary>
	/// 	Uri used for the SMTP server.
	/// </summary>
	static public Uri SmtpUri { get; set; } = new Uri(defaultSmtpUriString);

	/// <summary>
	/// 	Credentials used for the SMTP server.
	/// </summary>
	static public ICredentials SmtpCredentials
	{ get; set; } = new NetworkCredential();

	/// <summary>
	/// 
	/// </summary>
	static public string EmailValidationEndpoint { get; set; } =
		defaultEmailValidationEndpoint;

	/// <summary>
	/// 
	/// </summary>
	static public string PasswordRedefinitionEndpoint { get; set; } =
		defaultPasswordRedefinitionEndpoint;

	/// <summary>
	///		Sets or gets the builder of the email that should be sent if the user
	///		forgets his password.
	/// </summary>
	static public EmailBuilder PasswordRedefinitionMessageBuilder
	{ get; set; } = defaultForgotPasswordMessageBuilder;

	/// <summary>
	///		Sets or gets the builder of the email that should be sent when the user 
	///		is registered.
	/// </summary>
	static public EmailBuilder ValidateEmailMessageBuilder
	{ get; set; } = defaultValidateEmailMessageBuilder;



	// DEFAULT VALUES
	const string defaultSmtpUriString = "smtp://localhost:2525";

	const string defaultEmailValidationEndpoint =
		"http://localhost:8080/Authorization/ValidateEmail?token=";

	const string defaultPasswordRedefinitionEndpoint =
		"http://localhost:8080/Authorization/RedefinePassword?token=";

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

/// <summary>
///		An implementation of <see cref="IdentityUser{TKey}"/>
///		that sends confirmation and password reseting emails.
/// </summary>
/// <typeparam name="TKey">IdentityUser primary key type.</typeparam>
public class EmailSenderIdentityUser<TKey> : IdentityUser<TKey>
where TKey : IEquatable<TKey>
{

	///	<summary>
	///		Initialize a new instance of <see cref="EmailSenderIdentityUser{TKey}"/>.
	///	</summary>
	/// <param name="email">This user's email.</param>
	public EmailSenderIdentityUser(string email) : base()
	{
		Email = email;
	}

	///	<summary>
	///		Initialize a new instance of <see cref="EmailSenderIdentityUser{TKey}"/>.
	///	</summary>
	/// <param name="email">This user's email.</param>
	///	<param name="username">This user's name.</param>
	public EmailSenderIdentityUser(string email, string username) : base(username)
	{
		Email = email;
	}

	private string _email;
	/// <inheritdoc/>
	[MemberNotNull(nameof(_email))]
	override public string? Email
	{
		get => _email ?? throw new ArgumentNullException(nameof(_email));
		set => _email = value ?? throw new ArgumentNullException(nameof(value));
	}

	/// <summary>
	/// 	Sends an email to this user with a link for verifying his account.
	/// </summary>
	/// <param name="tokenBuilder">The app's <see cref="ITokenBuilder"/>.</param>
	/// <param name="baseAddress">The app's base address.</param>
	public void SendEmailValidationMessage(
		ITokenBuilder tokenBuilder,
		string baseAddress
	)
	{
		var message = EmailSenderIdentityUser.ValidateEmailMessageBuilder(
			Email!,
			EmailSenderIdentityUser.EmailValidationEndpoint
				+ tokenBuilder.BuildCustomTypeToken(
					new[] { new Claim(ClaimTypes.Email, Email!) },
					JwtCustomTypes.EmailValidation
				)
		);

		SendMessage(message);
	}

	/// <summary>
	/// 	Sends an email to this user with a link for reseting his password.
	/// </summary>
	/// <param name="tokenBuilder">The app's <see cref="ITokenBuilder"/>.</param>
	/// <param name="baseAddress">The app's base address.</param>
	public void SendPasswordRedefinitionMessage(
		ITokenBuilder tokenBuilder,
		string baseAddress
	)
	{
		var message = EmailSenderIdentityUser.PasswordRedefinitionMessageBuilder(
			Email!,
			EmailSenderIdentityUser.PasswordRedefinitionEndpoint
				+ tokenBuilder.BuildCustomTypeToken(
					new[] { new Claim(ClaimTypes.Email, Email!) },
					JwtCustomTypes.PasswordRedefinition
				)
		);

		SendMessage(message);
	}

	private void SendMessage(MimeMessage message)
	{
		using (var client = new SmtpClient()) {
			client.Connect(EmailSenderIdentityUser.SmtpUri);
			client.Authenticate(EmailSenderIdentityUser.SmtpCredentials);
			client.Send(message);
			client.Disconnect(true);
		}
	}
}