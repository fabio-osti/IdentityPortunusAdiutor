using System.Net;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using MessageBuilder = System.Func<string, string, MimeKit.MimeMessage>;

namespace PortunusAdiutor;

static public partial class WebBuilderExtensions
{
	/// <summary>
	/// 	Gets <see cref="MailLinkPosterParams"/> according to, in this order of precedence:
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Nº</term>
	/// 			<term>URI</term>
	/// 			<term>Username</term>
	/// 			<term>Password</term>
	/// 			<term>Email Validation Endpoint</term>
	/// 			<term>Password Redefinition Endpoint</term>
	/// 			<term>Email Validation Message Builder</term>
	/// 			<term>Password Redefinition Message Builder</term>
	/// 		</listheader>
	/// 		<item>
	/// 			<term>1</term>
	/// 			<term><paramref name="smtpUri"/></term>
	/// 			<term><paramref name="smtpUser"/></term>
	/// 			<term><paramref name="smtpPassword"/></term>
	/// 			<term><paramref name="emailConfirmationEndpoint"/></term>
	/// 			<term><paramref name="passwordRedefinitionEndpoint"/></term>
	/// 			<term><paramref name="EmailConfirmationMessageBuilder"/></term>
	/// 			<term><paramref name="passwordRedefinitionMessageBuilder"/></term>
	/// 		</item>
	/// 		<item>
	/// 			<term>2</term>
	/// 			<term>
	/// 				"SMTP_URI" configuration's value.
	/// 			</term>
	/// 			<term>
	/// 				"SMTP_USER" configuration's value.
	/// 			</term>
	/// 			<term>
	/// 				"SMTP_PSWRD" configuration's value.
	/// 			</term>
	/// 			<term>
	/// 				"SMTP_EVE" configuration's value.
	/// 			</term>
	/// 			<term>
	/// 				"SMTP_PRE" configuration's value.
	/// 			</term>
	/// 			<term>NULL</term>
	/// 			<term>NULL</term>
	/// 		</item>
	/// 		<item>
	/// 			<term>3</term>
	/// 			<term><see cref="MailLinkPosterParams.defaultSmtpUriString"/></term>
	/// 			<term>NULL</term>
	/// 			<term>NULL</term>
	/// 			<term><see cref="MailLinkPosterParams.defaultEmailConfirmationEndpoint"/></term>
	/// 			<term><see cref="MailLinkPosterParams.defaultPasswordRedefinitionEndpoint"/></term>
	/// 			<term><see cref="MailLinkPosterParams.defaultEmailConfirmationMessageBuilder"/></term>
	/// 			<term><see cref="MailLinkPosterParams.defaultPasswordRedefinitionMessageBuilder"/></term>
	/// 		</item>
	/// 	</list>
	/// </summary>
	/// <param name="builder">
	/// 	The <see cref="WebApplicationBuilder"/> to access appsetting.json.
	/// </param>
	/// <param name="smtpUri">SMTP address.</param>
	/// <param name="smtpUser">SMTP username.</param>
	/// <param name="smtpPassword">SMTP password.</param>
	/// <param name="emailConfirmationEndpoint">
	/// 	Endpoint that should be sent to user's email for validation.
	/// </param>
	/// <param name="passwordRedefinitionEndpoint">
	/// 	Endpoint that should be sent to user's email for password redefinition.
	/// </param>
	/// <param name="EmailConfirmationMessageBuilder">
	///		Builder of email validation messages.
	///	</param>
	/// <param name="passwordRedefinitionMessageBuilder">
	///		Builder of password redefinition messages.
	///	</param>
	///	<returns>
	///		Parameters to be used for configuration.
	///	</returns>
	public static MailLinkPosterParams GetMailLinkPosterParams(
		this WebApplicationBuilder builder,
		string? smtpUri = null,
		string? smtpUser = null,
		string? smtpPassword = null,
		string? emailConfirmationEndpoint = null,
		string? passwordRedefinitionEndpoint = null,
		MessageBuilder? EmailConfirmationMessageBuilder = null,
		MessageBuilder? passwordRedefinitionMessageBuilder = null
	)
	{
		var mailParams = new MailLinkPosterParams();

		smtpUri ??= builder.Configuration["SMTP_URI"];
		if (smtpUri != null) {
			mailParams.SmtpUri = new(smtpUri);
		}

		smtpUser ??= builder.Configuration["SMTP_USER"];
		if (smtpUser != null) {
			smtpPassword ??= builder.Configuration["SMTP_PSWRD"];
			mailParams.SmtpCredentials =
				new NetworkCredential(smtpUser, smtpPassword);
		}

		emailConfirmationEndpoint ??= builder.Configuration["SMTP_EVE"];
		if (emailConfirmationEndpoint != null) {
			mailParams.EmailConfirmationEndpoint = emailConfirmationEndpoint;
		}

		passwordRedefinitionEndpoint ??= builder.Configuration["SMTP_PRE"];
		if (passwordRedefinitionEndpoint != null) {
			mailParams.PasswordRedefinitionEndpoint = passwordRedefinitionEndpoint;
		}

		if (EmailConfirmationMessageBuilder != null) {
			mailParams.EmailConfirmationMessageBuilder = EmailConfirmationMessageBuilder;
		}

		if (passwordRedefinitionMessageBuilder != null) {
			mailParams.EmailConfirmationMessageBuilder = passwordRedefinitionMessageBuilder;
		}

		return mailParams;
	}

