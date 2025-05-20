namespace Lyt.Reflector.IL;

/// <summary> A switch instruction (<see cref="OperandType.InlineSwitch"/>). </summary>
public class SwitchInstruction : Instruction<int, List<IInstruction>>
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
    public SwitchInstruction(
        MethodInstructionsList parent, int offset, OpCode opCode, int operand, 
        int[] branchOperands)
        : base(parent, offset, opCode, operand)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(operand);

        this.BranchOperands = new ReadOnlyCollection<int>(branchOperands ??
            throw new ArgumentNullException(nameof(branchOperands)));

        if (branchOperands.Length != operand)
        {
            throw new ArgumentException(null, nameof(branchOperands));
        }

        this.branchBase = offset + opCode.Size + sizeof(int) * (operand + 1);
    }

    private readonly int branchBase;

    /// <summary> Gets the branch operands for this instruction. </summary>
    public IReadOnlyList<int> BranchOperands { get; }

    /// <summary> Resolve the branches for this instruction. </summary>
    public override void Resolve() =>
        this.Value ??= 
            [.. this.BranchOperands.Select(
                operand => this.Parent.ResolveInstruction(this.branchBase + operand))];

    /// <summary> Returns the formatted value. </summary>
    protected override string FormatValue()
    {
        var builder = new StringBuilder(1024);
        builder.Append('(');
        foreach (int operand in this.BranchOperands)
        {
            if (builder.Length > 1)
            {
                builder.Append(", ");
            }

            builder.Append(FormatLabel(this.branchBase + operand));
        }

        builder.Append(')');
        return builder.ToString();
    }
}
