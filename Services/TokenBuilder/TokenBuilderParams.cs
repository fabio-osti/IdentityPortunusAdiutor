using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace PortunusAdiutor.Services.TokenBuilder;

/// <summary>
/// 	Class representing the parameters for building and validating tokens.
/// </summary>
/// <remarks>
/// 	<see cref="SigningKey"/> and <see cref="EncryptionKey"/>
/// </remarks>
public class TokenBuilderParams
{
/// <summary>
/// 	Key for signing tokens.
/// </summary>
	public required SymmetricSecurityKey SigningKey { get; set; }
/// <summary>
/// 	Key for encrypting tokens.
/// </summary>
	public required SymmetricSecurityKey EncryptionKey { get; set; }
	/// <summary>
	///  	Configurator of <see cref="JwtBearerOptions"/>.
	/// </summary>
	/// <remarks>
	/// 	If this is defined, <see cref="ValidationParams"/> will be ignored.
	/// </remarks>
	public Action<JwtBearerOptions>? JwtConfigurator { get; set; }
	/// <summary>
	/// 	Parameters for token validation.
	/// </summary>
	/// <remarks>
	/// 	If <see cref="ValidationParams"/> is defined, this will be ignored.
	/// </remarks>
	public TokenValidationParameters? ValidationParams { get; set; }
}