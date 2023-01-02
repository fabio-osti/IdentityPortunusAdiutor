namespace PortunusAdiutor.Exceptions;

/// <summary>
/// 	Represents any error that may occur within this lib.
/// </summary>
public class PortunusException : Exception 
{
	public string ShortMessage { get; protected set; }

	public PortunusException(string shortMessage)
	{
		ShortMessage = shortMessage;
	}
}