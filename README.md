# PortunusAdiutor

A small helper with token authentication.
Inject it on your services like so:

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