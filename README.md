# PortunusAdiutor
A small helper with token authentication.

Examples are worth a thousand comments, so this is an example of the configuration to be called at the WebApplicationBuilder:

```csharp
var authParams = new AuthenticationConfigurationParams()
{
	SigningKey = Encoding.UTF8.GetBytes(builder.Configuration["SigningKey"]!),
	EncryptionKey = Encoding.UTF8.GetBytes(builder.Configuration["EncryptKey"]!),
	DbContextConfigurator = 
		options => options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")),
	LinkPosterParams = builder.GetMailLinkPosterParams(
		smtpUri:"smtp://localhost:2525",
		emailConfirmationEndpoint:"http://localhost/authorization/confirmemail?token=",
		passwordRedefinitionEndpoint:"http://localhost/authorization/redefinepassword?token="
	)
};
builder.ConfigureAuthentication<ApplicationIdentityDbContext, ApplicationUser, IdentityRole<Guid>, Guid>(authParams);
```

And a controller that uses the services:

```csharp
[ApiController]
[Route("[controller]/[action]")]
public class AuthorizationController : ControllerBase
{
	readonly ITokenBuilder _tokenBuilder;
	readonly IUserManager<ApplicationUser, IdentityRole<Guid>, Guid> _userManager;

	public AuthorizationController(
		ITokenBuilder tokenBuilder,
		IUserManager<ApplicationUser, IdentityRole<Guid>, Guid> userManager
	)
	{
		_tokenBuilder = tokenBuilder;
		_userManager = userManager;
	}

	[HttpPost]
	public IActionResult SignUp([FromBody] CredentialsDto credentials)
	{
		try {
			var user = _userManager.CreateUser(
				e => e.Email == credentials.Email,
				() => new ApplicationUser(credentials.Email!, credentials.Password!)
			);

			return user is null 
				? Problem("Email already registered.") 
				: Ok(_tokenBuilder.BuildToken(user.GetClaims()));
		} catch (Exception e) {
			return Problem(e.Message);
		}
	}

	[HttpPost]
	public IActionResult SignIn([FromBody] CredentialsDto credentials)
	{
		try {
			var user = _userManager.ValidateUser(
				(e) => e.Email == credentials.Email, 
				credentials.Password!
			);

			return user is null 
				? Problem("User or Password not found.")
				: Ok(_tokenBuilder.BuildToken(user.GetClaims()));
		} catch (Exception e) {
			return Problem(e.Message);
		}
	}

	[HttpGet]
	public IActionResult ConfirmEmail(string token)
	{
		try {
			var userPk =
				_tokenBuilder.ValidateCustomTypeToken(token, JwtCustomTypes.EmailConfirmation);
			var user = _userManager.ConfirmEmail(u => u.Id.ToString() == userPk);
					return user is null ? NotFound() : Ok();
		} catch (Exception e) {
			return Problem(e.Message);
		}
	}

	[HttpPost]
	public IActionResult SendPasswordRedefinition([FromBody] CredentialsDto redefine)
	{
		try {
			var user = 
				_userManager.SendPasswordRedefinition(e => e.Email == redefine.Email);

			return user is null ? NotFound() : Ok();
		} catch (Exception e) {
			return Problem(e.Message);
		}
	}

	[HttpGet]
	public IActionResult RedefinePassword(string token)
	{
		try {
			var userPk =
				_tokenBuilder.ValidateCustomTypeToken(token, JwtCustomTypes.PasswordRedefinition);
			if (userPk is null) {
				return NotFound();
			}

			var html = System.IO.File.ReadAllText("Content/redefine-password.html");
			return Content(html, "text/html");
		} catch (Exception e) {
			return Problem(e.Message);
		}
	}

	[HttpPost]
	public IActionResult RedefinePassword([FromBody] CredentialsDto redefined, string token)
	{
		try {
			var userPk =
				_tokenBuilder.ValidateCustomTypeToken(token, JwtCustomTypes.PasswordRedefinition);
			var user = _userManager.RedefinePassword(
				u => u.Id.ToString() == userPk, 
				redefined.Password!
			);
					return user is null ? NotFound() : Ok();
		} catch (Exception e) {

			return Problem(e.Message);
		}
	}
}
```
For a more in depth display of the components, keep reading.

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

## Pbfdk2IdentityUser\<TKey>
A class inheriting IdentityUser\<TKey> that automatically processes the password using the Pbfdk2 algorithm with the following parameters:
 -	**Salt:** The user creation UTC DateTime to binary, hashed with SHA256.
 -	**Pseudo Random Function:** `HMACSHA512`.
 -	**Iteration Count:** `262140`.
 -	**Hashed Size:** `128`.

It should be inherited and used like so:

```csharp
public class ApplicationUser : IdentityUserPbkdf2<Guid>
{
	public ApplicationUser(string email, string password) : base(email, password)
	{
		Id = Guid.NewGuid();
	}
}
```

and its usage:

```csharp
user.SetPassword("Pass123$");
user.ValidatePassword("Pass123$"); // returns True;
user.ValidatePassword("Pass132$"); // returns False;
```

