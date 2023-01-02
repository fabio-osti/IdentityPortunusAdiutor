using Microsoft.AspNetCore.Builder;

using Microsoft.Extensions.DependencyInjection;

using PortunusAdiutor.Data;
using PortunusAdiutor.Models;
using PortunusAdiutor.Services.MessagePoster;

namespace PortunusAdiutor.Extensions;

static public partial class WebBuilderExtensions
{
	/// <summary>
	/// 	Adds <see cref="CodeMessagePoster{TContext, TUser, TKey}"/> to the <see cref="ServiceCollection"/>.
	/// </summary>
	/// <typeparam name="TContext">Represents an Entity Framework database context used for identity.</typeparam>
	/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
	/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
	/// <param name="builder">The web app builder.</param>
	/// <param name="mailParams">The paramaters used by the <see cref="CodeMessagePoster{TContext, TUser, TKey}"/>.</param>
	public static void AddMailCodePoster<TContext, TUser, TKey>(
		this WebApplicationBuilder builder,
		CodeMessagePosterParams mailParams
	)
	where TContext : ManagedUserDbContext<TUser, TKey>
	where TUser : class, IManagedUser<TUser, TKey>
	where TKey : IEquatable<TKey>
	{
		builder.Services.AddSingleton<IMessagePoster<TUser, TKey>>(
			e => new CodeMessagePoster<TContext, TUser, TKey>(
				mailParams,
				e.GetRequiredService<TContext>()
			)
		);
	}

	/// <summary>
	/// 	Adds <see cref="LinkMessagePoster{TContext, TUser, TKey}"/> to the <see cref="ServiceCollection"/>.
	/// </summary>
	/// <typeparam name="TContext">Represents an Entity Framework database context used for identity.</typeparam>
	/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
	/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
	/// <param name="builder">The web app builder.</param>
	/// <param name="mailParams">The paramaters used by the <see cref="LinkMessagePoster{TContext, TUser, TKey}"/>.</param>
	public static void AddMailLinkPoster<TContext, TUser, TKey>(
		this WebApplicationBuilder builder,
		LinkMessagePosterParams mailParams
	)
	where TContext : ManagedUserDbContext<TUser, TKey>
	where TUser : class, IManagedUser<TUser, TKey>	
	where TKey : IEquatable<TKey>
	{
		builder.Services.AddSingleton<IMessagePoster<TUser, TKey>>(
			e => new LinkMessagePoster<TContext, TUser, TKey>(
				mailParams,
				e.GetRequiredService<TContext>()
			)
		);
	}
}