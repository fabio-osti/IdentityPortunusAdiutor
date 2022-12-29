namespace PortunusAdiutor.Models;

public static class JwtCustomTypes
{
	public const string EmailConfirmation = "email-confirmation-token";
	public const string PasswordRedefinition = "password-redefinition-token";

	public static string ToJwtString(this MessageType messageType) =>
	messageType switch
	{
		MessageType.EmailConfirmation => EmailConfirmation,
		MessageType.PasswordRedefinition => PasswordRedefinition,
		_ => throw new ArgumentOutOfRangeException(nameof(messageType))
	};
}

public enum MessageType
{
	EmailConfirmation,
	PasswordRedefinition
}