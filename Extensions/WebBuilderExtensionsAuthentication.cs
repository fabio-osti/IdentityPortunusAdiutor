using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;

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
static public partial class WebBuilderExtensions
{
	/// <summary>
	/// 	Adds all services to the <see cref="ServiceCollection"/> with <see cref="LinkMessagePoster{TContext, TUser, TKey}"/>.
	/// </summary>
	/// <typeparam name="TContext">Represents an Entity Framework database context used for identity.</typeparam>
	/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
	/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
	/// <param name="builder">The web app builder.</param>
	/// <param name="contextConfigurator">The configurator for the <typeparamref name="TContext"/>.</param>
	/// <param name="tokenBuilderParams">The parameters used by the <see cref="TokenBuilder"/>.</param>
	/// <param name="mailLinkPosterParams">The paramaters used by the <see cref="LinkMessagePoster{TContext, TUser, TKey}"/>.</param>
	/// <returns>An <see cref="AuthenticationBuilder"/> for further configurations.</returns>
	static public AuthenticationBuilder AddAllPortunusServices<TContext, TUser, TKey>(
		this WebApplicationBuilder builder,
		Action<DbContextOptionsBuilder> contextConfigurator,
		TokenBuilderParams tokenBuilderParams,
		LinkMessagePosterParams mailLinkPosterParams
	)
	where TContext : ManagedUserDbContext<TUser, TKey>
	where TUser : class, IManagedUser<TUser, TKey>
	where TKey : IEquatable<TKey>
	{
		builder.Services.AddDbContext<TContext>(contextConfigurator, ServiceLifetime.Singleton);
		var authenticationBuilder = builder.AddTokenBuilder(tokenBuilderParams);
		builder.AddMailLinkPoster<TContext, TUser, TKey>(mailLinkPosterParams);
		builder.AddUsersManager<TContext, TUser, TKey>();
		return authenticationBuilder;
	}

	/// <summary>
	/// 	Adds all services to the <see cref="ServiceCollection"/> with <see cref="CodeMessagePoster{TContext, TUser, TKey}"/>.
	/// </summary>
	/// <typeparam name="TContext">Represents an Entity Framework database context used for identity.</typeparam>
	/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
	/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
	/// <param name="builder">The web app builder.</param>
	/// <param name="contextConfigurator">The configurator for the <typeparamref name="TContext"/>.</param>
	/// <param name="tokenBuilderParams">The parameters used by the <see cref="TokenBuilder"/>.</param>
	/// <param name="mailCodePosterParams">The paramaters used by the <see cref="CodeMessagePoster{TContext, TUser, TKey}"/>.</param>
	/// <returns>An <see cref="AuthenticationBuilder"/> for further configurations.</returns>
	static public AuthenticationBuilder AddAllPortunusServices<TContext, TUser, TKey>(
		this WebApplicationBuilder builder,
		Action<DbContextOptionsBuilder> contextConfigurator,
		TokenBuilderParams tokenBuilderParams,
		CodeMessagePosterParams mailCodePosterParams
	)
	where TContext : ManagedUserDbContext<TUser, TKey>
	where TUser : class, IManagedUser<TUser, TKey>
	where TKey : IEquatable<TKey>

	{
		builder.Services.AddDbContext<TContext>(contextConfigurator, ServiceLifetime.Singleton);
		var authenticationBuilder = builder.AddTokenBuilder(tokenBuilderParams);
		builder.AddMailCodePoster<TContext, TUser, TKey>(mailCodePosterParams);
		builder.AddUsersManager<TContext, TUser, TKey>();
		return authenticationBuilder;
	}
}