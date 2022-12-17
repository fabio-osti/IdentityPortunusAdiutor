using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PortunusAdiutor;
///	<summary>
///		Class to build a token accordingly.
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
}

///	<summary>
///		Default ITokenBuilder implementation.
///	</summary>
///	<remarks>
///		Prefer to use any <see cref="WebBuilderExtensions"/>
///		to inject the <see cref="ITokenBuilder"/> dependency.
///	</remarks>
public class TokenBuilder : ITokenBuilder
{
	private readonly byte[] _key;
	///	<summary>
	///		Initiazlize a new instance of <see cref="TokenBuilder"/>.
	///	</summary>
	///	<param name="key">Secret string used for the token encryption.</param>
	public TokenBuilder(byte[] key)
	{
		_key = key;
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
			Subject = new(claims),
			SigningCredentials = new SigningCredentials(
				new SymmetricSecurityKey(_key),
				SecurityAlgorithms.HmacSha256Signature
			),
		};
		var handler = new JwtSecurityTokenHandler();
		return handler.WriteToken(handler.CreateToken(tokenDescriptor));
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
			new SymmetricSecurityKey(_key),
			SecurityAlgorithms.HmacSha256Signature
		);

		var handler = new JwtSecurityTokenHandler();
		return handler.WriteToken(handler.CreateToken(tokenDescriptor));
	}
}
