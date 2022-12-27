using System.Diagnostics.CodeAnalysis;

namespace PortunusAdiutor.Models;

public interface IManagedUser
{	
	[MemberNotNull(nameof(Salt))]
	void SetPassword(string password);
	bool ValidatePassword(string password);
	byte[] Salt { get; set; }
}