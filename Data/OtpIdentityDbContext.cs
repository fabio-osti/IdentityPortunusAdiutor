using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using PortunusAdiutor.Models;

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
	public OtpIdentityDbContext(DbContextOptions options) : base(options)
	{
	}
#pragma warning restore CS8618

	public DbSet<UserOneTimePassword<TUser,TKey>> UserOtps { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<UserOneTimePassword<TUser, TKey>>()
			.HasKey(e => new { e.UserId, e.Password, e.Type });

		builder.Entity<UserOneTimePassword<TUser, TKey>>()
			.HasOne<TUser>(e => e.User)
			.WithMany();

	}
}