namespace Lyt.Reflector.Tests;

[TestClass]
public sealed class OpCodes_Tests
{
    public const int ExpectedOpCodeCount = 226;
    private enum MyEnum
    {
        Yes,
        No
    };

    [TestMethod]
    public void Constructor_AllOpCodes()
    {
        _ = Enum.TryParse("Yes", out MyEnum myEnum);
        _ = DateTime.TryParse("2018-17-17", out DateTime myDateTime);

        //			Console.WriteLine("©");

        //			foreach (OpCode opCode in AllOpCodes.Instance.Values
        //				.Where(o => o.OperandType == OperandType.InlineSig))
        //				Console.WriteLine(opCode.Name);
        // need to test with a calli instruction

        int x = 0;
        var method = MethodBase.GetCurrentMethod()!;
        Debug.WriteLine(method.Name);
        MethodInstructionsList methodIL = method.GetIL();
        foreach (IInstruction instruction in methodIL.Instructions)
        {
            string? text = instruction.ToString();
            Debug.WriteLine(text);
            x++;
        }

        Assert.AreEqual(ExpectedOpCodeCount, AllOpCodes.Instance.Count);
    }
}
