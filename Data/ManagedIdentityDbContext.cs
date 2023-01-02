

using Microsoft.EntityFrameworkCore;
using PortunusAdiutor.Models;

namespace PortunusAdiutor.Data;

/// <summary>
/// 	Base class for the Entity Framework database context used for identity.
/// </summary>
/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
public class ManagedUserDbContext<TUser,TKey> : DbContext
where TUser : class, IManagedUser<TUser, TKey>
where TKey : IEquatable<TKey>
{
#pragma warning disable CS8618
	/// <summary>
	/// 	Initializes a new instance of the class.
	/// </summary>
	/// <param name="options">Options to be used by a <see cref="DbContext"/>.</param>
	public ManagedUserDbContext(DbContextOptions options) : base(options)
	{
	}
#pragma warning restore CS8618

	/// <summary>
	/// 	Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="SingleUseToken{TUser, TKey}"/>
	/// </summary>
	public DbSet<SingleUseToken<TUser, TKey>> UserSingleUseTokens { get; set; }

	/// <summary>
	/// 	Gets or sets the <see cref="DbSet{TEntity}"/> of the users.
    /// </summary>
	public DbSet<TUser> Users { get; set; }

	/// <inheritdoc/>
	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		var usrBuilder = builder.Entity<TUser>();

		usrBuilder.
			HasKey(e => e.Id);

		var sutBuilder = builder.Entity<SingleUseToken<TUser, TKey>>();

		sutBuilder
			.HasKey(e => e.Token);

		sutBuilder
			.HasOne<TUser>(e => e.User)
			.WithMany(e => e.SingleUseTokens);

	}
}