using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using PortunusAdiutor.Extensions;

namespace PortunusAdiutor.Services.TokenBuilder;

public class TokenBuilder : ITokenBuilder
{
	private readonly TokenBuilderParams _builderParams;

	public TokenBuilder(TokenBuilderParams builderParams)
	{
		_builderParams = builderParams;
	}

	public string BuildToken(SecurityTokenDescriptor tokenDescriptor)
	{
		tokenDescriptor.SigningCredentials = new SigningCredentials(
			_builderParams.SigningKey,
			SecurityAlgorithms.HmacSha256Signature
		);
		tokenDescriptor.EncryptingCredentials = new EncryptingCredentials(
			_builderParams.EncryptionKey,
			JwtConstants.DirectKeyUseAlg,
			SecurityAlgorithms.Aes128CbcHmacSha256
		);

		var handler = new JwtSecurityTokenHandler();
		return handler.WriteToken(handler.CreateToken(tokenDescriptor));
	}

	public string BuildToken(Claim[] claims)
	{
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Expires = DateTime.UtcNow.AddHours(2),
			Subject = new(claims)
		};
		return BuildToken(tokenDescriptor);
	}

	public string BuildToken(IDictionary<string, object> claims)
	{
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Expires = DateTime.UtcNow.AddHours(2),
			Claims = claims
		};
		return BuildToken(tokenDescriptor);
	}

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
			validationParameters.IssuerSigningKey = _builderParams.SigningKey;
			validationParameters.TokenDecryptionKey = _builderParams.EncryptionKey;

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

	public string BuildSpecialToken(
		ClaimsIdentity claims,
		string tokenType,
		DateTime expires,
		bool shouldEncrypt = false
	)
	{
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = claims,
			TokenType = tokenType,
			Expires = expires,
			SigningCredentials = new SigningCredentials(
				_builderParams.SigningKey,
				SecurityAlgorithms.HmacSha256Signature
			),
			EncryptingCredentials = shouldEncrypt
			? new EncryptingCredentials(
				_builderParams.EncryptionKey,
				JwtConstants.DirectKeyUseAlg,
				SecurityAlgorithms.Aes128CbcHmacSha256
			) : null
		};

		var handler = new JwtSecurityTokenHandler();
		return handler.WriteToken(handler.CreateToken(tokenDescriptor));
	}

	public Claim[]? ValidateSpecialToken(
		string token,
		string tokenType,
		out SecurityToken validatedToken
	)
	{
		var handler = new JwtSecurityTokenHandler();

		var validationParameters = new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidTypes = new List<string> { tokenType },
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = _builderParams.SigningKey,
			TokenDecryptionKey = _builderParams.EncryptionKey
		};


		var claims = handler.ValidateToken(
			token,
			validationParameters,
			out validatedToken
		);

		return claims.Claims.ToArray();
	}

}