namespace Lyt.Reflector.IL;

/// <summary> Extension methods for the <see cref="MethodBase"/> class. </summary>
public static class MethodBaseExtensions
{
	/// <summary> Get the Intermediate Language (IL) for this method's body. </summary>
	/// <param name="method">The <see cref="MethodBase"/> that is extended by this method.</param>
	/// <returns>The Intermediate Language (IL) for this method's body.</returns>
	public static MethodIL GetIL(this MethodBase method) => new (method);
}
