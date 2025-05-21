namespace Lyt.Reflector.Tests;

[TestClass]
public class Instruction_Tests : InstructionHelper
{
	[DataTestMethod]
	public void InlineNone_Instruction()
	{
		foreach (OpCode opCode in AllOpCodes.Instance.OfType(OperandType.InlineNone)
			.Where(o => !o.Name.StartsWith("ldarg") &&
				!o.Name.StartsWith("ldloc") &&
				!o.Name.StartsWith("stloc")))
			TestInstruction(opCode, $"IL_0000: {opCode.Name}");
	}

	[TestMethod]
	public void InlineI_Instruction()
	{
		const OperandType operandType = OperandType.InlineI;
		const int testArg = 257;

		foreach (OpCode opCode in AllOpCodes.Instance.OfType(operandType))
		{
			MethodInfo method = CreateMethod(opCode, operandType, il =>
				il.Emit(opCode, testArg));
			TestInstruction(method.GetIL(), opCode, testArg,
				$"IL_0000: {opCode.Name} {testArg}");
		}
	}

	[TestMethod]
	public void InlineI8_Instruction()
	{
		const OperandType operandType = OperandType.InlineI8;
		const long testArg = 123;

		foreach (OpCode opCode in AllOpCodes.Instance.OfType(operandType))
		{
			MethodInfo method = CreateMethod(opCode, operandType, il =>
				il.Emit(opCode, testArg));
			TestInstruction(method.GetIL(), opCode, testArg,
				$"IL_0000: {opCode.Name} {testArg}");
		}
	}

	[TestMethod]
	public void InlineR_Instruction()
	{
		const OperandType operandType = OperandType.InlineR;
		const double testArg = 123.456D;

		foreach (OpCode opCode in AllOpCodes.Instance.OfType(operandType))
		{
			MethodInfo method = CreateMethod(opCode, operandType, il =>
				il.Emit(opCode, testArg));
			TestInstruction(method.GetIL(), opCode, testArg,
				$"IL_0000: {opCode.Name} {testArg}");
		}
	}

	[TestMethod]
	public void ShortInlineI_Instruction()
	{
		//TestInstruction(OpCodes.Ldc_I4_S, (sbyte)-123, "IL_0000: ldc.i4.s -123");
		//TestInstruction(OpCodes.Unaligned, (byte)123, "IL_0000: unaligned. 123");
	}

	[TestMethod]
	public void ShortInlineR_Instruction()
	{
		const OperandType operandType = OperandType.ShortInlineR;
		const float testArg = 123.456F;

		foreach (OpCode opCode in AllOpCodes.Instance.OfType(operandType))
		{
			MethodInfo method = CreateMethod(opCode, operandType, il =>
				il.Emit(opCode, testArg));
			TestInstruction(method.GetIL(), opCode, testArg,
				$"IL_0000: {opCode.Name} {testArg}");
		}
	}
}
