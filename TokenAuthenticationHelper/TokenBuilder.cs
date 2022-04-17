using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TokenAuthenticationHelper;

public interface ITokenBuilder
{
	string BuildToken(Claim[] subject, DateTime expires);
}

static public class TokenAuthenticationHelper
{
	private class TokenBuilder : ITokenBuilder
	{
		private readonly byte[] _key;

		public TokenBuilder(byte[] key)
		{
			_key = key;
		}

		public string BuildToken(Claim[] subject, DateTime expires)
		{
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new(subject),
				Expires = expires,
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature
				)
			};

			var handler = new JwtSecurityTokenHandler();
			return handler.WriteToken(handler.CreateToken(tokenDescriptor));
		}
	}

	static public AuthenticationBuilder ConfigureTokenServices(
		this IServiceCollection builder, 
		byte[] key,
		bool isDevelopment = true,
		bool validateIssuer = false,
		bool validateAudience = false
	) { 
		return builder
			.AddSingleton<ITokenBuilder>(new TokenBuilder(key))
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(x =>
			{
				x.RequireHttpsMetadata = !isDevelopment;
				x.SaveToken = true;
				x.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = validateIssuer,
					ValidateAudience = validateAudience
				};
			});
	}

}