namespace Lyt.Reflector.IL;
/*
/// <summary> A list of instructions. </summary>
public interface IInstructionList : IReadOnlyList<IInstruction>
{
	/// <summary> Gets the byte data for these instructions. </summary>
	IReadOnlyList<byte> Data { get; }

	/// <summary>
	/// Gets a value indicating if any problems occurred decoding the byte
	/// data (<see cref="Data"/>) for these instructions.
	/// </summary>
	bool IsInvalidData { get; }

	/// <summary>
	/// Gets a value indicating if the specified instruction is contained
	/// within a method that has "this" as its first argument.
	/// </summary>
	/// <param name="instruction">The instruction to test.</param>
	/// <returns>True, if the containing method has a "this" argument; otherwise, false.</returns>
	bool HasThis(IInstruction instruction);

	/// <summary>
	/// Gets a value indicating if the specified assembly is the same
	/// as the one containing this instruction list.
	/// </summary>
	/// <param name="assembly">The assembly to test.</param>
	/// <returns>True, if the assembly is the same; otherwise, false.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="assembly"/> is null.
	/// </exception>
	bool IsSameAssembly(Assembly assembly);

	/// <summary>
	/// Resolve field information from a metadata token.
	/// </summary>
	/// <param name="token">The metadata token.</param>
	/// <returns>The field information.</returns>
	/// <exception cref="System.ArgumentException">
	/// <paramref name="token"/> is not a field within the scope of this method's module.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="token"/> is not a valid field within this method's module.
	/// </exception>
	FieldInfo ResolveField(Token token);

	/// <summary>
	/// Resolve an instruction for a byte offset.
	/// </summary>
	/// <param name="offset">The byte offset of an instruction within this method body.</param>
	/// <returns>The instruction.</returns>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> does not specify the valid byte offset of an
	/// instruction within this method body.
	/// </exception>
	IInstruction ResolveInstruction(int offset);

	/// <summary>
	/// Resolve member information from a metadata token.
	/// </summary>
	/// <param name="token">The metadata token.</param>
	/// <returns>The member information.</returns>
	/// <exception cref="System.ArgumentException">
	/// <paramref name="token"/> is not a member within the scope of this method's module.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="token"/> is not a valid member within this method's module.
	/// </exception>
	MemberInfo ResolveMember(Token token);

	/// <summary>
	/// Resolve method information from a metadata token.
	/// </summary>
	/// <param name="token">The metadata token.</param>
	/// <returns>The method information.</returns>
	/// <exception cref="System.ArgumentException">
	/// <paramref name="token"/> is not a method within the scope of this method's module.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="token"/> is not a valid method within this method's module.
	/// </exception>
	MethodBase ResolveMethod(Token token);

	/// <summary>
	/// Resolve parameter information from the specified index.
	/// </summary>
	/// <param name="operand">The zero-based index of the parameter.</param>
	/// <returns>The parameter information.</returns>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="operand"/> does not identify a valid parameter for this method.
	/// </exception>
	ParameterInfo ResolveParameter(int operand);

	/// <summary>
	/// Resolve signature information from a metadata token.
	/// </summary>
	/// <param name="token">The metadata token.</param>
	/// <returns>The signature information.</returns>
	/// <exception cref="System.ArgumentException">
	/// <paramref name="token"/> is not a signature within the scope of this method's module.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="token"/> is not a valid signature within this method's module.
	/// </exception>
	byte[] ResolveSignature(Token token);

	/// <summary>
	/// Resolve string information from a metadata token.
	/// </summary>
	/// <param name="token">The metadata token.</param>
	/// <returns>The string information.</returns>
	/// <exception cref="System.ArgumentException">
	/// <paramref name="token"/> is not a string within the scope of this method's module.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="token"/> is not a valid string within this method's module.
	/// </exception>
	string ResolveString(Token token);

	/// <summary>
	/// Resolve type information from a metadata token.
	/// </summary>
	/// <param name="token">The metadata token.</param>
	/// <returns>The type information.</returns>
	/// <exception cref="System.ArgumentException">
	/// <paramref name="token"/> is not a type within the scope of this method's module.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="token"/> is not a valid type within this method's module.
	/// </exception>
	Type ResolveType(Token token);

	/// <summary>
	/// Resolve local variable information from the specified index.
	/// </summary>
	/// <param name="operand">The zero-based index of the variable.</param>
	/// <returns>The local variable information.</returns>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="operand"/> does not identify a valid local variable for this method.
	/// </exception>
	LocalVariableInfo ResolveVariable(int operand);
}
*/