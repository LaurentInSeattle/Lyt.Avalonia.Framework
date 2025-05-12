namespace Lyt.Reflector.Structures; 

public interface IKeyProvider<TKey> where TKey : IEquatable<TKey> 
{
    public TKey Key { get; }
}
