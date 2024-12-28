namespace Lyt.Search;

public sealed class SearchEngine<TContent> where TContent : class
{
    private readonly ICollection<TContent> source;
    private readonly ILogger logger;
    private readonly Dictionary<string, MethodInfo> stringProperties;
    private readonly Dictionary<string, MethodInfo> boolProperties;

    public SearchEngine(ICollection<TContent> source, ILogger logger)
    {
        this.source = source;
        this.logger = logger;
        this.stringProperties = [];
        this.boolProperties = [];

        this.CreateReflectionCache();
    }

    public List<TContent> All => [.. this.source];

    public FilterResult<TContent> Filter(IEnumerable<FilterPredicate> filterPredicates)
        => this.Filter([], filterPredicates);

    public FilterResult<TContent> Filter(IEnumerable<FilterString> filterStrings)
        => this.Filter(filterStrings, []);

    /// <summary> 
    /// Filter the source collection 
    /// Content must satisfy all predicates (AND) 
    /// then...  Implicit OR on all string criteria  
    /// </summary>
    public FilterResult<TContent> Filter(
        IEnumerable<FilterString> filterStrings, 
        IEnumerable<FilterPredicate> filterPredicates)
    {
        string message = string.Empty;

        try 
        {
            var list = new List<TContent>();
            if (filterPredicates.Any())
            {
                foreach (TContent content in this.source)
                {
                    bool exclude = false;
                    foreach (FilterPredicate predicate in filterPredicates)
                    {
                        if (predicate.PropertyValue != this.InvokeBoolProperty(predicate.PropertyName, content))
                        {
                            exclude = true;
                            break;
                        }
                    }

                    if (!exclude)
                    {
                        list.Add(content);
                    }
                }
            } 
            else
            {
                list = [.. this.source];
            }

            var finalList = new List<TContent>();
            if (filterStrings.Any())
            {
                foreach (TContent content in list)
                {
                    foreach (FilterString filterString in filterStrings)
                    {
                        string propertyValue = this.InvokeStringProperty(filterString.PropertyName, content);
                        if (propertyValue.Contains(
                            filterString.PropertyValue, StringComparison.InvariantCultureIgnoreCase))
                        {
                            finalList.Add(content);
                            break;
                        }
                    }
                }
            }
            else
            {
                finalList = list; 
            } 

            return new FilterResult<TContent>(Success: true, finalList, message);
        }
        catch (Exception e)
        {
            if (Debugger.IsAttached) { Debugger.Break(); }
            message = e.Message;
            this.logger.Error(e.Message);
            this.logger.Error(e.ToString());
        }

        return new FilterResult<TContent>(Success: false, [.. this.source], message);
    }

    private void CreateReflectionCache()
    {
        try
        {
            var type = typeof(TContent);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (properties is null || properties.Length == 0)
            {
                throw new Exception("Content Type has no properties");
            }

            foreach (PropertyInfo property in properties)
            {
                if (!property.CanRead)
                {
                    // Skip write only properties 
                    continue;
                }

                var getter = property.GetGetMethod();
                if (getter is null)
                {
                    // Skip if fails to retrieve the getter 
                    continue;
                }

                if (property.PropertyType == typeof(string))
                {
                    this.stringProperties.Add(property.Name, getter);
                }
                else if (property.PropertyType == typeof(bool))
                {
                    this.boolProperties.Add(property.Name, getter);
                }
                else
                {
                    // Ignore all other returning different types 
                }
            }

            if ((this.stringProperties.Count == 0) && (this.boolProperties.Count == 0))
            {
                throw new Exception("Content Type has no searchable properties");
            }
        }
        catch (Exception e)
        {
            if (Debugger.IsAttached) { Debugger.Break(); }
            this.logger.Error(e.Message);
        }
    }

    private string InvokeStringProperty(string propertyName, TContent content)
    {
        if (this.stringProperties.TryGetValue(propertyName, out var getter))
        {
            if (getter is null)
            {
                throw new Exception("Null getter for " + propertyName);
            }

            object? maybeString = getter.Invoke(content, null);
            if (maybeString is string resultString)
            {
                return resultString;
            }

            throw new Exception("Not a string property: " + propertyName);
        }

        throw new Exception("No such property " + propertyName);
    }

    private bool InvokeBoolProperty(string propertyName, TContent content)
    {
        if (this.boolProperties.TryGetValue(propertyName, out var getter))
        {
            if (getter is null)
            {
                throw new Exception("Null getter for " + propertyName);
            }

            object? maybeBool = getter.Invoke(content, null);
            if (maybeBool is bool resultBool)
            {
                return resultBool;
            }

            throw new Exception("Not a bool property: " + propertyName);
        }

        throw new Exception("No such property " + propertyName);
    }
}
