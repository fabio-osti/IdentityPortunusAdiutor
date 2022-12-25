using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


public class AuthenticationConfigurationParams
{
	
	public required byte[] SigningKey { get; set; }
		
	public required byte[] EncryptionKey { get; set; }
		
	public MailLinkPosterParams? LinkPosterParams { get; set; }
		
	public MailCodePosterParams? CodePosterParams { get; set; }
		
	public TokenValidationParameters? ValidationParams { get; set; }
		
	public Action<JwtBearerOptions>? JwtConfigurator { get; set; }
	
	public required Action<DbContextOptionsBuilder> DbContextConfigurator { get; set; }
}