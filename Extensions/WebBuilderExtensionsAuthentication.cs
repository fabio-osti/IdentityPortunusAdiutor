using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using PortunusAdiutor.Data;
using PortunusAdiutor.Models;
using PortunusAdiutor.Services.MessagePoster;
using PortunusAdiutor.Services.TokenBuilder;

namespace PortunusAdiutor.Extensions;

/// <summary>
/// 	<see cref="WebApplicationBuilder"/> extensions for injecting the services.
/// </summary>
public static partial class WebBuilderExtensions
{
	/// <summary>
	/// 	Adds all services to the <see cref="ServiceCollection"/> with <see cref="LinkMessagePoster{TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken}"/>.
	/// </summary>
	/// <typeparam name="TContext">Represents an Entity Framework database context used for identity.</typeparam>
	/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
	/// <typeparam name="TRole">Represents a role in the identity system.</typeparam>
	/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
	/// <typeparam name="TUserClaim">Represents a claim possessed by an user.</typeparam>
	/// <typeparam name="TUserRole">Represents the link between an user and a role.</typeparam>
	/// <typeparam name="TUserLogin">Represents a login and its associated provider for an user.</typeparam>
	/// <typeparam name="TRoleClaim">Represents a claim that is granted to all users within a role.</typeparam>
	/// <typeparam name="TUserToken">Represents an authentication token for an user.</typeparam>	
	/// <param name="builder">The web app builder.</param>
	/// <param name="contextConfigurator">The configurator for the <typeparamref name="TContext"/>.</param>
	/// <param name="tokenBuilderParams">The parameters used by the <see cref="TokenBuilder"/>.</param>
	/// <param name="mailLinkPosterParams">The paramaters used by the <see cref="LinkMessagePoster{TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken}"/>.</param>
	/// <returns>An <see cref="AuthenticationBuilder"/> for further configurations.</returns>
	public static AuthenticationBuilder AddAllPortunusServices<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>(
		this WebApplicationBuilder builder,
		Action<DbContextOptionsBuilder> contextConfigurator,
		TokenBuilderParams tokenBuilderParams,
		LinkMessagePosterParams mailLinkPosterParams
	)
	where TContext : ManagedIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
	where TUser : IdentityUser<TKey>, IManagedUser<TUser, TKey>
	where TRole : IdentityRole<TKey>
	where TKey : IEquatable<TKey>
	where TUserClaim : IdentityUserClaim<TKey>
	where TUserRole : IdentityUserRole<TKey>
	where TUserLogin : IdentityUserLogin<TKey>
	where TRoleClaim : IdentityRoleClaim<TKey>
	where TUserToken : IdentityUserToken<TKey>
	{
		builder.Services.AddDbContext<TContext>(contextConfigurator, ServiceLifetime.Singleton);
		var authenticationBuilder = builder.AddTokenBuilder(tokenBuilderParams);
		builder.AddMailLinkPoster<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>(mailLinkPosterParams);
		builder.AddUsersManager<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>();
		return authenticationBuilder;
	}

	/// <summary>
	/// 	Adds all services to the <see cref="ServiceCollection"/> with <see cref="CodeMessagePoster{TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken}"/>.
	/// </summary>
	/// <typeparam name="TContext">Represents an Entity Framework database context used for identity.</typeparam>
	/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
	/// <typeparam name="TRole">Represents a role in the identity system.</typeparam>
	/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
	/// <typeparam name="TUserClaim">Represents a claim possessed by an user.</typeparam>
	/// <typeparam name="TUserRole">Represents the link between an user and a role.</typeparam>
	/// <typeparam name="TUserLogin">Represents a login and its associated provider for an user.</typeparam>
	/// <typeparam name="TRoleClaim">Represents a claim that is granted to all users within a role.</typeparam>
	/// <typeparam name="TUserToken">Represents an authentication token for an user.</typeparam>
	/// <param name="builder">The web app builder.</param>
	/// <param name="contextConfigurator">The configurator for the <typeparamref name="TContext"/>.</param>
	/// <param name="tokenBuilderParams">The parameters used by the <see cref="TokenBuilder"/>.</param>
	/// <param name="mailCodePosterParams">The paramaters used by the <see cref="CodeMessagePoster{TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken}"/>.</param>
	/// <returns>An <see cref="AuthenticationBuilder"/> for further configurations.</returns>
	public static AuthenticationBuilder AddAllPortunusServices<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>(
		this WebApplicationBuilder builder,
		Action<DbContextOptionsBuilder> contextConfigurator,
		TokenBuilderParams tokenBuilderParams,
		CodeMessagePosterParams mailCodePosterParams
	)
	where TContext : ManagedIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
	where TUser : IdentityUser<TKey>, IManagedUser<TUser, TKey>	where TRole : IdentityRole<TKey>
	where TKey : IEquatable<TKey>
	where TUserClaim : IdentityUserClaim<TKey>
	where TUserRole : IdentityUserRole<TKey>
	where TUserLogin : IdentityUserLogin<TKey>
	where TRoleClaim : IdentityRoleClaim<TKey>
	where TUserToken : IdentityUserToken<TKey>
	{
		builder.Services.AddDbContext<TContext>(contextConfigurator, ServiceLifetime.Singleton);
		var authenticationBuilder = builder.AddTokenBuilder(tokenBuilderParams);
		builder.AddMailCodePoster<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>(mailCodePosterParams);
		builder.AddUsersManager<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>();
		return authenticationBuilder;
	}
}
