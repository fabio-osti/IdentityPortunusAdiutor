using Microsoft.AspNetCore.Identity;

using PortunusAdiutor.Services.MailPoster;

public class MailCodePost<TUser, TKey>
where TUser : IdentityUser<TKey>
where TKey : IEquatable<TKey>
{
	public TUser? User { get; init; }
	public required TKey UserId { get; init; }
	public required string Code { get; init; }
	public required MessageType Type { get; init; }
	public required DateTime ExpiresOn { get; init; }
	public bool Consumed { get; set; } = false;
}