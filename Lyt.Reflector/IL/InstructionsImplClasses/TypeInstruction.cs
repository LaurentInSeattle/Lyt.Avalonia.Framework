namespace Lyt.Reflector.IL;

/// <summary>
/// An instruction that references type information (<see cref="OperandType.InlineType"/> or
/// <see cref="OperandType.InlineTok"/>).
/// </summary>
public class TypeInstruction : Instruction<Token, Type>
{
	/// <summary>
	/// Create an instance for the specified byte offset and operation code (opcode).
	/// </summary>
	/// <param name="parent">The set of instructions containing this instruction.</param>
	/// <param name="offset">The byte offset of this instruction.</param>
	/// <param name="opCode">The operation code (opcode) for this instruction.</param>
	/// <param name="token">The operand (token) for this instruction.</param>
	/// <param name="type">The (optional) type for this instruction.</param>
	public TypeInstruction(MethodInstructionsList parent, int offset, OpCode opCode,
		Token token, Type? type = null)
		: base(parent, offset, opCode, token) =>
        this.Value = type;

	/// <summary> Resolve the type for this instructon. </summary>
	/// <exception cref="System.ArgumentException">
	/// <see cref="Instruction{TOperand, TValue}.Operand"/> is not a type within the scope
	/// of <see cref="IInstruction.Parent"/>.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <see cref="Instruction{TOperand, TValue}.Operand"/> is not a valid type within
	/// the scope <see cref="IInstruction.Parent"/>.
	/// </exception>
	public override void Resolve() => Value = Value ?? Parent.ResolveType(Operand);

    /// <summary> Returns the formatted value. </summary>
	protected override string FormatValue() => Value == null ? InvalidValue : FormatType(Value);
}
