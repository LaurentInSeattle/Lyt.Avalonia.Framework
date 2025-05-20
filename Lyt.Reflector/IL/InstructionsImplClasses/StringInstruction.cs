namespace Lyt.Reflector.IL;

/// <summary>
/// An instruction that references signature information (<see cref="OperandType.InlineString"/>).
/// </summary>
public class StringInstruction : Instruction<Token, string>
{
	/// <summary>
	/// Create an instance for the specified byte offset and operation code (opcode).
	/// </summary>
	/// <param name="parent">The set of instructions containing this instruction.</param>
	/// <param name="offset">The byte offset of this instruction.</param>
	/// <param name="opCode">The operation code (opcode) for this instruction.</param>
	/// <param name="token">The operand (token) for this instruction.</param>
	public StringInstruction(MethodInstructionsList parent, int offset, OpCode opCode,
		Token token)
		: base(parent, offset, opCode, token)
	{
	}

	/// <summary>
	/// Resolve the string for this instructon.
	/// </summary>
	/// <exception cref="System.ArgumentException">
	/// <see cref="Instruction{TOperand, TValue}.Operand"/> is not a string within the scope
	/// of <see cref="IInstruction.Parent"/>.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <see cref="Instruction{TOperand, TValue}.Operand"/> is not a valid string within
	/// the scope <see cref="IInstruction.Parent"/>.
	/// </exception>
	public override void Resolve() =>
		Value = Value ?? Parent.ResolveString(Operand);

    /// <summary> Returns the formatted value. </summary>
	protected override string FormatValue()
	{
		var builder = new StringBuilder(Value.Length << 1);
		builder.Append('\"');
		bool asByteArray = false;

		foreach (char chr in Value)
		{
			switch (chr)
			{
				// Escape sequences recognized by ILASM
				case '\\': builder.Append("\\\\"); break;
				case '\"': builder.Append("\\\""); break;
				case '\a': builder.Append("\\a"); break;
				case '\b': builder.Append("\\b"); break;
				case '\f': builder.Append("\\f"); break;
				case '\n': builder.Append("\\n"); break;
				case '\r': builder.Append("\\r"); break;
				case '\t': builder.Append("\\t"); break;
				case '\v': builder.Append("\\v"); break;

				default:
					// Unescaped characters recognized by ILASM (we think?)
					if (Char.IsLetterOrDigit(chr) || Char.IsPunctuation(chr) ||
						Char.IsSeparator(chr) || Char.IsSymbol(chr))
					{
						builder.Append(chr);
						break;
					}

					// Escaped octal sequences recognized by ILASM
					if (chr > 0 && chr < 20)
					{
						builder.Append('\\');
						builder.Append(Convert.ToString(chr, 8).PadLeft(3, '0'));
						break;
					}

					// Characters where ILASM requires us to fall back to a byte array
					asByteArray = true;
					break;
			}

			if (asByteArray)
            {
                break;
            }
        }

		if (!asByteArray)
		{
			builder.Append('\"');
			return builder.ToString();
		}

		return FormatByteArray();
	}

	private string FormatByteArray()
	{
		var builder = new StringBuilder(Value.Length << 2);
		builder.Append("bytearray(");
		bool isFirstByte = true;

		foreach (byte nextByte in Encoding.Unicode.GetBytes(Value))
        {
            if (isFirstByte)
			{
				builder.AppendFormat("{0:X2}", nextByte);
				isFirstByte = false;
			}
			else
            {
                builder.AppendFormat(" {0:X2}", nextByte);
            }
        }

        builder.Append(")");
		return builder.ToString();
	}
}
