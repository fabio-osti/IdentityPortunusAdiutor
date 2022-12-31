using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using PortunusAdiutor.Data;
using PortunusAdiutor.Models;
using PortunusAdiutor.Services.MessagePoster;
using PortunusAdiutor.Services.TokenBuilder;

namespace PortunusAdiutor.Extensions;

static public partial class WebBuilderExtensions
{
	/// <summary>
	/// 	Adds <see cref="MessageCodePoster{TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken}"/> to the <see cref="ServiceCollection"/>.
	/// </summary>
	/// <typeparam name="TContext">Represents an Entity Framework database context used for identity with OTP keeping.</typeparam>
	/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
	/// <typeparam name="TRole">Represents a role in the identity system.</typeparam>
	/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
	/// <typeparam name="TUserClaim">Repesents a claim posessed by an user.</typeparam>
	/// <typeparam name="TUserRole">Represents the link between an user and a role.</typeparam>
	/// <typeparam name="TUserLogin">Represents a login and its associated provider for an user.</typeparam>
	/// <typeparam name="TRoleClaim">Represents a claim that is granted to all users within a role.</typeparam>
	/// <typeparam name="TUserToken">Represents an authentication token for an user.</typeparam>
	/// <param name="builder">The web app builder.</param>
	/// <param name="mailParams">The paramaters used by the <see cref="MessageCodePoster{TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken}"/>.</param>
	public static void AddMailCodePoster<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>(
		this WebApplicationBuilder builder,
		MessageCodePosterParams mailParams
	)
	where TContext : OtpIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
	where TUser : IdentityUser<TKey>, IManagedUser
	where TRole : IdentityRole<TKey>
	where TKey : IEquatable<TKey>
	where TUserClaim : IdentityUserClaim<TKey>
	where TUserRole : IdentityUserRole<TKey>
	where TUserLogin : IdentityUserLogin<TKey>
	where TRoleClaim : IdentityRoleClaim<TKey>
	where TUserToken : IdentityUserToken<TKey>
	{
		builder.Services.AddSingleton<IMessagePoster<TUser, TKey>>(
			e => new MessageCodePoster<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>(
				mailParams,
				e.GetRequiredService<TContext>(),
				e.GetRequiredService<ITokenBuilder>()
			)
		);
	}

	/// <summary>
	/// 	Adds <see cref="MessageLinkPoster{TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken}"/> to the <see cref="ServiceCollection"/>.
	/// </summary>
	/// <typeparam name="TContext"></typeparam>
	/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
	/// <typeparam name="TRole">Represents a role in the identity system.</typeparam>
	/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
	/// <typeparam name="TUserClaim">Repesents a claim posessed by an user.</typeparam>
	/// <typeparam name="TUserRole">Represents the link between an user and a role.</typeparam>
	/// <typeparam name="TUserLogin">Represents a login and its associated provider for an user.</typeparam>
	/// <typeparam name="TRoleClaim">Represents a claim that is granted to all users within a role.</typeparam>
	/// <typeparam name="TUserToken">Represents an authentication token for an user.</typeparam>
	/// <param name="builder">The web app builder.</param>
	/// <param name="mailParams">The paramaters used by the <see cref="MessageLinkPoster{TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken}"/>.</param>
	public static void AddMailLinkPoster<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>(
		this WebApplicationBuilder builder,
		MessageLinkPosterParams mailParams
	)
	where TContext : OtpIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
	where TUser : IdentityUser<TKey>, IManagedUser
	where TRole : IdentityRole<TKey>
	where TKey : IEquatable<TKey>
	where TUserClaim : IdentityUserClaim<TKey>
	where TUserRole : IdentityUserRole<TKey>
	where TUserLogin : IdentityUserLogin<TKey>
	where TRoleClaim : IdentityRoleClaim<TKey>
	where TUserToken : IdentityUserToken<TKey>
	{
		builder.Services.AddSingleton<IMessagePoster<TUser, TKey>>(
			e => new MessageLinkPoster<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>(
				mailParams,
				e.GetRequiredService<TContext>(),
				e.GetRequiredService<ITokenBuilder>()
			)
		);
	}
}