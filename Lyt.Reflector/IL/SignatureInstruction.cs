namespace Lyt.Reflector.IL;

/// <summary>
/// An instruction that references signature information (<see cref="OperandType.InlineSig"/>).
/// </summary>
public class SignatureInstruction : Instruction<Token, MethodSignature>
{
	/// <summary>
	/// Create an instance for the specified byte offset and operation code (opcode).
	/// </summary>
	/// <param name="parent">The set of instructions containing this instruction.</param>
	/// <param name="offset">The byte offset of this instruction.</param>
	/// <param name="opCode">The operation code (opcode) for this instruction.</param>
	/// <param name="token">The operand (token) for this instruction.</param>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="parent"/> is null.
	/// </exception>
	public SignatureInstruction(InstructionList parent, int offset, OpCode opCode,
		Token token)
		: base(parent, offset, opCode, token)
	{
	}

	/// <summary>
	/// Resolve the signature for this instructon.
	/// </summary>
	/// <exception cref="System.ArgumentException">
	/// <see cref="Instruction{TOperand, TValue}.Operand"/> is not a signature within the scope
	/// of <see cref="IInstruction.Parent"/>.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <see cref="Instruction{TOperand, TValue}.Operand"/> is not a valid signature within
	/// the scope <see cref="IInstruction.Parent"/>.
	/// </exception>
	public override void Resolve() =>
		Value = Value ?? new MethodSignature(Parent, Parent.ResolveSignature(Operand));

	/// <summary>
	/// Format the value.
	/// </summary>
	/// <returns>The formatted value.</returns>
	protected override string FormatValue() =>
		Value == null ? InvalidValue : Value.ToString();

}
