using System.Text.Json.Serialization;

namespace Lyt.Avalonia.Mvvm.Extensions;

public static class GeneralExtensions
{
    public static bool Implements<TInterface>(this Type type)
        => typeof(TInterface).IsAssignableFrom(type);

    public static Action<object>? CastToActionObject<T>(this Action<T> actionOfT)
    {
        if (actionOfT == null)
        {
            return null;
        }

        return new Action<object>((o) => actionOfT((T)o));
    }

    public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
    {
        if (sequence == null)
        {
            return;
        }

        foreach (var item in sequence)
        {
            if (item != null)
            {
                action(item);
            }
        }
    }

    public static bool IsOutOfBounds<T>(this int index, ICollection<T> collection)
        => (index < 0) || (index >= collection.Count);

    public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        => null == collection || 0 == collection.Count;

    public static bool ToBool(this bool? ternary) => ternary ?? false;

    public static bool IsNullOrEmpty(this Guid? id) => null == id || Guid.Empty == id;
}