	/// <summary>
	/// 	Gets <see cref="MailCodePosterParams"/> according to, in this order of precedence:
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Nº</term>
	/// 			<term>URI</term>
	/// 			<term>Username</term>
	/// 			<term>Password</term>
	/// 			<term>Email Validation Message Builder</term>
	/// 			<term>Password Redefinition Message Builder</term>
	/// 		</listheader>
	/// 		<item>
	/// 			<term>1</term>
	/// 			<term><paramref name="smtpUri"/></term>
	/// 			<term><paramref name="smtpUser"/></term>
	/// 			<term><paramref name="smtpPassword"/></term>
	/// 			<term><paramref name="emailConfirmationMessageBuilder"/></term>
	/// 			<term><paramref name="passwordRedefinitionMessageBuilder"/></term>
	/// 		</item>
	/// 		<item>
	/// 			<term>2</term>
	/// 			<term>
	/// 				"SMTP_URI" configuration's value.
	/// 			</term>
	/// 			<term>
	/// 				"SMTP_USER" configuration's value.
	/// 			</term>
	/// 			<term>
	/// 				"SMTP_PSWRD" configuration's value.
	/// 			</term>
	/// 			<term>NULL</term>
	/// 			<term>NULL</term>
	/// 		</item>
	/// 		<item>
	/// 			<term>3</term>
	/// 			<term><see cref="MailLinkPosterParams.defaultSmtpUriString"/></term>
	/// 			<term><see cref="MailLinkPosterParams.defaultEmailConfirmationMessageBuilder"/></term>
	/// 			<term><see cref="MailLinkPosterParams.defaultPasswordRedefinitionMessageBuilder"/></term>
	/// 		</item>
	/// 	</list>
	/// </summary>
	/// <param name="builder">
	/// 	The <see cref="WebApplicationBuilder"/> to access appsetting.json.
	/// </param>
	/// <param name="smtpUri">SMTP address.</param>
	/// <param name="smtpUser">SMTP username.</param>
	/// <param name="smtpPassword">SMTP password.</param>
	/// <param name="emailConfirmationMessageBuilder">
	///		Builder of email validation messages.
	///	</param>
	/// <param name="passwordRedefinitionMessageBuilder">
	///		Builder of password redefinition messages.
	///	</param>
	///	<returns>
	///		Parameters to be used for configuration.
	///	</returns>
	public static MailCodePosterParams GetMailCodePosterParams(
		this WebApplicationBuilder builder,
		string? smtpUri = null,
		string? smtpUser = null,
		string? smtpPassword = null,
		MessageBuilder? emailConfirmationMessageBuilder = null,
		MessageBuilder? passwordRedefinitionMessageBuilder = null
	)
	{
		var mailParams = new MailCodePosterParams();

		smtpUri ??= builder.Configuration["SMTP_URI"];
		if (smtpUri != null) {
			mailParams.SmtpUri = new(smtpUri);
		}

		smtpUser ??= builder.Configuration["SMTP_USER"];
		if (smtpUser != null) {
			smtpPassword ??= builder.Configuration["SMTP_PSWRD"];
			mailParams.SmtpCredentials =
				new NetworkCredential(smtpUser, smtpPassword);
		}

		if (emailConfirmationMessageBuilder != null) {
			mailParams.ConfirmEmailMessageBuilder = emailConfirmationMessageBuilder;
		}

		if (passwordRedefinitionMessageBuilder != null) {
			mailParams.ConfirmEmailMessageBuilder = passwordRedefinitionMessageBuilder;
		}

		return mailParams;
	}

	/// <summary>
	/// 	Configures <see cref="MailLinkPoster{TUser, TKey}"/> service.
	/// </summary>
	/// <typeparam name="TUser"><see cref="IdentityUser{TKey}"/> type to be used.</typeparam>
	/// <typeparam name="TKey">The type used for the primary key for the <typeparamref name="TUser"/>.</typeparam>
	/// <param name="builder">The app's web builder.</param>
	/// <param name="mailParams">Parameters to be used.</param>	
	/// <remarks>
	/// 	Should be called after an overload of ConfigureTokenServices.
	/// </remarks>
	public static void ConfigureMailLinkPoster<TUser, TKey>(
		WebApplicationBuilder builder,
		MailLinkPosterParams mailParams
	)
		where TUser : IdentityUser<TKey>
		where TKey : IEquatable<TKey>
	{
		builder.Services.AddSingleton<IMailPoster<TUser, TKey>>(
			e => new MailLinkPoster<TUser, TKey>(
				mailParams,
				e.GetRequiredService<ITokenBuilder>()
			)
		);
	}

	/// <summary>
	/// 	Configures <see cref="MailCodePoster{TUser, TRole, TKey}"/> service.
	/// </summary>
	/// <typeparam name="TUser"><see cref="IdentityUser{TKey}"/> type to be used.</typeparam>
	/// <typeparam name="TRole"><see cref="IdentityRole{TKey}"/> type to be used.</typeparam>
	/// <typeparam name="TKey">The type used for the primary key for the <typeparamref name="TUser"/>.</typeparam>
	/// <param name="builder">The app's web builder.</param>
	/// <param name="mailParams">Parameters to be used.</param>
	/// <remarks>
	/// 	Should be called after an overload of ConfigureTokenServices.
	/// </remarks>
	public static void ConfigureMailCodePoster<TUser, TRole, TKey>(
		WebApplicationBuilder builder,
		MailCodePosterParams mailParams
	)
		where TUser : IdentityUser<TKey>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
	{
		builder.Services.AddSingleton<IMailPoster<TUser, TKey>>(
			e => new MailCodePoster<TUser, TRole, TKey>(
				mailParams,
				e.GetRequiredService<CodeBasedIdentityDbContext<TUser, TRole, TKey>>()
			)
		);
	}
}