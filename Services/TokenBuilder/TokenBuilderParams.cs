using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace PortunusAdiutor.Services.TokenBuilder;

/// <summary>
/// 	Represents the parameters needed for building a token.
/// </summary>
public class TokenBuilderParams
{
	/// <summary>
	/// 	Key used for signing the token.
	/// </summary>
	public required SymmetricSecurityKey SigningKey { get; set; }
	/// <summary>
	/// 	Key used for encrypting the token.
	/// </summary>
	public required SymmetricSecurityKey EncryptionKey { get; set; }
	/// <summary>
	/// 	Action to set the <see cref="JwtBearerOptions"/>.
	/// </summary>
	/// <remarks>
	/// 	If this is set, <see cref="ValidationParams"/> is ignored.
	/// </remarks>
	public Action<JwtBearerOptions>? JwtConfigurator { get; set; }
	/// <summary>
	/// 	Action to set the parameters of validation.
	/// </summary>
	/// <remarks>
	/// 	If <see cref="JwtConfigurator"/> is set, this is ignored.
	/// </remarks>
	public TokenValidationParameters? ValidationParams { get; set; }
}