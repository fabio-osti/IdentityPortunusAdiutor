using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
		this IServiceCollection builder,
		byte[] key
	)
	{
		return builder
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
		this IServiceCollection builder,
		byte[] key,
		Action<JwtBearerOptions> configurator
	)
	{
		var hijackedConfigurator = (JwtBearerOptions opt) =>
		{
			configurator(opt);
			opt.TokenValidationParameters.ValidateIssuerSigningKey = true;
			opt.TokenValidationParameters.IssuerSigningKey = 
				new SymmetricSecurityKey(key);
		};
		return builder
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
		this IServiceCollection builder,
		byte[] key,
		TokenValidationParameters validationParams
	)
	{
		validationParams.ValidateIssuerSigningKey = true;
		validationParams.IssuerSigningKey = new SymmetricSecurityKey(key);
		return builder
			.AddSingleton<ITokenBuilder>(new TokenBuilder(key))
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(opt =>
			{
				opt.SaveToken = true;
				opt.TokenValidationParameters = validationParams;
			});
	}
}