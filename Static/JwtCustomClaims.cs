namespace PortunusAdiutor.Models;

/// <summary>
/// 	Class to define claims constants.
/// </summary>
static public class JwtCustomClaims
{
	/// <summary>
	/// 	Claim of type "x-digits-code".
	/// </summary>
	public const string XDigitsCode = "x-digits-code";
	/// <summary>
    /// 	Claim of type "email-confirmed".
    /// </summary>
	public const string EmailConfirmed = "email-confirmed";
}