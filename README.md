# PortunusAdiutor
A small helper with token authentication.

## ITokenBuilder
Class to build the tokens using a secret key.
Add it on your services like so:
```
builder.ConfigureTokenServices(
	"B4nTg#8reNm7b23vvT@b68GT#kuw3psX" // Example key
);
```
and then inject it on your controller to use:
```
class AuthorizationController : ControllerBase
{
	ITokenBuilder TokenBuilder { get; }

	AuthorizationController(ITokenBuilder tokenBuilder)
	{
		TokenBuilder = tokenBuilder;
	}
}
```
and to use it:
```
string GenToken(Claim[] claims)
{
	return TokenBuilder.BuildToken(new SecurityTokenDescriptor
	{
		Expires = DateTime.UtcNow.AddHours(2),
		Subject = new(claims)
	});
}
```
## IdentityUserPbfdk2\<TKey>
A class inheriting IdentityUser\<TKey> that automatically processes the password using the Pbfdk2 algorithim with the following parameters:
 -  **Salt:** The user creation UTC DateTime to binary, hashed with SHA256.
 -  **Pseudo Random Function:** HMACSHA256.
 -  **Iteration Count:** "PBKDF2_ITER_COUNT" key's value on appsetings.json. Defaults to 262140.
 -  **Hashed Size:** "PBKDF2_HASHED_SIZE" key's value on appsettings.json. Defaults to 128.
It should be inherited and used like so:
```
public class  User : IdentityUserPbkdf2<Guid>
{
	public  User(string userName, string password) : base(userName)
	{
		Id = Guid.NewGuid();
		SetPassword(password);
	}

	public  bool IsAdmin { get; set; }
}
```
and it's usage:
```
user.SetPassword("Pass123$");
user.ValidatePassword("Pass123$"); // returns True;
user.ValidatePassword("Pass132$"); // returns False;
```