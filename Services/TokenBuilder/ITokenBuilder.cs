using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

namespace PortunusAdiutor.Services.TokenBuilder;

/// <summary>
/// 	Defines all necessary methods for token building.
/// </summary>
public interface ITokenBuilder
{
	/// <summary>
	/// 	Builds a token using <paramref name="tokenDescriptor"/>.
	/// </summary>
	/// <param name="tokenDescriptor">Descriptor of the build token.</param>
	/// <returns>A JWT describing <paramref name="tokenDescriptor"/>.</returns>
	string BuildToken(SecurityTokenDescriptor tokenDescriptor);
	/// <summary>
	/// 	Builds a token with <paramref name="claims"/>.
	/// </summary>
	/// <param name="claims">Claims of the build token.</param>
	/// <returns>A JWT containing <paramref name="claims"/>.</returns>
	string BuildToken(Claim[] claims);
	/// <summary>
	/// 	Builds a token with <paramref name="claims"/>.
	/// </summary>
	/// <param name="claims">Claims of the build token.</param>
	/// <returns>A JWT containing <paramref name="claims"/>.</returns>
	string BuildToken(IDictionary<string, object> claims);
	/// <summary>
	/// 	Validates and gets the claims contained by <paramref name="token"/>.
	/// </summary>
	/// <param name="token">Token to be validated.</param>
	/// <param name="validationParameters">Parameter of validation.</param>
	/// <returns>Claims of <paramref name="token"/>.</returns>
	Claim[]? ValidateToken(
		string token,
		TokenValidationParameters? validationParameters = null
	);
}