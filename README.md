# PortunusAdiutor
A small helper with token authentication.

## ITokenBuilder
Class to build the tokens using a secret key.
Add it on your services like so:

```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.ConfigureTokenServices(
	"B4nTg#8reNm7b23vvT@b68GT#kuw3psX" // Example key
);
```

and then inject it on your controller to use:

```csharp
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

```csharp
string GenToken(Claim[] claims)
{
	return TokenBuilder.BuildToken(new SecurityTokenDescriptor
	{
		Expires = DateTime.UtcNow.AddHours(2),
		Subject = new(claims)
	});
}
```

## EmailingIdentityUser\<TKey>
A class inheriting IdentityUser\<TKey> with methods for sending emails to validate the email and redefine the password using an SMTP server with the following parameters:
 - **URI:** The URI address of the server. Defaults to `smtp://localhost:2525`.
 - **User:** The username for authentication with the server. Defaults to `null`.
 - **Password:** The user password for authentication with the server. Defaults to `null`.
 - **Email Validation Endpoint:** The app's endpoint for email validation. Defaults to `http://localhost:8080/Authorization/ConfirmEmail?token=`.
 - **Password Redefinition Endpoint:** The app's endpoint for email redefinition. Defaults to `http://localhost:8080/Authorization/RedefinePassword?token=`.

The endpoints should have the token as the last parameter so that it may be appendended by this class.

Setting the parameters:

```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.SetSmtpParams(
	smtpUri: "smtp://postoffice:2525",
	smtpUser: "Bob",
	smtpPassword: "aliceIsAcute314",
	EmailConfirmationEndpoint: "https://gatesofapp.com/valmail?token=",
	passwordRedefinitionEndpoint: "https://gatesofapp.com/repswr?token="
);
```

Usage example:

```csharp
user.SendEmailConfirmationMessage(tokenBuilder);
user.SendPasswordRedefinitionMessage(tokenBuilder);
```


This class is inherited by Pbfdk2IdentityUser\<TKey>, prefer to use it instead.

## Pbfdk2IdentityUser\<TKey>
A class inheriting EmailingIdentityUser\<TKey> that automatically processes the password using the Pbfdk2 algorithm with the following parameters:
 -	**Salt:** The user creation UTC DateTime to binary, hashed with SHA256.
 -	**Pseudo Random Function:** `HMACSHA512`.
 -	**Iteration Count:** `262140`.
 -	**Hashed Size:** `128`.

It should be inherited and used like so:

```csharp
public class User : IdentityUserPbkdf2<Guid>
{
	public User(string userName, string password) : base(userName)
	{
		Id = Guid.NewGuid();
		SetPassword(password);
	}

	public bool IsAdmin { get; set; }
}
```

and its usage:

```csharp
user.SetPassword("Pass123$");
user.ValidatePassword("Pass123$"); // returns True;
user.ValidatePassword("Pass132$"); // returns False;
```

