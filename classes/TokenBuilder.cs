using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PortunusAdiutor;

///	<summary>
///		Default ITokenBuilder implementation.
///	</summary>
///	<remarks>
///		Prefer to use any <see cref="WebBuilderExtensions"/>
///		to inject the <see cref="ITokenBuilder"/> dependency.
///	</remarks>
public class TokenBuilder : ITokenBuilder
{
	private readonly SymmetricSecurityKey _signingKey;
	private readonly SymmetricSecurityKey _encryptKey;


	///	<summary>
	///		Initiazlize a new instance of <see cref="TokenBuilder"/>.
	///	</summary>
	/// <param name="signingKey">Secret string used for the token signing.</param>
	/// <param name="encryptKey">Secret string used for the token encryption.</param>
	public TokenBuilder(
		SymmetricSecurityKey signingKey,
		SymmetricSecurityKey encryptKey
	)
	{
		_encryptKey = encryptKey;
		_signingKey = signingKey;
	}

	///	<summary>
	///		Builds a token with the specified parameters.
	///	</summary>
	///	<param name="tokenDescriptor">Token generation configuration.</param>
	///	<returns>
	///		A JWT string containing the provided <paramref name="tokenDescriptor"/>
	///		configurations.
	///	</returns>
	public string BuildToken(SecurityTokenDescriptor tokenDescriptor)
	{
		tokenDescriptor.SigningCredentials = new SigningCredentials(
			_signingKey,
			SecurityAlgorithms.HmacSha256Signature
		);
		tokenDescriptor.EncryptingCredentials = new EncryptingCredentials(
			_encryptKey,
			JwtConstants.DirectKeyUseAlg,
			SecurityAlgorithms.Aes128CbcHmacSha256
		);

		var handler = new JwtSecurityTokenHandler();
		return handler.WriteToken(handler.CreateToken(tokenDescriptor));
	}

	///	<summary>
	///		Builds a token with the specified claims.
	///	</summary>
	///	<param name="claims">The claims the token should carry.</param>
	///	<returns>
	///		A JWT string containing the provided <paramref name="claims"/>.
	///	</returns>
	public string BuildToken(Claim[] claims)
	{
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Expires = DateTime.UtcNow.AddHours(2),
			Subject = new(claims)
		};
		return BuildToken(tokenDescriptor);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="claims"></param>
	/// <param name="tokenType"></param>
	/// <returns></returns>
	public string BuildCustomTypeToken(Claim[] claims, string tokenType)
	{
		return BuildToken(new SecurityTokenDescriptor
		{
			Subject = new(claims),
			TokenType = tokenType,
			Expires = DateTime.UtcNow.AddMinutes(15),
		});
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="token"></param>
	/// <param name="validationParameters"></param>
	/// <returns></returns>
	public Claim[]? ValidateToken(
		string token,
		TokenValidationParameters? validationParameters = null
	)
	{
		try {
			validationParameters ??= new TokenValidationParameters
			{
				ValidateIssuer = false,
				ValidateAudience = false
			};
			validationParameters.ValidateIssuerSigningKey = true;
			validationParameters.IssuerSigningKey = _signingKey;
			validationParameters.TokenDecryptionKey = _encryptKey;

			var handler = new JwtSecurityTokenHandler();
			var claims = handler.ValidateToken(
				token,
				validationParameters,
				out var tokenSecure
			);

			return claims.Claims.ToArray();
		} catch {
			return null;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="token"></param>
	/// <param name="tokenType"></param>
	/// <param name="validationParameters"></param>
	/// <returns></returns>
	public Claim[]? ValidateCustomTypeToken(
		string token,
		string tokenType,
		TokenValidationParameters? validationParameters = null
	)
	{
		var handler = new JwtSecurityTokenHandler();

		validationParameters ??= new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
		};

		validationParameters.ValidTypes = new List<string> { tokenType };

		return ValidateToken(token, validationParameters);
	}

}
