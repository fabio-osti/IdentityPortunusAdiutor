using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace PortunusAdiutor.Services.TokenBuilder;

/// <summary>
/// 	Class representing the parameters used for configuring all services at once.
/// </summary>
public class TokenBuilderParams
{
	/// <summary>
	/// 	Secret string used for the token signing.
	/// </summary>
	public required SymmetricSecurityKey SigningKey { get; set; }
	/// <summary>
	/// 	Secret string used for the token encryption.
	/// </summary>
	public required SymmetricSecurityKey EncryptionKey { get; set; }
	/// <summary>
	/// 	Configurator to be passed to the
	/// 	<see cref="JwtBearerExtensions.AddJwtBearer(AuthenticationBuilder, Action{JwtBearerOptions})"/>
	/// 	injector.
	/// </summary>
	/// <remarks>
	/// 	If this is set, <see cref="ValidationParams"/>
	/// 	is ignored
	/// </remarks>
	public Action<JwtBearerOptions>? JwtConfigurator { get; set; }
	/// <summary>
	/// 	Parameters for configuring 
	/// 	<see cref="JwtBearerExtensions.AddJwtBearer(AuthenticationBuilder, Action{JwtBearerOptions})"/>
	/// </summary>	///	<remarks>
	/// 	If <see cref="JwtConfigurator"/> is set,
	/// 	this is ignored
	/// </remarks>
	public TokenValidationParameters? ValidationParams { get; set; }
}