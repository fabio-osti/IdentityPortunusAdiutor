using System.Security.Cryptography;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace PortunusAdiutor.Models;

/// <summary>
/// 	Class representing a single use password for special access.
/// </summary>
/// <typeparam name="TUser">Represents an user in the identity system.</typeparam>
/// <typeparam name="TKey">Represents the key of an user in the identity system.</typeparam>
public class SingleUseToken<TUser, TKey>
where TKey : IEquatable<TKey>
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="xdc"></param>
	/// <param name="type"></param>
	/// <returns></returns>
	public static string GetTokenFrom(TKey userId, string xdc, string type)
	{
		var concat = Encoding.UTF8.GetBytes(userId.ToString() + type + xdc);
		return Base64UrlEncoder.Encode(SHA512.HashData(concat));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="xdc"></param>
	/// <param name="type"></param>
	public SingleUseToken(TKey userId, string xdc, string type)
	{
		Token = GetTokenFrom(userId, xdc, type);
		UserId = userId;
		Type = type;
		ExpiresOn = DateTime.UtcNow.AddMinutes(15);
	}

	private SingleUseToken(TKey userId, string token, string type, DateTime expiresOn)
	{
		UserId = userId;
		Token = token;
		Type = type;
		ExpiresOn = expiresOn;
	}

	/// <summary>
	///  	The user this <see cref="SingleUseToken{TUser, TKey}"/> gives access.
	/// </summary>
	public TUser? User { get; init; }
	/// <summary>
	/// 	The primary key of the user this <see cref="SingleUseToken{TUser, TKey}"/> gives access.
	/// </summary>
	public TKey UserId { get; init; }
	/// <summary>
	/// 	The one use password.
	/// </summary>
	public string Token { get; init; }
	/// <summary>
	///  The type of access given by this <see cref="SingleUseToken{TUser, TKey}"/>.
	/// </summary>
	public string Type { get; init; }
	/// <summary>
	///  Expiration <see cref="DateTime"/>.
	/// </summary>
	public DateTime ExpiresOn { get; init; }
}