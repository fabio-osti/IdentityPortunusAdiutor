using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
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
	/// <param name="signingKey"></param>
	/// <param name="encryptionKey"></param>
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
	///		and 
	///		<see cref="TokenValidationParameters.IssuerSigningKey"/>
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
	/// <param name="signingKey"></param>
	/// <param name="encryptionKey"></param>
	///	<param name="validationParams">
	///		<see cref="SecurityTokenHandler"/>'s validation parameters.
	///	</param>
	///	<remarks>
	///		<see cref="TokenValidationParameters.ValidateIssuerSigningKey"/>
	///		and 
	///		<see cref="TokenValidationParameters.IssuerSigningKey"/>
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
	/// 	Sets Pbkdf2 parameters according to appsetting.json definitions
	/// 	or, if undefined, default values.
	/// </summary>
	/// <param name="builder">
	/// 	The <see cref="WebApplicationBuilder"/> to access appsetting.json.
	/// </param>
	/// <param name="iterCount"></param>
	/// <param name="hashedSize"></param>
	public static void SetPbkdf2Params(
		this WebApplicationBuilder builder,
		int? iterCount = null,
		int? hashedSize = null)
	{
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
	/// 	Sets Smtp parameters according to appsetting.json definitions
	/// 	or, if undefined, default values.
	/// </summary>
	/// <param name="builder">
	/// 	The <see cref="WebApplicationBuilder"/> to access appsetting.json.
	/// </param>
	/// <param name="uri"></param>
	/// <param name="user"></param>
	/// <param name="password"></param>
	public static void SetSmtpParams(
		this WebApplicationBuilder builder,
		string? uri = null,
		string? user = null,
		SecureString? password = null
	)
	{
		uri ??= builder.Configuration["SMTP_URI"];
		if (uri != null) {
			EmailSenderIdentityUser.SmtpUri = new(uri);
		}
		
		user ??= builder.Configuration["SMTP_USER"];
		EmailSenderIdentityUser.SmtpCredentials =
			new NetworkCredential(user,password);
	}
}
