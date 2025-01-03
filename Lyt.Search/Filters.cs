namespace Lyt.Search;

public sealed record class FilterResult<TContent>(
    bool Success, List<TContent> Result, string Message)
    where TContent : class
{
}

public sealed record class FilterString(string PropertyName, string PropertyValue);

public sealed record class FilterPredicate(string PropertyName, bool PropertyValue);

public sealed record class FilterSort(string PropertyName, bool IsAscending = true);