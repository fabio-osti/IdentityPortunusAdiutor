using Microsoft.AspNetCore.Identity;
namespace PortunusAdiutor.Models;

public class UserOneTimePassword<TUser, TKey>
where TUser : IdentityUser<TKey>
where TKey : IEquatable<TKey>
{
	public TUser? User { get; init; }
	public required TKey UserId { get; init; }
	public required string Password { get; init; }
	public required string Type { get; init; }
	public required DateTime ExpiresOn { get; init; }
	public bool Consumed { get; set; } = false;
}