namespace Lyt.Reflector.IL;

/// <summary>
/// A branch instruction (<see cref="OperandType.InlineBrTarget"/> or
/// <see cref="OperandType.ShortInlineBrTarget"/>).
/// </summary>
/// <typeparam name="TOperand">The type of operand for this instruction.</typeparam>
public class BranchInstruction<TOperand> : Instruction<TOperand, IInstruction>
	where TOperand : struct, IComparable, IFormattable, IConvertible,
		IComparable<TOperand>, IEquatable<TOperand>
{
    private int branchBase;

    /// <summary>
    /// Create an instance for the specified byte offset, operation code (opcode),
    /// operand type, and operand size.
    /// </summary>
    /// <param name="parent">The set of instructions containing this instruction.</param>
    /// <param name="offset">The byte offset of this instruction.</param>
    /// <param name="opCode">The operation code (opcode) for this instruction.</param>
    /// <param name="operand">The operand for this instruction.</param>
    /// <param name="operandSize">The byte size of the operand for this instruction.</param>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="parent"/> is null.
    /// </exception>
    public BranchInstruction(IInstructionList parent, int offset, OpCode opCode,
		TOperand operand, byte operandSize)
		: base(parent, offset, opCode, operand) =>
		branchBase = offset + OpCode.Size + operandSize;

	/// <summary> Resolve the branch for this instructon. </summary>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <see cref="Instruction{TOperand, TValue}.Operand"/> does not resolve to the byte
	/// offset of an instruction within the scope of <see cref="IInstruction.Parent"/>.
	/// </exception>
	public override void Resolve() =>
		Value = Value ?? Parent.ResolveInstruction(branchBase + Operand.ToInt32(null));

	/// <summary> Format the value. </summary>
	/// <returns>The formatted value.</returns>
	protected override string FormatValue()
	{
		string targetLabel = FormatLabel(branchBase + Operand.ToInt32(null));
		return Value == null ? $"{InvalidValue} // {targetLabel}" : targetLabel;
	}
}
