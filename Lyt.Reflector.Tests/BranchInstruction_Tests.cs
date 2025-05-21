namespace Lyt.Reflector.Tests;

[TestClass]
public class BranchInstruction_Tests : InstructionHelper
{
	[TestMethod]
	public void InlineBrTarget_BranchInstruction()
	{
		foreach (OpCode opCode in AllOpCodes.Instance.OfType(OperandType.InlineBrTarget))
		{
			MethodInfo method = CreateMethod(il =>
			{
				il.Emit(opCode, 1);
				il.Emit(OpCodes.Nop);
				il.Emit(OpCodes.Ret);
			});
			TestBranchInstruction(method, opCode, (int)1, sizeof(int) + 1,
				"IL_0006", instructions => instructions.Instructions[2]);
		}
	}

	[TestMethod]
	public void InlineBrTarget_Invalid_BranchInstruction()
	{
		int instructionSize = sizeof(int) + 1;
		int operand = 123;

		MethodInfo method = CreateMethod(il =>
		{
			il.Emit(OpCodes.Beq, operand);
			il.Emit(OpCodes.Nop);
			il.Emit(OpCodes.Ret);
		});
		TestBranchInstruction(method, OpCodes.Beq, operand, instructionSize,
			$"? // IL_{operand + instructionSize:X4}", instructions => null);
	}

	[TestMethod]
	public void ShortInlineBrTarget_BranchInstruction()
	{
		foreach (OpCode opCode in AllOpCodes.Instance.OfType(OperandType.ShortInlineBrTarget))
		{
			MethodInfo method = CreateMethod(il =>
			{
				il.Emit(opCode, (sbyte)1);
				il.Emit(OpCodes.Nop);
				il.Emit(OpCodes.Ret);
			});
			TestBranchInstruction(method, opCode, (sbyte)1, sizeof(sbyte) + 1,
				"IL_0003", instructions => instructions.Instructions[2]);
		}
	}

	[TestMethod]
	public void ShortInlineBrTarget_Invalid_BranchInstruction()
	{
		int instructionSize = sizeof(sbyte) + 1;
		sbyte operand = 123;

		MethodInfo method = CreateMethod(il =>
		{
			il.Emit(OpCodes.Beq_S, operand);
			il.Emit(OpCodes.Nop);
			il.Emit(OpCodes.Ret);
		});
		TestBranchInstruction(method, OpCodes.Beq_S, operand, instructionSize,
			$"? // IL_{operand + instructionSize:X4}", instructions => null);
	}

	private void TestBranchInstruction<TOperand>(MethodInfo method, OpCode opCode,
		TOperand operand, int instructionSize, string expectedTarget,
		Func<MethodInstructionsList, IInstruction> getExpectedValue)
		where TOperand : struct, IBinaryInteger<TOperand>
	{
		Assert.IsNotNull(method, $"{opCode.Name} method failed");

		var instructions = method.GetIL();
		Assert.IsNotNull(instructions, $"{opCode.Name} instructions failed");
		Assert.AreEqual(3, instructions.Count,
			$"{opCode.Name} instructions.Count failed");

		var instruction = instructions.Instructions[0] as BranchInstruction<TOperand>;
		Assert.IsNotNull(instruction, $"{opCode.Name} instruction failed");

		Assert.AreEqual(operand, instruction.Operand,
			$"{opCode.Name} instruction.Token failed");
		Assert.AreEqual(operand, instruction.GetOperand(),
			$"{opCode.Name} instruction.GetOperand() failed");

		IInstruction expectedValue = getExpectedValue(instructions);
		Assert.AreEqual(expectedValue, instruction.Value,
			$"{opCode.Name} instruction.Value failed");
		Assert.AreEqual(expectedValue, instruction.GetValue(),
			$"{opCode.Name} instruction.GetValue() failed");

		Assert.AreEqual(
			instructions.Instructions[0].ToString(), 
			$"IL_0000: {opCode.Name} {expectedTarget}",
			$"{opCode.Name} instructions[0].ToString() failed");
		Assert.AreEqual(instructions.Instructions[1].ToString(), $"IL_{instructionSize:X4}: nop",
			$"{opCode.Name} instructions[1].ToString() failed");
		Assert.AreEqual(instructions.Instructions[2].ToString(), $"IL_{instructionSize + 1:X4}: ret",
			$"{opCode.Name} instructions[2].ToString() failed");
	}
}
