namespace PortunusAdiutor.Exceptions;

/// <summary>
/// 	Represents any error that may occur within this lib.
/// </summary>
public class PortunusException : Exception 
{
	/// <summary>
    /// 	Gets or sets a message describing the problem.
    /// </summary>
	public string ShortMessage { get; protected set; }


	/// <summary>
	/// 	Initializes the exception.
	/// </summary>
	public PortunusException(string shortMessage)
	{
		ShortMessage = shortMessage;
	}
}