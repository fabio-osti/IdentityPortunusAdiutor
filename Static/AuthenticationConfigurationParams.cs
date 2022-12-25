using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// 	Class representing the parameters used for configuring all services at once.
/// </summary>
public class AuthenticationConfigurationParams
{
	/// <summary>
	/// 	Secret string used for the token signing.
	/// </summary>
	public required byte[] SigningKey { get; set; }
	/// <summary>
	/// 	Secret string used for the token encryption.
	/// </summary>
	public required byte[] EncryptionKey { get; set; }
	/// <summary>
	/// 	Parameters for configuring <see cref="MailLinkPoster{TUser, TKey}"/>.
	/// </summary>
	/// <remarks>
	/// 	If this is set, <see cref="AuthenticationConfigurationParams.CodePosterParams"/>
	/// 	is ignored
	/// </remarks>
	public MailLinkPosterParams? LinkPosterParams { get; set; }
	/// <summary>
	/// 	Parameters for configuring <see cref="MailCodePoster{TUser, TRole, TKey}"/>.
	/// </summary>
	/// <remarks>
	/// 	If <see cref="AuthenticationConfigurationParams.LinkPosterParams"/> is set,
	/// 	this is ignored
	/// </remarks>

	public MailCodePosterParams? CodePosterParams { get; set; }
	/// <summary>
	/// 	Configurator to be passed to the
	/// 	<see cref="JwtBearerExtensions.AddJwtBearer(AuthenticationBuilder, Action{JwtBearerOptions})"/>
	/// 	injector.
	/// </summary>
	/// <remarks>
	/// 	If this is set, <see cref="AuthenticationConfigurationParams.ValidationParams"/>
	/// 	is ignored
	/// </remarks>
	public Action<JwtBearerOptions>? JwtConfigurator { get; set; }
	/// <summary>
	/// 	Parameters for configuring 
	/// 	<see cref="JwtBearerExtensions.AddJwtBearer(AuthenticationBuilder, Action{JwtBearerOptions})"/>
	/// </summary>	
	///	<remarks>
	/// 	If <see cref="AuthenticationConfigurationParams.JwtConfigurator"/> is set,
	/// 	this is ignored
	/// </remarks>
	public TokenValidationParameters? ValidationParams { get; set; }
	/// <summary>
	/// 	DbContext configurator to be passed to the 
	/// 	<see cref="EntityFrameworkServiceCollectionExtensions.AddDbContext{TContext}(IServiceCollection, Action{DbContextOptionsBuilder}?, ServiceLifetime, ServiceLifetime)"/>
	/// 	injector.
	/// </summary>
	public required Action<DbContextOptionsBuilder> DbContextConfigurator { get; set; }
}