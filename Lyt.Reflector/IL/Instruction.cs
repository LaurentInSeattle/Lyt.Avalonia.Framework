namespace Lyt.Reflector.IL;

/// <summary>
/// The base class for all instructions.  Also, the sole class for
/// instructions without an operand.
/// </summary>
public class Instruction : IInstruction
{
	/// <summary>
	/// Create an instance for the specified byte offset and operation code (opcode).
	/// </summary>
	/// <param name="parent">The set of instructions containing this instruction.</param>
	/// <param name="offset">The byte offset of this instruction.</param>
	/// <param name="opCode">The operation code (opcode) for this instruction.</param>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="parent"/> is null.
	/// </exception>
	public Instruction(IInstructionList parent, int offset, OpCode opCode)
	{
		Parent = parent ?? throw new ArgumentNullException(nameof(parent));
		OpCode = opCode;
		Offset = offset;
	}

	/// <summary>
	/// The text displayed for an unresolved value.
	/// </summary>
	protected const string InvalidValue = "?";

	/// <summary>
	/// Gets a value indicating if this instruction if the target of a branch or
	/// switch instruction.
	/// </summary>
	public bool IsTarget { get; internal set; }

	/// <summary>
	/// Gets a label for this instruction.
	/// </summary>
	public string Label =>
		FormatLabel(Offset);

	/// <summary>
	/// Gets the byte offset of this instruction.
	/// </summary>
	public int Offset { get; }

	/// <summary>
	/// Gets the operation code (opcode) for this instruction.
	/// </summary>
	public OpCode OpCode { get; }

	/// <summary>
	/// Gets the set of instructions containing this instruction.
	/// </summary>
	public IInstructionList Parent { get; }

	/// <summary>
	/// Get the operand for this instruction.
	/// </summary>
	/// <returns>The operand for this instruction.</returns>
	public virtual object GetOperand() => null;

	/// <summary>
	/// Get the resolved value of the operand for this instruction.
	/// </summary>
	/// <returns>The resolved value of the operand for this instruction.</returns>
	public virtual object GetValue() => null;

	/// <summary>
	/// Resolve the value for this instruction from the operand.
	/// </summary>
	public virtual void Resolve() { }

	/// <summary>
	/// Get a textual representation of this instruction.
	/// </summary>
	/// <returns>A textual representation of this instruction.</returns>
	public sealed override string ToString() =>
		ToString(true);

	/// <summary>
	/// Get a textual representation of this instruction.
	/// </summary>
	/// <param name="includeLabel">A value indicating if a label should be included.</param>
	/// <returns>A textual representation of this instruction.</returns>
	public virtual string ToString(bool includeLabel) =>
		includeLabel ? $"{Label}: {OpCode.Name}" : $"{OpCode.Name}";

	/// <summary>
	/// Append the text for the specified type to the specified string builder.
	/// </summary>
	/// <param name="builder">The string builder to which the text is appended.</param>
	/// <param name="type">The type to format.</param>
	/// <param name="includeModifiers">A value indicating if type modifiers should be included.</param>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="builder"/> or <paramref name="type"/> is null.
	/// </exception>
	protected void AppendType(StringBuilder builder, Type type,
		bool includeModifiers = false) =>
		CilTypes.Instance.AppendType(builder, Parent, type, includeModifiers);

	/// <summary>
	/// Append the text for the specified type parameters to the specified string builder.
	/// </summary>
	/// <param name="builder">The string builder to which the text is appended.</param>
	/// <param name="types">The type parameters.</param>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="builder"/> or <paramref name="types"/> is null.
	/// </exception>
	protected void AppendTypeParameters(StringBuilder builder, IEnumerable<Type> types) =>
		CilTypes.Instance.AppendTypeParameters(builder, Parent, types);

	/// <summary>
	/// Format the label for the instruction at the specified offset.
	/// </summary>
	/// <param name="offset">The zero-based byte offset of the instruction.</param>
	/// <returns>The label for the instruction at the specified offset.</returns>
	protected static string FormatLabel(int offset) =>
		$"IL_{offset:X4}";

	/// <summary>
	/// Format the specified type for this instruction.
	/// </summary>
	/// <param name="type">The type to format.</param>
	/// <param name="includeModifiers">A value indicating if type modifiers should be included.</param>
	/// <returns>The text for the specified type.</returns>
	protected string FormatType(Type type, bool includeModifiers = false) =>
		CilTypes.Instance.FormatType(Parent, type, includeModifiers);
}

