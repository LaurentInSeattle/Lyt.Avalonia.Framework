namespace Lyt.Reflector.IL;

/// <summary>
/// An instruction that references field information (<see cref="OperandType.InlineField"/>
/// or <see cref="OperandType.InlineTok"/>).
/// </summary>
public class FieldInstruction : Instruction<Token, FieldInfo>
{
	/// <summary> Create an instance for the specified byte offset and operation code (opcode). </summary>
	/// <param name="parent">The set of instructions containing this instruction.</param>
	/// <param name="offset">The byte offset of this instruction.</param>
	/// <param name="opCode">The operation code (opcode) for this instruction.</param>
	/// <param name="token">The operand (token) for this instruction.</param>
	/// <param name="field">The (optional) field for this instruction.</param>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="parent"/> is null.
	/// </exception>
	public FieldInstruction(IInstructionList parent, int offset, OpCode opCode,
		Token token, FieldInfo field = null)
		: base(parent, offset, opCode, token) =>
		Value = field;

	/// <summary> Resolve the field for this instructon. </summary>
	/// <exception cref="System.ArgumentException">
	/// <see cref="Instruction{TOperand, TValue}.Operand"/> is not a field within the scope
	/// of <see cref="IInstruction.Parent"/>.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <see cref="Instruction{TOperand, TValue}.Operand"/> is not a valid field within
	/// the scope of <see cref="IInstruction.Parent"/>.
	/// </exception>
	public override void Resolve() =>
		Value = Value ?? Parent.ResolveField(Operand);

	/// <summary> Format the value. </summary>
	/// <returns>The formatted value.</returns>
	protected override string FormatValue()
	{
		if (Value == null)
			return InvalidValue;

		var builder = new StringBuilder(1024);
		if (OpCode.OperandType == OperandType.InlineTok)
			builder.Append("field ");

		AppendType(builder, Value.FieldType);
		builder.Append(' ');
		AppendType(builder, Value.DeclaringType);
		builder.Append("::");
		builder.Append(Value.Name);
		return builder.ToString();
	}
}
