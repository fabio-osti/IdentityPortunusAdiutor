using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

///	<summary>
///		Interface to define the TokenBuilder service necessary methods.
///	</summary>
public interface ITokenBuilder
{
	///	<summary>
	///		Builds a token with the specified parameters.
	///	</summary>
	///	<param name="tokenDescriptor">Token generation configuration.</param>
	///	<returns>
	///		A JWT string containing the provided <paramref name="tokenDescriptor"/>
	///		configurations.
	///	</returns>
	string BuildToken(SecurityTokenDescriptor tokenDescriptor);

	///	<summary>
	///		Builds a token with the specified claims.
	///	</summary>
	///	<param name="claims">The claims the token should carry.</param>
	///	<returns>
	///		A JWT string containing the provided <paramref name="claims"/>.
	///	</returns>
	string BuildToken(Claim[] claims);

	///	<summary>
	///		Builds a token with the specified claims.
	///	</summary>
	///	<param name="claims">The claims the token should carry.</param>
	///	<returns>
	///		A JWT string containing the provided <paramref name="claims"/>.
	///	</returns>
	string BuildToken(IDictionary<string, object> claims);

	/// <summary>
	/// 	Builds a token with a custom header "typ".
	/// </summary>
	/// <param name="user"></param>
	/// <param name="tokenType">The token header "typ" value.</param>
	///	<returns>
	///		A JWT string containing the provided 
	///		<paramref name="user"/> and <paramref name="tokenType"/>.
	///	</returns>
	string BuildCustomTypeToken<TUser, TKey>(TUser user, string tokenType)
	where TUser : IdentityUser<TKey>
	where TKey : IEquatable<TKey>;

	/// <summary>
	/// 	Validates a <paramref name="token"/> 
	/// 	with <paramref name="validationParameters"/>
	/// </summary>
	/// <param name="token">Token to validate.</param>
	/// <param name="validationParameters">Parameters of validation.</param>
	/// <returns>The <paramref name="token"/> claims.</returns>
	Claim[]? ValidateToken(
		string token, 
		TokenValidationParameters? validationParameters = null
	);

	/// <summary>
	/// 	Validates a <paramref name="token"/> 
	/// 	with <paramref name="validationParameters"/>
	/// 	and a custom <paramref name="tokenType"/>
	/// </summary>
	/// <param name="token">Token to validate.</param>
	/// <param name="tokenType">Type for validation.</param>
	/// <param name="validationParameters">Parameters of validation.</param>
	/// <returns>The <paramref name="token"/> <see cref="ClaimTypes.PrimarySid"/> claim value.</returns>
	string? ValidateCustomTypeToken(
		string token,
		string tokenType,
		TokenValidationParameters? validationParameters = null
	);
}