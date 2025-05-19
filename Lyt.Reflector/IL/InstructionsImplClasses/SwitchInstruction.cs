namespace Lyt.Reflector.IL;

/// <summary>
/// A switch instruction (<see cref="OperandType.InlineSwitch"/>).
/// </summary>
public class SwitchInstruction : Instruction<int, IReadOnlyList<IInstruction>>
{
	/// <summary>
	/// Create an instance for the specified byte offset and operation code (opcode).
	/// </summary>
	/// <param name="parent">The set of instructions containing this instruction.</param>
	/// <param name="offset">The byte offset of this instruction.</param>
	/// <param name="opCode">The operation code (opcode) for this instruction.</param>
	/// <param name="operand">The operand (number of branches) for this instruction.</param>
	/// <param name="branchOperands">The operands for each of the branches of this switch instruction.</param>
	/// <exception cref="System.ArgumentException">
	/// <paramref name="branchOperands"/> contains an incorrect number of branches.
	/// </exception>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="branchOperands"/> or <paramref name="parent"/> is null.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="operand"/> is less than zero.
	/// </exception>
	public SwitchInstruction(InstructionList parent, int offset, OpCode opCode,
		int operand, int[] branchOperands)
		: base(parent, offset, opCode, operand)
	{
		if (operand < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(operand));
        }

        BranchOperands = new ReadOnlyCollection<int>(branchOperands ??
			throw new ArgumentNullException(nameof(branchOperands)));

		if (branchOperands.Length != operand)
        {
            throw new ArgumentException(null, nameof(branchOperands));
        }

        branchBase = offset + opCode.Size + sizeof(int) * (operand + 1);
	}

	private readonly int branchBase;

	/// <summary>
	/// Gets the branch operands for this instruction.
	/// </summary>
	public IReadOnlyList<int> BranchOperands { get; }

	/// <summary>
	/// Resolve the branches for this instructon.
	/// </summary>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// One of the values in <see cref="BranchOperands"/> does not resolve to the byte
	/// offset of an instruction within <see cref="IInstruction.Parent"/>.
	/// </exception>
	public override void Resolve() =>
		Value = Value ?? new ReadOnlyCollection<IInstruction>(BranchOperands
			.Select(operand => Parent.ResolveInstruction(branchBase + operand))
			.ToList());

	/// <summary> Format the value. </summary>
	/// <returns>The formatted value.</returns>
	protected override string FormatValue()
	{
		var builder = new StringBuilder(1024);
		builder.Append('(');
		foreach (int operand in BranchOperands)
		{
			if (builder.Length > 1)
            {
                builder.Append(", ");
            }

            builder.Append(FormatLabel(branchBase + operand));
		}

		builder.Append(')');
		return builder.ToString();
	}
}
