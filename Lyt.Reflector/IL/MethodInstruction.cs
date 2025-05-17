namespace Lyt.Reflector.IL;

/// <summary>
/// An instruction that references method information (<see cref="OperandType.InlineMethod"/> or
/// <see cref="OperandType.InlineTok"/>).
/// </summary>
public class MethodInstruction : Instruction<Token, MethodBase>
{
	/// <summary>
	/// Create an instance for the specified byte offset and operation code (opcode).
	/// </summary>
	/// <param name="parent">The set of instructions containing this instruction.</param>
	/// <param name="offset">The byte offset of this instruction.</param>
	/// <param name="opCode">The operation code (opcode) for this instruction.</param>
	/// <param name="token">The operand (token) for this instruction.</param>
	/// <param name="method">The (optional) method for this instruction.</param>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="parent"/> is null.
	/// </exception>
	public MethodInstruction(IInstructionList parent, int offset, OpCode opCode,
		Token token, MethodBase method = null)
		: base(parent, offset, opCode, token) =>
		Value = method;

	/// <summary>
	/// Resolve the method for this instructon.
	/// </summary>
	/// <exception cref="System.ArgumentException">
	/// <see cref="Instruction{TOperand, TValue}.Operand"/> is not a method within the scope
	/// of <see cref="IInstruction.Parent"/>.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <see cref="Instruction{TOperand, TValue}.Operand"/> is not a valid method within
	/// the scope <see cref="IInstruction.Parent"/>.
	/// </exception>
	public override void Resolve() =>
		Value = Value ?? Parent.ResolveMethod(Operand);

	/// <summary>
	/// Format the value.
	/// </summary>
	/// <returns>The formatted value.</returns>
	protected override string FormatValue()
	{
		if (Value == null)
			return InvalidValue;

		var builder = new StringBuilder(1024);
		if (OpCode.OperandType == OperandType.InlineTok)
			builder.Append("method ");

		MethodSignature.AppendConventions(builder, Value.CallingConvention);

		AppendReturnType(builder);
		AppendFullName(builder);

		if (Value.IsGenericMethod)
			AppendTypeParameters(builder, Value.GetGenericArguments());

		AppendParameters(builder);

		return builder.ToString();
	}

	// Append the full name of the method including the namespace and declaring type
	private void AppendFullName(StringBuilder builder)
	{
		AppendType(builder, Value.DeclaringType);
		builder.Append("::");
		builder.Append(Value.Name);
	}

	// Append the return type of the method
	private void AppendReturnType(StringBuilder builder)
	{
		AppendType(builder, Value is MethodInfo methodInfo ?
			methodInfo.ReturnType : typeof(void), true);
		builder.Append(' ');
	}

	// Append the parameters passed to the method
	private void AppendParameters(StringBuilder builder)
	{
		builder.Append('(');

		bool isFirstParameter = true;
		foreach (ParameterInfo parameter in Value.GetParameters())
		{
			if (isFirstParameter)
				isFirstParameter = false;
			else
				builder.Append(", ");

			AppendType(builder, parameter.ParameterType, true);
		}

		builder.Append(')');
	}
}
