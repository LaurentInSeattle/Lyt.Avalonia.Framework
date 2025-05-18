namespace Lyt.Reflector.IL;

/// <summary> The base interface for all instructions. </summary>
public interface IInstruction
{
	/// <summary> 
	/// Gets a value indicating if this instruction if the target of a branch or switch instruction.
	/// </summary>
	bool IsTarget { get; }

	/// <summary> Gets a label for this instruction. </summary>
	string Label { get; }

	/// <summary> Gets the byte offset of this instruction. </summary>
	int Offset { get; }

	/// <summary> Gets the operation code (opcode) for this instruction. </summary>
	OpCode OpCode { get; }

	/// <summary> Gets the set of instructions containing this instruction. </summary>
	InstructionList Parent { get; }

	/// <summary> Get the operand for this instruction. </summary>
	/// <returns>The operand for this instruction.</returns>
	object? GetOperand();

	/// <summary> Get the resolved value of the operand for this instruction. </summary>
	/// <returns>The resolved value of the operand for this instruction.</returns>
	object? GetValue();

	/// <summary> Resolve the value for this instruction from the operand. </summary>
	void Resolve();
}

/// <summary> The interface for all instructions that have an operand. </summary>
/// <typeparam name="TOperand">The type of the operand for this instruction.</typeparam>
/// <typeparam name="TValue">The type of the resolved value of the operand for this instruction</typeparam>
public interface IInstruction<TOperand, TValue> : IInstruction where TOperand : struct
{
    /// <summary> Gets the operand for this instruction. </summary>
    TOperand Operand { get; }

	/// <summary> Gets the resolved value of the operand for this instruction. </summary>
	TValue? Value { get; }
}