/// <summary>
/// The sole class for instructions where the operand is the same as the
/// resolved value for the operand.
/// </summary>
/// <typeparam name="TOperand">The type of the operand for this instruction.</typeparam>
public class Instruction<TOperand> : Instruction, IInstruction<TOperand, TOperand>
{
	/// <summary>
	/// Create an instance for the specified byte offset and operation code (opcode).
	/// </summary>
	/// <param name="parent">The set of instructions containing this instruction.</param>
	/// <param name="offset">The byte offset of this instruction.</param>
	/// <param name="opCode">The operation code (opcode) for this instruction.</param>
	/// <param name="operand">The operand for this instruction.</param>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="parent"/> is null.
	/// </exception>
	public Instruction(IInstructionList parent, int offset, OpCode opCode, TOperand operand)
		: base(parent, offset, opCode) =>
		Operand = operand;

	/// <summary>
	/// Gets the operand for this instruction.
	/// </summary>
	public TOperand Operand { get; }

	/// <summary>
	/// Gets the resolved value of the operand for this instruction.
	/// </summary>
	public TOperand Value => Operand;

	/// <summary>
	/// Get the operand for this instruction.
	/// </summary>
	/// <returns>The operand for this instruction.</returns>
	public override object GetOperand() => Operand;

	/// <summary>
	/// Get the resolved value of the operand for this instruction.
	/// </summary>
	/// <returns>The resolved value of the operand for this instruction.</returns>
	public override object GetValue() => Operand;

	/// <summary>
	/// Get a textual representation of this instruction.
	/// </summary>
	/// <param name="includeLabel">A value indicating if a label should be included.</param>
	/// <returns>A textual representation of this instruction.</returns>
	public override string ToString(bool includeLabel) =>
		includeLabel ? $"{Label}: {OpCode.Name} {FormatValue()}" :
			$"{OpCode.Name} {FormatValue()}";

	/// <summary>
	/// Format the value.
	/// </summary>
	/// <returns>The formatted value.</returns>
	protected virtual string FormatValue() =>
		Value == null ? InvalidValue : Value.ToString();
}

/// <summary>
/// The base class for instructions where the operand is the different from the
/// resolved value for the operand.
/// </summary>
/// <typeparam name="TOperand">The type of the operand for this instruction.</typeparam>
/// <typeparam name="TValue">The type of the resolved value of the operand for this instruction</typeparam>
public class Instruction<TOperand, TValue> : Instruction, IInstruction<TOperand, TValue>
{
	#region Constructors
	/// <summary>
	/// Create an instance for the specified byte offset and operation code (opcode).
	/// </summary>
	/// <param name="parent">The set of instructions containing this instruction.</param>
	/// <param name="offset">The byte offset of this instruction.</param>
	/// <param name="opCode">The operation code (opcode) for this instruction.</param>
	/// <param name="operand">The operand for this instruction.</param>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="parent"/> is null.
	/// </exception>
	public Instruction(IInstructionList parent, int offset, OpCode opCode, TOperand operand)
		: base(parent, offset, opCode) =>
		Operand = operand;
	#endregion // Constructors

	#region Properties
	/// <summary>
	/// Gets the operand for this instruction.
	/// </summary>
	public TOperand Operand { get; }

	/// <summary>
	/// Gets the resolved value of the operand for this instruction.
	/// </summary>
	public TValue Value { get; protected set; }
	#endregion // Properties

	#region Public methods
	/// <summary>
	/// Get the operand for this instruction.
	/// </summary>
	/// <returns>The operand for this instruction.</returns>
	public override object GetOperand() => Operand;

	/// <summary>
	/// Get the resolved value of the operand for this instruction.
	/// </summary>
	/// <returns>The resolved value of the operand for this instruction.</returns>
	public override object GetValue() => Value;

	/// <summary>
	/// Get a textual representation of this instruction.
	/// </summary>
	/// <param name="includeLabel">A value indicating if a label should be included.</param>
	/// <returns>A textual representation of this instruction.</returns>
	public override string ToString(bool includeLabel) =>
		includeLabel ? $"{Label}: {OpCode.Name} {FormatValue()}" :
			$"{OpCode.Name} {FormatValue()}";
	#endregion // Public methods

	#region Protected methods
	/// <summary>
	/// Format the value.
	/// </summary>
	/// <returns>The formatted value.</returns>
	protected virtual string FormatValue() =>
		Value == null ? InvalidValue : Value.ToString();
	#endregion // Protected methods
}
