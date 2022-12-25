using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using PortunusAdiutor.Models;
using PortunusAdiutor.Services.UserManager;

namespace PortunusAdiutor.Extensions;

///	<summary>
///		Helper to configure the services.
///	</summary>
static public partial class WebBuilderExtensions
{
	/// <summary>
	/// 	Configures everything in the right order.
	/// </summary>
	/// <typeparam name="TContext">DbContext type to be used.</typeparam>
	/// <typeparam name="TUser"><see cref="IdentityUser{TKey}"/> type to be used.</typeparam>
	/// <typeparam name="TRole"><see cref="IdentityRole{TKey}"/> type to be used.</typeparam>
	/// <typeparam name="TKey">The type used for the primary key for the <typeparamref name="TUser"/>.</typeparam>
	/// <param name="builder">The app's web builder.</param>
	/// <param name="authenticationConfiguration">
	/// 	The configuration to be used. 
	/// 	<seealso cref="AuthenticationConfigurationParams"/>
	/// </param>
	/// <returns><see cref="AuthenticationBuilder"/> for further configurations.</returns>
	static public AuthenticationBuilder ConfigureAuthentication<TContext, TUser, TRole, TKey>(
		this WebApplicationBuilder builder,
		AuthenticationConfigurationParams authenticationConfiguration
	)
	where TContext : IdentityDbContext<TUser, TRole, TKey>
	where TUser : IdentityUser<TKey>, IManagedUser
	where TRole : IdentityRole<TKey>
	where TKey : IEquatable<TKey>
	{
		builder.Services.AddDbContext<TContext>(authenticationConfiguration.DbContextConfigurator, ServiceLifetime.Singleton);
		AuthenticationBuilder authBuilder = authenticationConfiguration switch
		{
			{ JwtConfigurator: not null } => builder.ConfigureTokenServices(
				authenticationConfiguration.SigningKey,
				authenticationConfiguration.EncryptionKey,
				authenticationConfiguration.JwtConfigurator
			),
			{ ValidationParams: not null } => builder.ConfigureTokenServices(
				authenticationConfiguration.SigningKey,
				authenticationConfiguration.EncryptionKey,
				authenticationConfiguration.ValidationParams
			),
			_ => builder.ConfigureTokenServices(
					authenticationConfiguration.SigningKey,
					authenticationConfiguration.EncryptionKey
				)
		};
		switch (authenticationConfiguration) {
			case { LinkPosterParams: not null }:
				ConfigureMailLinkPoster<TUser, TKey>(
					builder,
					authenticationConfiguration.LinkPosterParams
				);
				break;
			case { CodePosterParams: not null }:
				ConfigureMailCodePoster<TUser, TRole, TKey>(
					builder,
					authenticationConfiguration.CodePosterParams
				);
				break;
			default:
				break;
		}
		builder.Services.AddSingleton<IUserManager<TUser, TRole, TKey>, UserManager<TContext, TUser, TRole, TKey>>();
		return authBuilder;
	}




}