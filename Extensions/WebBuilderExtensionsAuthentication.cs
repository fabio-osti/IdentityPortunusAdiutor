using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using PortunusAdiutor.Models;
using PortunusAdiutor.Services.TokenBuilder;
using PortunusAdiutor.Services.UsersManager;

namespace PortunusAdiutor.Extensions;

static public partial class WebBuilderExtensions
{
	static public AuthenticationBuilder AddAllPortunusServices<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>(
		this WebApplicationBuilder builder,
		Action<DbContextOptionsBuilder> contextConfigurator,
		TokenBuilderParams tokenBuilderParams,
		MailLinkPosterParams mailLinkPosterParams
	)
	where TContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
	where TUser : IdentityUser<TKey>, IManagedUser
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

	static public AuthenticationBuilder AddAllPortunusServices<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>(
		this WebApplicationBuilder builder,
		Action<DbContextOptionsBuilder> contextConfigurator,
		TokenBuilderParams tokenBuilderParams,
		MailCodePosterParams mailCodePosterParams
	)
	where TContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
	where TUser : IdentityUser<TKey>, IManagedUser
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

		builder.AddMailCodePoster<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>(mailCodePosterParams);

		builder.AddUsersManager<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>();

		return authenticationBuilder;
	}
}