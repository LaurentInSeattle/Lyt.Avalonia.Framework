namespace Lyt.Reflector.Tests;

    [TestClass]
public class MemberInstruction_Tests : InstructionHelper
{
	[TestMethod]
	public void InlineTok_MemberInstruction()
	{
		foreach (OpCode opCode in AllOpCodes.Instance.OfType(OperandType.InlineTok))
		{
			// This is super-hacky...we depend on the default object constructor being
			// the first member reference.  I would have preferred to create our own
			// member reference.  Regrettably, despite trying, I couldn't find a way to
			// do this reliably.
			string expectedText = "method instance void object::.ctor()";
			Token expectedToken = new Token(TokenType.MemberRef, 1);

			MethodInfo method = CreateMethod(il => il.Emit(opCode, expectedToken.Value));

			var member = method.Module.ResolveMember(expectedToken.Value) as MethodBase;
			Assert.IsNotNull(member);

			TestInstruction(method.GetIL(), opCode, expectedToken,
				member, $"IL_0000: {opCode.Name} {expectedText}",
				expectedType: typeof(MethodInstruction));
		}
	}
}
