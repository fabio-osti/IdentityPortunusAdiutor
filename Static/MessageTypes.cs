namespace PortunusAdiutor.Static;

/// <summary>
/// 	Class to define constants representing message types.
/// </summary>
public static class MessageTypes
{
	/// <summary>
	/// 	Message of type "email-confirmation-token".
	/// </summary>
	public const string EmailConfirmation = "email-confirmation-token";
	/// <summary>
	///  Message of type "password-redefinition".
	/// </summary>
	public const string PasswordRedefinition = "password-redefinition-token";

	/// <summary>
	///  Gets a string associated with <paramref name="messageType"/>.
	/// </summary>
	/// <param name="messageType">Type of message represented by the returned string.</param>
	/// <returns>A string describing the <paramref name="messageType"/>.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Undefined <see cref="MessageType"/>.</exception>
	public static string ToJwtTypeString(this MessageType messageType) =>
	messageType switch
	{
		MessageType.EmailConfirmation => EmailConfirmation,
		MessageType.PasswordRedefinition => PasswordRedefinition,
		_ => throw new ArgumentOutOfRangeException(nameof(messageType))
	};
}

/// <summary>
///		Enumeration of types of message.
/// </summary>
public enum MessageType
{
	/// <summary>
	/// 	Message for email confirmation.
	/// </summary>
	EmailConfirmation,
	/// <summary>
	/// 	Message for password redefinition.
	/// </summary>
	PasswordRedefinition
}
