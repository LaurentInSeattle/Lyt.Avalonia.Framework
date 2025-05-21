namespace Lyt.Reflector.Tests;

    [TestClass]
public class SwitchInstruction_Tests : InstructionHelper
{
    [TestMethod]
    public void InlineSwitch_SwitchInstruction()
    {
        foreach (OpCode opCode in AllOpCodes.Instance.OfType(OperandType.InlineSwitch))
            TestInstruction(opCode);
    }

    private Label[] CreateLabels(ILGenerator il, int count)
    {
        var labels = new Label[count];
        for (int index = 0; index < count; index++)
            labels[index] = il.DefineLabel();
        return labels;
    }

    private void CreateBranches(ILGenerator il, Label[] labels)
    {
        for(int index = 0; index < labels.Length; index++)
        {
            il.MarkLabel(labels[index]);
            il.Emit(OpCodes.Ret);
        }
    }

    private string GetExpectedText(OpCode opCode, int baseOffset, int labelCount)
    {
        var builder = new StringBuilder(1024);

        builder.Append("IL_0000: ");
        builder.Append(opCode.Name);
        builder.Append(" (");
        for (int index = 0; index < labelCount; index++)
        {
            if (index > 0)
                builder.Append(", ");
            builder.Append($"IL_{baseOffset + index:X4}");
        }

        builder.Append(")");
        return builder.ToString();
    }

    private void TestInstruction(OpCode opCode)
    {
        int labelCount = 3;

        var method = CreateMethod(il =>
        {
            Label[] labels = CreateLabels(il, labelCount);
            il.Emit(OpCodes.Switch, labels);
            CreateBranches(il, labels);
        });

        var instructions = method.GetIL();
        Assert.AreEqual(labelCount + 1, instructions.Count);

        int baseOffset = opCode.Size + sizeof(int) * (labelCount + 1);
        string expectedText = GetExpectedText(opCode, baseOffset, labelCount);

        Assert.IsInstanceOfType(instructions.Instructions[0], typeof(SwitchInstruction));
        var switchInstruction = instructions.Instructions[0] as SwitchInstruction;
        TestInstruction(switchInstruction, instructions, opCode, expectedText);

        Assert.AreEqual(labelCount, switchInstruction.BranchOperands.Count);
        for (int index = 0; index < labelCount; index++)
            Assert.AreEqual(switchInstruction.BranchOperands[index], index);

        Assert.AreEqual(labelCount, switchInstruction.Value.Count);

        for(int index = 0; index < labelCount; index++)
        {
            IInstruction instruction = switchInstruction.Value[index];
            int offset = baseOffset + index;

            TestInstruction(instruction, instructions, OpCodes.Ret,
                $"IL_{offset:X4}: ret", offset, true);
        }
    }
}
