using System.Linq.Expressions;

using Microsoft.AspNetCore.Identity;

using PortunusAdiutor.Models;

namespace PortunusAdiutor.Services.UsersManager;

public interface IUsersManager<TUser, TKey>
where TUser : IdentityUser<TKey>, IManagedUser
where TKey : IEquatable<TKey>
{
	Task<TUser> CreateUser(Expression<Func<TUser, bool>> userFinder, Func<TUser> userBuilder);
	Task<TUser> ValidateUser(Expression<Func<TUser, bool>> userFinder, string userPassword);
	Task<TUser> SendEmailConfirmation(Expression<Func<TUser, bool>> userFinder);
	Task<TUser> ConfirmEmail(string otp, Expression<Func<TUser, bool>>? userFinder);
	Task<TUser> SendPasswordRedefinition(Expression<Func<TUser, bool>> userFinder);
	Task<TUser> RedefinePassword(string otp, string newPassword, Expression<Func<TUser, bool>>? userFinder);
}