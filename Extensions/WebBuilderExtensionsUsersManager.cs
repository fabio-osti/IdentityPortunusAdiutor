using Microsoft.AspNetCore.Builder;

using Microsoft.Extensions.DependencyInjection;

using PortunusAdiutor.Data;
using PortunusAdiutor.Models;
using PortunusAdiutor.Services.UsersManager;

namespace PortunusAdiutor.Extensions;

static public partial class WebBuilderExtensions
{
	/// <summary>
	/// 	Adds <see cref="UsersManager{TContext, TUser, TKey}"/> to the <see cref="ServiceCollection"/>.
	/// </summary>
	/// <typeparam name="TContext">Represents an Entity Framework database context used for identity.</typeparam>
	/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
	/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
	/// <param name="builder">The web app builder.</param>
	public static void AddUsersManager<TContext, TUser, TKey>(this WebApplicationBuilder builder)
	where TContext : ManagedUserDbContext<TUser, TKey>
	where TUser : class, IManagedUser<TUser, TKey>
	where TKey : IEquatable<TKey>
	{
		builder.Services.AddSingleton<IUsersManager<TUser, TKey>, UsersManager<TContext, TUser, TKey>>();
	}
}