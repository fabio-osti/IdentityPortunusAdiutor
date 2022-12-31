using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

namespace PortunusAdiutor.Services.TokenBuilder;

public interface ITokenBuilder
{
	string BuildToken(SecurityTokenDescriptor tokenDescriptor);
	string BuildToken(Claim[] claims);
	string BuildToken(IDictionary<string, object> claims);

	Claim[]? ValidateToken(
		string token,
		TokenValidationParameters? validationParameters = null
	);
	string BuildSpecialToken(
		ClaimsIdentity claims,
		string tokenType,
		DateTime expires,
		bool shouldEncrypt = false
	);

	Claim[]? ValidateSpecialToken(
		string token,
		string tokenType,
		out SecurityToken validatedToken
	);

}