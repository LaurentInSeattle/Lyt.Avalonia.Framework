namespace Lyt.Reflector.IL;

/// <summary> The instructions for a method body. </summary>
public class MethodIL : InstructionList
{
	/// <summary> Create an instance for the specified method. </summary>
	/// <param name="method">The method containing the instructions.</param>
	public MethodIL(MethodBase method)
		: base((method )?
			  .GetMethodBody()?.GetILAsByteArray() ?? new byte[0])
	{
		Method = method ?? throw new ArgumentNullException(nameof(method));
		MethodBody = method.GetMethodBody();
		Module = method.Module;
		DecodeInstructions();
	}

	/// <summary>Gets the method containing these instructions.</summary>
	public MethodBase Method { get; }

	/// <summary> Gets the body of the method containing these instructions. </summary>
	public MethodBody MethodBody { get; }

	/// <summary> Gets the module containing these instructions. </summary>
	public Module Module { get; }

	/// <summary>
	/// Gets a value indicating if the specified instruction is contained
	/// within a method that has "this" as its first argument.
	/// </summary>
	/// <param name="instruction">The instruction to test.</param>
	/// <returns>True, if the containing method has a "this" argument; otherwise, false.</returns>
	public override bool HasThis(IInstruction instruction) =>
		(Method.CallingConvention & CallingConventions.HasThis) != 0;

	/// <summary>
	/// Gets a value indicating if the specified assembly is the same
	/// as the one containing this instruction list.
	/// </summary>
	/// <param name="assembly">The assembly to test.</param>
	/// <returns>True, if the assembly is the same; otherwise, false.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="assembly"/> is null.
	/// </exception>
	public override bool IsSameAssembly(Assembly assembly)
	{
		if (assembly == null)
			throw new ArgumentNullException(nameof(assembly));

		return Module.Assembly == assembly;
	}

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
	public override FieldInfo ResolveField(Token token) =>
		Module.ResolveField(token.Value, Method.DeclaringType.GetGenericArguments(),
			Method.GetGenericArguments());

	/// <summary>
	/// Resolve an instruction for a byte offset.
	/// </summary>
	/// <param name="offset">The byte offset of an instruction within this method body.</param>
	/// <returns>The instruction.</returns>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> does not specify the valid byte offset of an
	/// instruction within this method body.
	/// </exception>
	public override IInstruction ResolveInstruction(int offset)
	{
		int high = Count - 1;
		int low = 0;

		while(low <= high)
		{
			int mid = (low + high) >> 1;
			var candidate = (Instruction)this[mid];
			int candidateOffset = candidate.Offset;

			if (offset == candidateOffset)
			{
				candidate.IsTarget = true;
				return this[mid];
			}

			if (low == high)
            {
                break;
            }

            if (offset < candidateOffset)
            {
                high = mid - 1;
            }
            else if (offset > candidateOffset)
            {
                low = mid + 1;
            }
        }

		throw new ArgumentOutOfRangeException(nameof(offset));
	}

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
	public override MemberInfo ResolveMember(Token token) =>
		Module.ResolveMember(token.Value, Method.DeclaringType.GetGenericArguments(),
			Method.GetGenericArguments());

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
	public override MethodBase ResolveMethod(Token token) =>
		Module.ResolveMethod(token.Value, Method.DeclaringType.GetGenericArguments(),
			Method.GetGenericArguments());

	/// <summary>
	/// Resolve parameter information from the specified index.
	/// </summary>
	/// <param name="operand">The operand for the parameter.</param>
	/// <returns>The parameter information.</returns>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="operand"/> does not identify a valid parameter for this method.
	/// </exception>
	public override ParameterInfo ResolveParameter(int operand)
	{
		if ((Method.CallingConvention & CallingConventions.HasThis) != 0 && operand-- == 0)
        {
            return null; // The "this" argument
        }

        ParameterInfo[] parameters = Method.GetParameters();
		if (operand < 0 || operand > parameters.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(operand));
        }

        return parameters[operand];
	}

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
	public override byte[] ResolveSignature(Token token) =>
		Module.ResolveSignature(token.Value);

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
	public override string ResolveString(Token token) =>
		Module.ResolveString(token.Value);

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
	public override Type ResolveType(Token token) =>
		Module.ResolveType(token.Value, Method.DeclaringType.GetGenericArguments(),
			Method.GetGenericArguments());

	/// <summary>
	/// Resolve local variable information from the specified index.
	/// </summary>
	/// <param name="operand">The zero-based index of the variable.</param>
	/// <returns>The local variable information.</returns>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="operand"/> does not identify a valid local variable for this method.
	/// </exception>
	public override LocalVariableInfo ResolveVariable(int operand)
	{
		if (operand < 0 || operand > MethodBody.LocalVariables.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(operand));
        }

        return MethodBody.LocalVariables[operand];
	}
}
