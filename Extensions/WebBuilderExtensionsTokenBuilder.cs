using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using PortunusAdiutor.Services.TokenBuilder;

namespace PortunusAdiutor.Extensions;

static public partial class WebBuilderExtensions
{
	/// <summary>
	/// 	Adds <see cref="TokenBuilder"/> to the <see cref="ServiceCollection"/>.
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="builderParams"></param>
	/// <returns></returns>
	static public AuthenticationBuilder AddTokenBuilder(
		this WebApplicationBuilder builder,
		TokenBuilderParams builderParams
	)
	{
		switch (builderParams)
		{
			case { JwtConfigurator: not null }:
				var hijackedConfigurator = (JwtBearerOptions opt) =>
				{
					builderParams.JwtConfigurator(opt);
					opt.TokenValidationParameters.ValidateIssuerSigningKey = true;
					opt.TokenValidationParameters.IssuerSigningKey = builderParams.SigningKey;
					opt.TokenValidationParameters.TokenDecryptionKey = builderParams.EncryptionKey;
					builderParams.ValidationParams = opt.TokenValidationParameters;
				};

				return builder.Services
					.AddSingleton<ITokenBuilder>(new TokenBuilder(builderParams))
					.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					.AddJwtBearer(hijackedConfigurator);

			case { ValidationParams: not null }:
				builderParams.ValidationParams.ValidateIssuerSigningKey = true;
				builderParams.ValidationParams.IssuerSigningKey = builderParams.SigningKey;
				builderParams.ValidationParams.TokenDecryptionKey = builderParams.EncryptionKey;

				return builder.Services
					.AddSingleton<ITokenBuilder>(_ => new TokenBuilder(builderParams))
					.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					.AddJwtBearer(
						opt =>
						{
							opt.SaveToken = true;
							opt.TokenValidationParameters = builderParams.ValidationParams;
						}
					);
			default:
				return builder.Services
					.AddSingleton<ITokenBuilder>(_ => new TokenBuilder(builderParams))
					.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					.AddJwtBearer(opt =>
					{
						opt.SaveToken = true;
						builderParams.ValidationParams = opt.TokenValidationParameters = new TokenValidationParameters
						{
							ValidateIssuerSigningKey = true,
							IssuerSigningKey = builderParams.SigningKey,
							TokenDecryptionKey = builderParams.SigningKey,
							ValidateAudience = false,
							ValidateIssuer = false,
						};
					});
		}
	}
}