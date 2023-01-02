using System.Linq.Expressions;

using Microsoft.AspNetCore.Identity;

using PortunusAdiutor.Models;

namespace PortunusAdiutor.Services.UsersManager;

public interface IUsersManager<TUser, TKey>
where TUser : IdentityUser<TKey>, IManagedUser<TUser, TKey>where TKey : IEquatable<TKey>
{
	Task<TUser> CreateUser(Expression<Func<TUser, bool>> userFinder, Func<TUser> userBuilder);
	Task<TUser> ValidateUser(Expression<Func<TUser, bool>> userFinder, string userPassword);
	Task<TUser> SendEmailConfirmation(Expression<Func<TUser, bool>> userFinder);
	Task<TUser> ConfirmEmail(string token);
	Task<TUser> SendPasswordRedefinition(Expression<Func<TUser, bool>> userFinder);
	Task<TUser> RedefinePassword(string token, string newPassword);
}