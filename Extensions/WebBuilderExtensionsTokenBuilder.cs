using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using PortunusAdiutor.Data;
using PortunusAdiutor.Services.TokenBuilder;

namespace PortunusAdiutor.Extensions;

static public partial class WebBuilderExtensions
{
	/// <summary>
	/// 	Adds <see cref="TokenBuilder"/> to the <see cref="ServiceCollection"/>.
	/// </summary>
	/// <param name="builder">The web app builder.</param>
	/// <param name="tokenBuilderParams">The parameters used by the <see cref="TokenBuilder"/>.</param>
	/// <returns></returns>
	static public AuthenticationBuilder AddTokenBuilder(
		this WebApplicationBuilder builder,
		TokenBuilderParams tokenBuilderParams
	)
	{
		switch (tokenBuilderParams) {
			case { JwtConfigurator: not null }:
				var hijackedConfigurator = (JwtBearerOptions opt) =>
				{
					tokenBuilderParams.JwtConfigurator(opt);
					opt.TokenValidationParameters.ValidateIssuerSigningKey = true;
					opt.TokenValidationParameters.IssuerSigningKey = tokenBuilderParams.SigningKey;
					opt.TokenValidationParameters.TokenDecryptionKey = tokenBuilderParams.EncryptionKey;
					tokenBuilderParams.ValidationParams = opt.TokenValidationParameters;
				};

				return builder.Services
					.AddSingleton<ITokenBuilder>(new TokenBuilder(tokenBuilderParams))
					.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					.AddJwtBearer(hijackedConfigurator);

			case { ValidationParams: not null }:
				tokenBuilderParams.ValidationParams.ValidateIssuerSigningKey = true;
				tokenBuilderParams.ValidationParams.IssuerSigningKey = tokenBuilderParams.SigningKey;
				tokenBuilderParams.ValidationParams.TokenDecryptionKey = tokenBuilderParams.EncryptionKey;

				return builder.Services
					.AddSingleton<ITokenBuilder>(_ => new TokenBuilder(tokenBuilderParams))
					.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					.AddJwtBearer(
						opt =>
						{
							opt.SaveToken = true;
							opt.TokenValidationParameters = tokenBuilderParams.ValidationParams;
						}
					);
			default:
				return builder.Services
					.AddSingleton<ITokenBuilder>(_ => new TokenBuilder(tokenBuilderParams))
					.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					.AddJwtBearer(opt =>
					{
						opt.SaveToken = true;
						tokenBuilderParams.ValidationParams = opt.TokenValidationParameters = new TokenValidationParameters
						{
							ValidateIssuerSigningKey = true,
							IssuerSigningKey = tokenBuilderParams.SigningKey,
							TokenDecryptionKey = tokenBuilderParams.SigningKey,
							ValidateAudience = false,
							ValidateIssuer = false,
						};
					});
		}
	}
}