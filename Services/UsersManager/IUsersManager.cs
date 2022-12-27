using System.Linq.Expressions;

using Microsoft.AspNetCore.Identity;

using PortunusAdiutor.Models;

namespace PortunusAdiutor.Services.UsersManager;

public interface IUsersManager<TUser, TKey>
where TUser : IdentityUser<TKey>, IManagedUser
where TKey : IEquatable<TKey>
{
	TUser? CreateUser(Expression<Func<TUser, bool>> userFinder, Func<TUser> userBuilder);
	TUser? ValidateUser(Expression<Func<TUser, bool>> userFinder, string userPassword);
	TUser? SendEmailConfirmation(Expression<Func<TUser, bool>> userFinder);
	TUser? ConfirmEmail(Expression<Func<TUser, bool>> userFinder, string message);
	TUser? SendPasswordRedefinition(Expression<Func<TUser, bool>> userFinder);
	TUser? RedefinePassword(Expression<Func<TUser, bool>> userFinder, string message, string newPassword);
}