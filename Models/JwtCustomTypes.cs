/// <summary>
/// 	Class to define JWT header custom "typ".
/// </summary>
public static class JwtCustomTypes
{
	/// <summary>
	/// 	Email Validation Token
	/// </summary>
	public const string EmailConfirmation = "email-confirmation-token";

	/// <summary>
	/// 	Password Redefinition Token
	/// </summary>
	public const string PasswordRedefinition = "password-redefinition-token";
}