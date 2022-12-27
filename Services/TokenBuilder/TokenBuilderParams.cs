using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace PortunusAdiutor.Services.TokenBuilder;

public class TokenBuilderParams
{
	public required SymmetricSecurityKey SigningKey { get; set; }
	public required SymmetricSecurityKey EncryptionKey { get; set; }
	public Action<JwtBearerOptions>? JwtConfigurator { get; set; }
	public TokenValidationParameters? ValidationParams { get; set; }
}