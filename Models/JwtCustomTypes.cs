using PortunusAdiutor.Services.MailPoster;

namespace PortunusAdiutor.Models
{
	/// <summary>
	/// 	Class to define JWT header custom "typ".
	/// </summary>
	public static class JwtCustomTypes
	{
		public const string XDigitsCode = "x-digits-code";

		/// <summary>
		/// 	Email Validation Token
		/// </summary>
		public const string EmailConfirmation = "email-confirmation-token";

		/// <summary>
		/// 	Password Redefinition Token
		/// </summary>
		public const string PasswordRedefinition = "password-redefinition-token";

		public static string ToJwtString(this MessageType messageType) =>
		messageType switch
		{
			MessageType.EmailConfirmation => EmailConfirmation,
			MessageType.PasswordRedefinition => PasswordRedefinition,
			_ => throw new ArgumentOutOfRangeException(nameof(messageType))
		};

	}
}