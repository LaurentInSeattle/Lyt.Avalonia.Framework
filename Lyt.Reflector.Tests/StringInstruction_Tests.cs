namespace Lyt.Reflector.Tests;

    [TestClass]
public class StringInstruction_Tests : InstructionHelper
{
    private static readonly string TestEncodeString = "AAA\x1\x2\\\"\aBBB\b\f\n\r\t\vCCC©";
    private static readonly Token TestToken = new Token(TokenType.String, 5);
    private static readonly string TestByteArray = "A\u0080";
    private const string TestString = "Hello";

    //[TestMethod]
    //public void InlineString_StringInstruction() =>
    //    TestInstruction(OpCodes.Ldstr, TestString,
    //        $"\"{TestString}\"");

    //[TestMethod]
    //public void InlineString_Encode_StringInstruction() =>
    //    TestInstruction(OpCodes.Ldstr, TestEncodeString,
    //        @"""AAA\001\002\\\""\aBBB\b\f\n\r\t\vCCC©""");

    //[TestMethod]
    //public void InlineString_ByteArray_StringInstruction() =>
    //    TestInstruction(OpCodes.Ldstr, TestByteArray,
    //        "bytearray(41 00 80 00)");

    private void TestInstruction(OpCode opCode, string testString, string expectedText)
    {
        MethodInfo method = CreateMethod(il =>
            il.Emit(opCode, testString));

        TestInstruction(method.GetIL(), opCode, TestToken, testString,
            $@"IL_0000: {opCode.Name} {expectedText}", expectedType: typeof(StringInstruction));
    }
}
