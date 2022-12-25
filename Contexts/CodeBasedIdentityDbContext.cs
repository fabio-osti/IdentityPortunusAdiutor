using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// 	<see cref="IdentityDbContext{TUser, TRole, TKey}"/> for 
/// 	<see cref="MailCodePoster{TUser, TRole, TKey}"/> apps.
/// </summary>
/// <typeparam name="TUser">Type of <see cref="IdentityUser{TKey}"/> used by the identity system.</typeparam>
/// <typeparam name="TRole">Type of <see cref="IdentityRole{TKey}"/> used by the identity system</typeparam>
/// <typeparam name="TKey">The type used for the primary key for the <typeparamref name="TUser"/>.</typeparam>
public class CodeBasedIdentityDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey>
where TUser : IdentityUser<TKey>
where TRole : IdentityRole<TKey>
where TKey : IEquatable<TKey>
{
	/// <summary>
	/// 	<see cref="DbSet{TEntity}"/> where each entry represents an <see cref="EmailCode{TUser, TKey}"/> sent.
	/// </summary>
	public DbSet<EmailCode<TUser, TKey>> EmailCodes { get; set; }

#pragma warning disable CS8618
	/// <inheritdoc/>
	public CodeBasedIdentityDbContext(DbContextOptions options) : base(options)
	{
	}
#pragma warning restore CS8618

	/// <inheritdoc/>
	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<EmailCode<TUser,TKey>>()
			.HasKey(e => new { e.UserId, e.Code });
		builder.Entity<EmailCode<TUser,TKey>>()
			.HasOne(e => e.User)
			.WithMany();
	}
}