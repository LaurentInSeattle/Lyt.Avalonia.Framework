namespace Lyt.Reflector.Tests;

[TestClass]
public class TypeInstruction_Tests : InstructionHelper
{
    public enum LocalEnum
    {
        Yes,
        No
    }

    public class LocalClass
    {
        public class InnerClass
        {
        }
    }

    //[TestMethod]
    //public void InlineType_TypeInstruction() =>
    //    TestInstructions(AllOpCodes.Instance.OfType(OperandType.InlineType));

    //[TestMethod]
    //public void InlineTok_TypeInstruction() =>
    //    TestInstructions(AllOpCodes.Instance.OfType(OperandType.InlineTok));

    [DataTestMethod]
    [DataRow(typeof(bool), "bool")]
    [DataRow(typeof(byte), "uint8")]
    [DataRow(typeof(char), "char")]
    [DataRow(typeof(double), "float64")]
    [DataRow(typeof(short), "int16")]
    [DataRow(typeof(int), "int32")]
    [DataRow(typeof(long), "int64")]
    [DataRow(typeof(object), "object")]
    [DataRow(typeof(sbyte), "int8")]
    [DataRow(typeof(float), "float32")]
    [DataRow(typeof(string), "string")]
    [DataRow(typeof(ushort), "uint16")]
    [DataRow(typeof(uint), "uint32")]
    [DataRow(typeof(ulong), "uint64")]
    [DataRow(typeof(IntPtr), "native int")]
    [DataRow(typeof(UIntPtr), "native uint")]
    [DataRow(typeof(TypedReference), "typedref")]
    [DataRow(typeof(void), "void")]
    [DataRow(typeof(int[,]), "int32[0...,0...]")]
    [DataRow(typeof(DateTime), "[System.Private.CoreLib]System.DateTime")]
    [DataRow(typeof(DBNull), "[System.Private.CoreLib]System.DBNull")]
    [DataRow(typeof(decimal), "[System.Private.CoreLib]System.Decimal")]
    [DataRow(typeof(Dictionary<string, int>), "[System.Private.CoreLib]System.Collections.Generic.Dictionary`2<string, int32>")]
    [DataRow(typeof(List<int>[]), "[System.Private.CoreLib]System.Collections.Generic.List`1<int32>[]")]
    [DataRow(typeof(List<int>[,]), "[System.Private.CoreLib]System.Collections.Generic.List`1<int32>[0...,0...]")]
    [DataRow(typeof(List<DateTime>), "[System.Private.CoreLib]System.Collections.Generic.List`1<valuetype [mscorlib]System.DateTime>")]
    [DataRow(typeof(List<FileInfo>), "[System.Private.CoreLib]System.Collections.Generic.List`1<class [mscorlib]System.IO.FileInfo>")]
    [DataRow(typeof(LocalEnum), "[Lyt.Reflector.Tests]Lyt.Reflector.Tests.TypeInstruction_Tests/LocalEnum")]
    [DataRow(typeof(LocalClass), "[Lyt.Reflector.Tests]Lyt.Reflector.Tests.TypeInstruction_Tests/LocalClass")]
    [DataRow(typeof(LocalClass.InnerClass), "[Lyt.Reflector.Tests]Lyt.Reflector.Tests.TypeInstruction_Tests/LocalClass/InnerClass")]
    public void InlineType_Types_TypeInstruction(Type expectedValue, string expectedText)
    {
        //TestInstruction(OpCodes.Box, expectedValue, expectedText);
        //if (expectedValue != typeof(void) && expectedValue != typeof(TypedReference) &&
        //    !expectedValue.IsArray)
        //    TestInstruction(OpCodes.Box, expectedValue.MakeArrayType(), expectedText + "[]");
    }

    private void TestInstructions(IEnumerable<OpCode> opCodes)
    {
        foreach (OpCode opCode in opCodes)
            TestInstruction(opCode, typeof(int), "int32");
    }

    private void TestInstruction(OpCode opCode, Type expectedValue, string expectedText)
    {
        MethodInfo method = CreateMethod(il =>
        {
            il.Emit(opCode, expectedValue);
        });

        Token expectedToken = GetToken(method, TokenType.TypeRef, expectedValue);
        if (expectedToken.Equals(Token.Empty))
            expectedToken = GetToken(method, TokenType.TypeSpec, expectedValue);
        Assert.AreNotEqual(Token.Empty, expectedToken);

        TestInstruction(method.GetIL(), opCode, expectedToken,
            expectedValue, $"IL_0000: {opCode.Name} {expectedText}",
            expectedType: typeof(TypeInstruction));
    }

    private Token GetToken(MethodInfo method, TokenType tokenType, Type type) =>
        GetToken(method, tokenType, type, (token, methodInfo, value) =>
            method.Module.ResolveType(token.Value,
                method.DeclaringType.GetGenericArguments(),
                method.GetGenericArguments()).Equals(type));
}
