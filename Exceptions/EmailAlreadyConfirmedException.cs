namespace PortunusAdiutor.Exceptions;

/// <summary>
/// 	Represents error that occur when an user tries to confirm a confirmed email.
/// </summary>
public class EmailAlreadyConfirmedException : PortunusException
{
	public EmailAlreadyConfirmedException() : base("This user have already confirmed its password.") { }
}