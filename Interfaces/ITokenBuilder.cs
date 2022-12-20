using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

///	<summary>
///		Interface to define the TokenBuilder service necessary methods.
///	</summary>
public interface ITokenBuilder
{
	///	<summary>
	///		Builds a token with the specified claims.
	///	</summary>
	///	<param name="claims">The claims the token should carry.</param>
	///	<returns>
	///		A JWT string containing the provided <paramref name="claims"/>.
	///	</returns>
	string BuildToken(Claim[] claims);
	///	<summary>
	///		Builds a token with the specified parameters.
	///	</summary>
	///	<param name="tokenDescriptor">Token generation configuration.</param>
	///	<returns>
	///		A JWT string containing the provided <paramref name="tokenDescriptor"/>
	///		configurations.
	///	</returns>
	string BuildToken(SecurityTokenDescriptor tokenDescriptor);

	/// <summary>
	/// 	Builds a token with a custom header "typ".
	/// </summary>
	/// <param name="claims"></param>
	/// <param name="tokenType"></param>
	/// <returns></returns>
	string BuildCustomTypeToken(Claim[] claims, string tokenType);

	/// <summary>
	/// 	Validates a <paramref name="token"/> 
	/// 	with <paramref name="validationParameters"/>
	/// </summary>
	/// <param name="token">Token to validate.</param>
	/// <param name="validationParameters">Parameters of validation.</param>
	/// <returns></returns>
	Claim[]? ValidateToken(
		string token, 
		TokenValidationParameters? validationParameters = null
	);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="token"></param>
	/// <param name="tokenType"></param>
	/// <param name="validationParameters"></param>
	/// <returns></returns>
	Claim[]? ValidateCustomTypeToken(
		string token,
		string tokenType,
		TokenValidationParameters? validationParameters = null
	);
}