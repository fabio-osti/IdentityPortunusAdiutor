using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

namespace PortunusAdiutor.Services.TokenBuilder;

/// <summary>
/// 	Defines all necessary methods for building a token.
/// </summary>
public interface ITokenBuilder
{
	/// <summary>
	/// 	Builds a token using the <paramref name="tokenDescriptor"/>.
	/// </summary>
	/// <param name="tokenDescriptor">Information used to create a security token.</param>
	/// <returns>JWT describing the <paramref name="tokenDescriptor"/>.</returns>
	string BuildToken(SecurityTokenDescriptor tokenDescriptor);
	/// <summary>
	/// 	Builds a token using the <paramref name="claims"/>.
	/// </summary>
	/// <param name="claims">Array of user claims.</param>
	/// <returns>JWT containing the <paramref name="claims"/>.</returns>
	string BuildToken(Claim[] claims);
	/// <summary>
	/// 	Builds a token using the <paramref name="claims"/>.
	/// </summary>
	/// <param name="claims">Dict of user claims.</param>
	/// <returns>JWT containing the <paramref name="claims"/>.</returns>
	string BuildToken(IDictionary<string, object> claims);
	/// <summary>
	/// 	Validates a token built by an instance of this class.
	/// </summary>
	/// <param name="token">Token to be validated.</param>
	/// <param name="validatedToken">The <see cref="JwtSecurityToken"/> that was validated.</param>
	/// <param name="validationParameters">Parameters to validate de token with.</param>
	/// <returns>The claims contained by the token.</returns>
	public IEnumerable<Claim>? ValidateToken(
		string token,
		out SecurityToken? validatedToken,
		TokenValidationParameters? validationParameters = null
	);
}