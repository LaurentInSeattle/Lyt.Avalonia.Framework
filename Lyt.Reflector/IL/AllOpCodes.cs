namespace Lyt.Reflector.IL; 

/// <summary>
/// A dictionary of all of the operation codes for Common Intermediate Language (CIL),
/// see: https://en.wikipedia.org/wiki/Common_Intermediate_Language
/// </summary>
public class AllOpCodes : ReadOnlyDictionary<short, OpCode>
{
	/// <summary> Create the sole singleton instance of this class. </summary>
	private AllOpCodes() : base(GetOpCodes().ToDictionary(opCode => opCode.Value)) { }

	/// <summary> Gets the sole singleton instance of this class. </summary>
	public static AllOpCodes Instance { get; } = new AllOpCodes();

	/// <summary> Get all of the operation codes (opcodes) with the specified operand type. </summary>
	/// <param name="operandType">The type of operand.</param>
	/// <returns>The operation codes (opcodes) with the specified operand type.</returns>
	public IEnumerable<OpCode> OfType(OperandType operandType) =>
		this.Values
			.Where(opCode => opCode.OperandType == operandType)
			.OrderBy(opCode => opCode.Name);

	private static IEnumerable<OpCode> GetOpCodes() =>
		typeof(OpCodes)
			.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static)
			.Where(field => field.FieldType == typeof(OpCode))
			.Select(field => field.GetValue(null) is OpCode opcode ? opcode : default);
}
