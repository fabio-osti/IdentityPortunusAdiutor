using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using PortunusAdiutor.Models;

namespace PortunusAdiutor.Data;

/// <summary>
/// 	Base class for the Entity Framework database context used for identity with OTP keeping.
/// </summary>
/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
/// <typeparam name="TRole">Represents a role in the identity system.</typeparam>
/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
/// <typeparam name="TUserClaim">Repesents a claim posessed by an user.</typeparam>
/// <typeparam name="TUserRole">Represents the link between an user and a role.</typeparam>
/// <typeparam name="TUserLogin">Represents a login and its associated provider for an user.</typeparam>
/// <typeparam name="TRoleClaim">Represents a claim that is granted to all users within a role.</typeparam>
/// <typeparam name="TUserToken">Represents an authentication token for an user.</typeparam>
public class OtpIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
where TUser : IdentityUser<TKey>
where TRole : IdentityRole<TKey>
where TKey : IEquatable<TKey>
where TUserClaim : IdentityUserClaim<TKey>
where TUserRole : IdentityUserRole<TKey>
where TUserLogin : IdentityUserLogin<TKey>
where TRoleClaim : IdentityRoleClaim<TKey>
where TUserToken : IdentityUserToken<TKey>
{
#pragma warning disable CS8618
	/// <summary>
	/// 	Initializes a new instance of the class.
	/// </summary>
	/// <param name="options">Options to be used by a <see cref="DbContext"/>.</param>
	public OtpIdentityDbContext(DbContextOptions options) : base(options)
	{
	}
#pragma warning restore CS8618

	/// <summary>
	/// 	Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="UserOneTimePassword{TUser, TKey}"/>
	/// </summary>
	public DbSet<UserOneTimePassword<TUser, TKey>> UserOtps { get; set; }

	/// <inheritdoc/>
	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		var entityTypeBuilder = builder.Entity<UserOneTimePassword<TUser, TKey>>();

		entityTypeBuilder
			.Metadata
			.SetTableName("AspNetUserOtps");

		entityTypeBuilder
			.HasKey(e => new { e.UserId, e.Password, e.Type });

		entityTypeBuilder
			.HasOne<TUser>(e => e.User)
			.WithMany();

	}
}