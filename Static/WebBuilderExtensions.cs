using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security;

namespace PortunusAdiutor;

///	<summary>
///		Helper to configure the services.
///	</summary>
static public class WebBuilderExtensions
{
	///	<summary>
	///		Configures all needed services for token authentication.
	///	</summary>
	///	<param name="builder">The services injector.</param>
	///	<param name="signingKey">The secret key used for signing.</param>
	/// <param name="encryptionKey">The secret key used for encryption.</param>
	///	<returns>
	///		The <see cref="AuthenticationBuilder"/> for further configurations.
	///	</returns>
	static public AuthenticationBuilder ConfigureTokenServices(
		this WebApplicationBuilder builder,
		byte[] signingKey,
		byte[] encryptionKey
	)
	{
		var signSymKey = new SymmetricSecurityKey(signingKey);
		var cryptSymKey = new SymmetricSecurityKey(encryptionKey);

		return builder.Services
			.AddSingleton<ITokenBuilder>(new TokenBuilder(signSymKey, cryptSymKey))
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(opt =>
			{
				opt.SaveToken = true;
				opt.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = signSymKey,
					TokenDecryptionKey = cryptSymKey,
					ValidateAudience = false,
					ValidateIssuer = false,
				};
			});
	}

	///	<summary>
	///		Configures all needed services for token authentication.
	///	</summary>
	///	<param name="builder">The services injector.</param>
	///	<param name="signingKey">The secret key used for signing.</param>
	/// <param name="encryptionKey">The secret key used for encryption.</param>
	///	<param name="configurator">
	///		Configures the <see cref="JwtBearerOptions"/>
	///	</param>
	///	<remarks>
	///		<see cref="TokenValidationParameters.ValidateIssuerSigningKey"/>
	///		and <see cref="TokenValidationParameters.IssuerSigningKey"/>
	///		and <see cref="TokenValidationParameters.TokenDecryptionKey"/>
	///		will be overwritten.
	///	</remarks>
	///	<returns>
	///		The <see cref="AuthenticationBuilder"/> for further configurations.
	///	</returns>
	static public AuthenticationBuilder ConfigureTokenServices(
		this WebApplicationBuilder builder,
		byte[] signingKey,
		byte[] encryptionKey,
		Action<JwtBearerOptions> configurator
	)
	{
		var signSymKey = new SymmetricSecurityKey(signingKey);
		var cryptSymKey = new SymmetricSecurityKey(encryptionKey);

		var hijackedConfigurator = (JwtBearerOptions opt) =>
		{
			configurator(opt);
			opt.TokenValidationParameters.ValidateIssuerSigningKey = true;
			opt.TokenValidationParameters.IssuerSigningKey = signSymKey;
			opt.TokenValidationParameters.TokenDecryptionKey = cryptSymKey;
		};

		return builder.Services
			.AddSingleton<ITokenBuilder>(new TokenBuilder(signSymKey, cryptSymKey))
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(hijackedConfigurator);
	}

	///	<summary>
	///		Configures all needed services for token authentication.
	///	</summary>
	///	<param name="builder">The services injector.</param>
	///	<param name="signingKey">The secret key used for signing.</param>
	/// <param name="encryptionKey">The secret key used for encryption.</param>
	///	<param name="validationParams">
	///		<see cref="SecurityTokenHandler"/>'s validation parameters.
	///	</param>
	///	<remarks>
	///		<see cref="TokenValidationParameters.ValidateIssuerSigningKey"/>
	///		and <see cref="TokenValidationParameters.IssuerSigningKey"/>
	///		and <see cref="TokenValidationParameters.TokenDecryptionKey"/>
	///		will be overwritten.
	///	</remarks>
	///	<returns>
	///		The <see cref="AuthenticationBuilder"/> for further configurations.
	///	</returns>
	static public AuthenticationBuilder ConfigureTokenServices(
		this WebApplicationBuilder builder,
		byte[] signingKey,
		byte[] encryptionKey,
		TokenValidationParameters validationParams
	)
	{
		var signSymKey = new SymmetricSecurityKey(signingKey);
		var cryptSymKey = new SymmetricSecurityKey(encryptionKey);
		validationParams.ValidateIssuerSigningKey = true;
		validationParams.IssuerSigningKey = signSymKey;
		validationParams.TokenDecryptionKey = cryptSymKey;

		return builder.Services
			.AddSingleton<ITokenBuilder>(new TokenBuilder(signSymKey, cryptSymKey))
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(opt =>
			{
				opt.SaveToken = true;
				opt.TokenValidationParameters = validationParams;
			});
	}

	/// <summary>
	/// 	Sets Pbkdf2 parameters according to, in this order of precedence:
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Nº</term>
	/// 			<term>Pseudo Random Function</term>
	/// 			<term>Iteration Count</term>
	/// 			<term>Hashed Size Result</term>
	/// 		</listheader>
	/// 		<item>
	/// 			<term>1</term>
	/// 			<term><paramref name="prf"/></term>
	/// 			<term><paramref name="iterCount"/></term>
	/// 			<term><paramref name="hashedSize"/></term>
	/// 		</item>
	/// 		<item>
	/// 			<term>2</term>
	/// 			<term>
	/// 				"PBKDF2_PRF_ENUM" configuration's value.
	/// 			</term>
	/// 			<term>
	/// 				"PBKDF2_ITER_COUNT" configuration's value.
	/// 			</term>
	/// 			<term>
	/// 				"PBKDF2_HASHED_SIZE" configuration's value.
	/// 			</term>
	/// 		</item>
	/// 		<item>
	/// 			<term>3</term>
	/// 			<term><see cref="KeyDerivationPrf.HMACSHA256"/></term>
	/// 			<term><see cref="Pbkdf2IdentityUser.defaultIterCount"/></term>
	/// 			<term><see cref="Pbkdf2IdentityUser.defaultHashedSize"/></term>
	/// 		</item>
	/// 	</list>
	/// </summary>
	/// <param name="builder">
	/// 	The <see cref="WebApplicationBuilder"/> to access appsetting.json.
	/// </param>
	/// <param name="prf">Which pseudo random function should be used for key derivation.</param>
	/// <param name="iterCount">How many iterations should be run.</param>
	/// <param name="hashedSize">The size of the derived key.</param>
	public static void SetPbkdf2Params(
		this WebApplicationBuilder builder,
		KeyDerivationPrf? prf = null,
		int? iterCount = null,
		int? hashedSize = null
	)
	{
		if (prf != null) {
			Pbkdf2IdentityUser.PseudoRandomFunction = prf.Value;
		} else if (
			Enum.TryParse<KeyDerivationPrf>(
				builder.Configuration["PBKDF2_PRF_ENUM"], 
				out var _prf
			)
		) {
			Pbkdf2IdentityUser.PseudoRandomFunction = _prf;
		}

		if (iterCount != null) {
			Pbkdf2IdentityUser.IterationCount = iterCount.Value;
		} else if (
			int.TryParse(
				builder.Configuration["PBKDF2_ITER_COUNT"],
				out var _iterCount
			)
		) {
			Pbkdf2IdentityUser.IterationCount = _iterCount;
		}

		if (hashedSize != null) {
			Pbkdf2IdentityUser.HashedSize = hashedSize.Value;
		} else if (
			int.TryParse(
				builder.Configuration["PBKDF2_HASHED_SIZE"],
				out var _hashedSize
			)
		) {
			Pbkdf2IdentityUser.HashedSize = _hashedSize;
		}
	}

	/// <summary>
	/// 	Sets Smtp parameters according to, in this order of precedence:
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Nº</term>
	/// 			<term>Uri</term>
	/// 			<term>Username</term>
	/// 			<term>Password</term>
	/// 			<term>Email Validation Endpoint</term>
	/// 			<term>Password Redefinition Endpoint</term>
	/// 		</listheader>
	/// 		<item>
	/// 			<term>1</term>
	/// 			<term><paramref name="smtpUri"/></term>
	/// 			<term><paramref name="smtpUser"/></term>
	/// 			<term><paramref name="smtpPassword"/></term>
	/// 			<term><paramref name="emailValidationEndpoint"/></term>
	/// 			<term><paramref name="passwordRedefinitionEndpoint"/></term>
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
	/// 		</item>
	/// 		<item>
	/// 			<term>3</term>
	/// 			<term><see cref="EmailingIdentityUser.defaultSmtpUriString"/></term>
	/// 			<term>NULL</term>
	/// 			<term>NULL</term>
	/// 		</item>
	/// 	</list>
	/// </summary>
	/// <param name="builder">
	/// 	The <see cref="WebApplicationBuilder"/> to access appsetting.json.
	/// </param>
	/// <param name="smtpUri">SMTP address.</param>
	/// <param name="smtpUser">SMTP username.</param>
	/// <param name="smtpPassword">SMTP password.</param>
	/// <param name="emailValidationEndpoint">
	/// 	Endpoint that should be sent to user's email for validation.
	/// </param>
	/// <param name="passwordRedefinitionEndpoint">
	/// 	Endpoint that should be sent to user's email for password redefinition.
	/// </param>
	public static void SetSmtpParams(
		this WebApplicationBuilder builder,
		string? smtpUri = null,
		string? smtpUser = null,
		string? smtpPassword = null,
		string? emailValidationEndpoint = null,
		string? passwordRedefinitionEndpoint = null
	)
	{
		smtpUri ??= builder.Configuration["SMTP_URI"];
		if (smtpUri != null) {
			EmailingIdentityUser.SmtpUri = new(smtpUri);
		}

		smtpUser ??= builder.Configuration["SMTP_USER"];
		if (smtpUser != null) {
			smtpPassword ??= builder.Configuration["SMTP_PSWRD"];
			EmailingIdentityUser.SmtpCredentials =
				new NetworkCredential(smtpUser, smtpPassword);
		}

		emailValidationEndpoint ??= builder.Configuration["SMTP_EVE"];
		if (emailValidationEndpoint != null) {
			EmailingIdentityUser.EmailValidationEndpoint = emailValidationEndpoint;
		}

		passwordRedefinitionEndpoint ??= builder.Configuration["SMTP_PRE"];
		if (passwordRedefinitionEndpoint != null) {
			EmailingIdentityUser.PasswordRedefinitionEndpoint = passwordRedefinitionEndpoint;
		}
	}
}
