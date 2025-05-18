namespace Lyt.Reflector.IL;

/// <summary>
/// An instruction that references member information (<see cref="OperandType.InlineTok"/>).
/// </summary>
public class MemberInstruction : Instruction<Token, MemberInfo>
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
	private MemberInstruction(InstructionList parent, int offset, OpCode opCode,
		Token token)
		: base(parent, offset, opCode, token)
	{
	}

	/// <summary>
	/// Create an instruction for the specified byte offset and operation code (opcode).
	/// </summary>
	/// <param name="parent">The set of instructions containing this instruction.</param>
	/// <param name="offset">The byte offset of this instruction.</param>
	/// <param name="opCode">The operation code (opcode) for this instruction.</param>
	/// <param name="token">The operand (token) for this instruction.</param>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="parent"/> is null.
	/// </exception>
	public static IInstruction Create(InstructionList parent, int offset, OpCode opCode,
		Token token)
	{
		MemberInfo member = parent.ResolveMember(token);

		if (member is MethodBase method)
        {
            return new MethodInstruction(parent, offset, opCode, token, method);
        }

        if (member is FieldInfo field)
        {
            return new FieldInstruction(parent, offset, opCode, token, field);
        }

        if (member is Type type)
        {
            return new TypeInstruction(parent, offset, opCode, token, type);
        }

        return new MemberInstruction(parent, offset, opCode, token);
	}

	/// <summary> Resolve the member for this instructon. </summary>
	/// <exception cref="System.ArgumentException">
	/// <see cref="Instruction{TOperand, TValue}.Operand"/> is not a member within the scope
	/// of <see cref="IInstruction.Parent"/>.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <see cref="Instruction{TOperand, TValue}.Operand"/> is not a valid member within
	/// the scope <see cref="IInstruction.Parent"/>.
	/// </exception>
	public override void Resolve() =>
		Value = Value ?? Parent.ResolveMember(Operand);

	/// <summary> Format the value. </summary>
	/// <returns>The formatted value.</returns>
	protected override string FormatValue() => $"0x{Operand:X}";
}
