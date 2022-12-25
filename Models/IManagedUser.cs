using System.Diagnostics.CodeAnalysis;

/// <summary>
/// 	An interface for types that implement some sort of password processing.
/// </summary>
public interface IManagedUser
{
	///	<summary>
	///		Sets password for this <see cref="IManagedUser"/>.
	///	</summary>
	///	<param name="password">Clear text password.</param>
	[MemberNotNull(nameof(Salt))]
	void SetPassword(string password);

	///	<summary>
	///		Check if <paramref name="password"/> is valid for this 
	///		<see cref="IManagedUser"/>.
	///	</summary>
	///	<param name="password"></param>
	///	<returns></returns>
	bool ValidatePassword(string password);

	///	<summary>
	///		Gets or sets the salt for this <see cref="IManagedUser"/>
	///	</summary>
	byte[] Salt { get; set; }
}