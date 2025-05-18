namespace Lyt.Reflector.IL;

/// <summary>
/// The base class for instructions where the operand is the different from the
/// resolved value for the operand.
/// </summary>
/// <typeparam name="TOperand">The type of the operand for this instruction.</typeparam>
/// <typeparam name="TValue">The type of the resolved value of the operand for this instruction</typeparam>
/// <remarks>
/// Create an instance for the specified byte offset and operation code (opcode).
/// </remarks>
/// <param name="parent">The set of instructions containing this instruction.</param>
/// <param name="offset">The byte offset of this instruction.</param>
/// <param name="opCode">The operation code (opcode) for this instruction.</param>
/// <param name="operand">The operand for this instruction.</param>
public class Instruction<TOperand, TValue>(
    IInstructionList parent, int offset, OpCode opCode, TOperand operand) 
    : Instruction(parent, offset, opCode), IInstruction<TOperand, TValue>
        where TOperand : struct
{
    /// <summary> Gets the operand for this instruction. </summary>
    public TOperand Operand { get; } = operand;

    /// <summary> Gets the resolved value of the operand for this instruction. </summary>
    public TValue? Value { get; protected set; }

    /// <summary> Get the operand for this instruction. </summary>
    /// <returns>The operand for this instruction.</returns>
    public override object? GetOperand() => this.Operand;

    /// <summary> Get the resolved value of the operand for this instruction. </summary>
    /// <returns>The resolved value of the operand for this instruction.</returns>
    public override object? GetValue() => this.Value;

    /// <summary> Get a textual representation of this instruction. </summary>
    /// <param name="includeLabel">A value indicating if a label should be included.</param>
    /// <returns>A textual representation of this instruction.</returns>
    public override string ToString(bool includeLabel) 
        =>
            includeLabel ? 
                $"{this.Label}: {this.OpCode.Name} {this.FormatValue()}" :
                $"{this.OpCode.Name} {this.FormatValue()}";

    /// <summary> Format the value. </summary>
    /// <returns>The formatted value.</returns>
    protected virtual string FormatValue() 
        => this.Value is null ? 
            InvalidValue :
            this.Value is null ? string.Empty : this.Value.ToString()!;
}
