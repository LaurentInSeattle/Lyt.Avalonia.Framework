namespace Lyt.Reflector.IL; 

public static  class MethodExtensions
{
    /// <summary> Returns the Intermediate Language (IL) for this method's body. </summary>
    public static MethodInstructionsList GetIL(this MethodBase method) => new(method);
}
