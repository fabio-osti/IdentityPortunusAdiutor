# PortunsAdiutor

## Adding it to your app

	```csharp
	builder.AddAllPortunusServices<ApplicationDbContext, ApplicationUser, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>(
		(e) => e.UseSqlite("Data Source=app.db;"),
		new()
		{
			SigningKey = new(Encoding.UTF8.GetBytes("BeautifulKeyUsedToSignThoseMostSecureTokens")),
			EncryptionKey = new(Encoding.UTF8.GetBytes("BeautifulerKeyUsedToEncryptThoseMostSecureTokens")),
		},
		new MailCodePosterParams()
		{
			SmtpUri = new Uri("smtp://localhost:2525")
		}
	);
	```

## UserManager

 ```csharp
	[ApiController]
	[Route("[controller]/[action]")]
	public class AuthorizationController : ControllerBase
	{
		ILogger _logger;
		IUsersManager<ApplicationUser, Guid> _manager;
		ITokenBuilder _tokenBuilder;

		public AuthorizationController(ILogger logger, IUsersManager<ApplicationUser, Guid> manager, ITokenBuilder tokenBuilder)
		{
			_logger = logger;
			_manager = manager;
			_tokenBuilder = tokenBuilder;
		}

		[HttpPost]
		public IActionResult SignUp([FromBody] CredentialsDto cred)
		{
			try
			{
				var user = _manager.CreateUser(
					e => e.Email == cred.Email,
					() => new ApplicationUser(cred.Email!, cred.Password!)
				);

				return user.Exception is not null
					? Unauthorized()
					: Ok(_tokenBuilder.BuildToken(user.Result.GetClaims()));
			}
			catch (System.Exception e)
			{
				_logger.LogError(e, "An error has occured.");
				return Problem();
			}
		}

		[HttpPost]
		public IActionResult LogIn([FromBody] CredentialsDto cred)
		{
			try
			{
				var user = _manager.ValidateUser(e => e.Email == cred.Email!, cred.Password!);

				return user.Exception is not null
					? Unauthorized()
					: Ok(_tokenBuilder.BuildToken(user.Result.GetClaims()));
			}
			catch (System.Exception e)
			{
				_logger.LogError(e, "An error has occured.");
				return Problem();
			}
		}

		[HttpPost]
		public IActionResult ConfirmMail([FromBody] CredentialsDto cred)
		{
			try
			{
				ArgumentException.ThrowIfNullOrEmpty(cred.Otp);

				var user = _manager.ConfirmEmail(
					cred.Otp,
					e => e.Email == cred.Email
				);

				return user.Exception is not null
					? Unauthorized()
					: Ok();
			}
			catch (System.Exception e)
			{
				_logger.LogError(e, "An error has occured.");
				return Problem();
			}
		}

		[HttpPost]
		public IActionResult RedefinePassword([FromBody] CredentialsDto cred)
		{
			try
			{
				ArgumentException.ThrowIfNullOrEmpty(cred.Password);
				ArgumentException.ThrowIfNullOrEmpty(cred.Otp);

				var user = _manager.RedefinePassword(
					cred.Otp,
					cred.Password,
					e => e.Email == cred.Email
				);

				return user.Exception is not null
					? Unauthorized()
					: Ok();
			}
			catch (System.Exception e)
			{
				_logger.LogError(e, "An error has occured.");
				return Problem();
			}
		}

		[HttpPost]
		public IActionResult SendEmailConfirmation(string email)
		{
			_manager.SendEmailConfirmation(e => e.Email == email);
			return Ok();
		}

		[HttpPost]
		public IActionResult SendPasswordRedefinition(string email)
		{
			_manager.SendPasswordRedefinition(e => e.Email == email);
			return Ok();
		}
	}
 ```