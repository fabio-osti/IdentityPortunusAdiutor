namespace PortunusAdiutor.Exceptions;

/// <summary>
/// 	Represents error that occur when an user password is not valid.
/// </summary>
public class InvalidPasswordException : PortunusException
{
	/// <summary>
	/// 	Initializes the exception.
	/// </summary>
	public InvalidPasswordException() : base("The validation of the password for this user failed.") { }
}