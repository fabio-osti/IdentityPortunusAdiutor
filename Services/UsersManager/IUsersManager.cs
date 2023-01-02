using System.Linq.Expressions;

using Microsoft.AspNetCore.Identity;

using PortunusAdiutor.Models;

namespace PortunusAdiutor.Services.UsersManager;

public interface IUsersManager<TUser, TKey>
where TUser : IdentityUser<TKey>, IManagedUser<TUser, TKey>
where TKey : IEquatable<TKey>
{
	TUser CreateUser(Expression<Func<TUser, bool>> userFinder, Func<TUser> userBuilder);
	TUser ValidateUser(Expression<Func<TUser, bool>> userFinder, string userPassword);
	TUser SendEmailConfirmation(Expression<Func<TUser, bool>> userFinder);
	TUser ConfirmEmail(string token);
	TUser SendPasswordRedefinition(Expression<Func<TUser, bool>> userFinder);
	TUser RedefinePassword(string token, string newPassword);
}