using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace PortunusAdiutor;

/// <summary>
/// 	Helper to configure the services.
/// </summary>
static public class WebBuilderExtensions
{
	/// <summary>
	/// 	Configures all needed services for token authentication.
	/// </summary>
	/// <param name="builder">The services injector.</param>
	/// <param name="key">The secret key used for encription.</param>
	/// <returns>
	/// 	The <see cref="AuthenticationBuilder"/> for further configurations.
	/// </returns>
	static public AuthenticationBuilder ConfigureTokenServices(
		this WebApplicationBuilder builder,
		byte[] key
	)
	{
		SetPbkdf2Params(builder);
		
		return builder.Services
			.AddSingleton<ITokenBuilder>(new TokenBuilder(key))
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(opt =>
			{
				opt.SaveToken = true;
				opt.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
				};
			});
	}

	/// <summary>
	/// 	Configures all needed services for token authentication.
	/// </summary>
	/// <param name="builder">The services injector.</param>
	/// <param name="key">The secret key used for encription.</param>
	/// <param name="configurator">
	/// 	Configures the <see cref="JwtBearerOptions"/>
	/// </param>
	/// <remarks>
	/// 	<see cref="TokenValidationParameters.ValidateIssuerSigningKey"/>
	/// 	and 
	/// 	<see cref="TokenValidationParameters.IssuerSigningKey"/>
	/// 	will be overwritten.
	/// </remarks>
	/// <returns>
	/// 	The <see cref="AuthenticationBuilder"/> for further configurations.
	/// </returns>
	static public AuthenticationBuilder ConfigureTokenServices(
		this WebApplicationBuilder builder,
		byte[] key,
		Action<JwtBearerOptions> configurator
	)
	{
		SetPbkdf2Params(builder);

		var hijackedConfigurator = (JwtBearerOptions opt) =>
		{
			configurator(opt);
			opt.TokenValidationParameters.ValidateIssuerSigningKey = true;
			opt.TokenValidationParameters.IssuerSigningKey =
				new SymmetricSecurityKey(key);
		};
		return builder.Services
			.AddSingleton<ITokenBuilder>(new TokenBuilder(key))
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(hijackedConfigurator);
	}

	/// <summary>
	/// 	Configures all needed services for token authentication.
	/// </summary>
	/// <param name="builder">The services injector.</param>
	/// <param name="key">The secret key used for encription.</param>
	/// <param name="validationParams">
	/// 	<see cref="SecurityTokenHandler"/>'s validation parameters.
	/// </param>
	/// <remarks>
	/// 	<see cref="TokenValidationParameters.ValidateIssuerSigningKey"/>
	/// 	and 
	/// 	<see cref="TokenValidationParameters.IssuerSigningKey"/>
	/// 	will be overwritten.
	/// </remarks>
	/// <returns>
	/// 	The <see cref="AuthenticationBuilder"/> for further configurations.
	/// </returns>
	static public AuthenticationBuilder ConfigureTokenServices(
		this WebApplicationBuilder builder,
		byte[] key,
		TokenValidationParameters validationParams
	)
	{
		SetPbkdf2Params(builder);

		validationParams.ValidateIssuerSigningKey = true;
		validationParams.IssuerSigningKey = new SymmetricSecurityKey(key);
		return builder.Services
			.AddSingleton<ITokenBuilder>(new TokenBuilder(key))
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(opt =>
			{
				opt.SaveToken = true;
				opt.TokenValidationParameters = validationParams;
			});
	}

	private static void SetPbkdf2Params(WebApplicationBuilder builder)
	{
		// Sets the IterationCount to what's in the appsetting.json or the default value; 
		var iterCount = builder.Configuration["PBKDF2_ITER_COUNT"];
		if (iterCount is not null) {
			IdentityUserPbkdf2<byte>.IterationCount = int.Parse(iterCount);
		}

		// Sets the HashedSize to what's in the appsetting.json or the default value; 
		var hashedSize = builder.Configuration["PBKDF2_HASHED_SIZE"];
		if (hashedSize is not null) {
			IdentityUserPbkdf2<byte>.HashedSize = int.Parse(hashedSize);
		}
	}
}