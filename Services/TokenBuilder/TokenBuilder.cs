using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PortunusAdiutor.Services.TokenBuilder;

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

	///	<inheritdoc/>
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

	///	<inheritdoc/>
	public string BuildToken(Claim[] claims)
	{
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Expires = DateTime.UtcNow.AddHours(2),
			Subject = new(claims)
		};
		return BuildToken(tokenDescriptor);
	}

	///	<inheritdoc/>
	public string BuildToken(IDictionary<string, object> claims)
	{
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Expires = DateTime.UtcNow.AddHours(2),
			Claims = claims
		};
		return BuildToken(tokenDescriptor);
	}

	///	<inheritdoc/>
	public string BuildCustomTypeToken<TUser, TKey>(TUser user, string tokenType)
	where TUser : IdentityUser<TKey>
	where TKey : IEquatable<TKey>
	{
		return BuildToken(new SecurityTokenDescriptor
		{
			Subject = new(new[] { new Claim(ClaimTypes.PrimarySid, user.Id.ToString()!) }),
			TokenType = tokenType,
			Expires = DateTime.UtcNow.AddMinutes(15),
		});
	}

	///	<inheritdoc/>
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

	///	<inheritdoc/>
	public string? ValidateCustomTypeToken(
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

		return ValidateToken(token, validationParameters)?.First(c => c.Type == ClaimTypes.PrimarySid).Value;
	}

}
