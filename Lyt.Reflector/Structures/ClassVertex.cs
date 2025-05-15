namespace Lyt.Reflector.Structures;

public sealed class ClassVertex : IKeyProvider<string>
{
    private readonly AssemblyVertex assemblyVertex;
    private readonly Type classType;

    public ClassVertex(AssemblyVertex assemblyVertex, Type type)
    {
        this.assemblyVertex = assemblyVertex;
        this.classType = type;

        // Remove that later when everything is peachy 
        if (type.Name.StartsWith('<'))
        {
            // All computer generated classes should have been filtered out 
            Debugger.Break();
        }

        this.Key = type.SafeFullName();
        this.EventDescriptors = new(8);
        this.FieldDescriptors = new(8);
        this.PropertyDescriptors = new(16);
        this.MethodDescriptors = new(16);

        if (type.IsGenericType)
        {
            // Debug.WriteLine("Generic: " + type.Name);
            // FullName is null for generics so we figure our way to provide a key for generics
            this.Key = string.Concat(assemblyVertex.Key + "." + type.Name);
        }
        else
        {
            if (type.FullName is null)
            {
                Debugger.Break();
            }
        }
    }

    public Type ClassType => this.classType;

    public string Key { get; private set; }
    public List<EventDescriptor> EventDescriptors { get; private set; }
    public List<FieldDescriptor> FieldDescriptors { get; private set; }
    public List<PropertyDescriptor> PropertyDescriptors { get; private set; }
    public List<MethodDescriptor> MethodDescriptors { get; private set; }

    public void Load() => this.Load(this.classType);

    private void Load(Type type)
    {
        Debug.WriteLine("Class: " + this.Key);
        Debug.Indent();
        this.LoadEvents(type);
        this.LoadFields(type);
        this.LoadMethods(type);
        this.LoadProperties(type);
        this.LoadNestedTypes(type);
        Debug.Unindent();
    }

    private void LoadEvents(Type type)
    {
        void Load(BindingFlags bindingFlag, string debugString)
        {
            EventInfo[] eventInfos = type.GetEvents(bindingFlag | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var eventInfo in eventInfos)
            {
                string eventName = eventInfo.Name;
                this.EventDescriptors.Add(new EventDescriptor(IsStatic: true, Name: eventName));
                Debug.WriteLine(debugString + " Event: " + eventName);
            }

        }

        Load(BindingFlags.Static, "Static");
        Load(BindingFlags.Instance, "Instance");
    }

    private void LoadFields(Type type)
    {
        void Load(BindingFlags bindingFlag, string debugString)
        {
            FieldInfo[] fieldInfos = type.GetFields(bindingFlag | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var fieldInfo in fieldInfos)
            {
                string fieldName = fieldInfo.Name;
                Type fieldType = fieldInfo.FieldType;
                (bool relevantType , List<Type> dependantTypes) = fieldType.AnalyseType();
                if (relevantType)
                {
                    FieldDescriptor fieldDescriptor = new(IsStatic: true, fieldType, dependantTypes, fieldName);
                    this.FieldDescriptors.Add(fieldDescriptor);
                    Debug.WriteLine(debugString + " Field: " + fieldName + "   Type: " + fieldType.ToString());
                    if (dependantTypes.Count > 0)
                    {
                        Debug.Indent();
                        foreach (var dependantType in dependantTypes)
                        {
                            Debug.WriteLine("Dependant Type: " + dependantType.ToString());
                        }
                        Debug.Unindent();
                    }
                }
            }
        }

        Load(BindingFlags.Static, "Static");
        Load(BindingFlags.Instance, "Instance");
    }

    private void LoadProperties(Type type)
    {
        void Load(BindingFlags bindingFlag, string debugString)
        {
            PropertyInfo[] propertyInfos = 
                type.GetProperties(bindingFlag | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                string propertyName = propertyInfo.Name;
                Type propertyType = propertyInfo.PropertyType;
                (bool relevantType, List<Type> dependantTypes) = propertyType.AnalyseType();
                if (relevantType)
                {
                    PropertyDescriptor propertyDescriptor = 
                        new(IsStatic: true, propertyType, dependantTypes, propertyName);
                    this.PropertyDescriptors.Add(propertyDescriptor);
                    Debug.WriteLine(debugString + " Property: " + propertyName + "   Type: " + propertyType.ToString());
                    if (dependantTypes.Count > 0)
                    {
                        Debug.Indent();
                        foreach (var dependantType in dependantTypes)
                        {
                            Debug.WriteLine("Dependant Type: " + dependantType.ToString());
                        }
                        Debug.Unindent();
                    }
                }
            }
        }

        Load(BindingFlags.Static, "Static");
        Load(BindingFlags.Instance, "Instance");
    }

    private void LoadMethods(Type type)
    {
        void Load(BindingFlags bindingFlag, string debugString)
        {
            MethodInfo[] methodInfos =
                type.GetMethods(bindingFlag | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (MethodInfo methodInfo in methodInfos)
            {
                string methodName = methodInfo.Name;
                Type returnType = methodInfo.ReturnType;
                if ( returnType != typeof(void))
                {

                }

                ParameterInfo [] parameterInfos = methodInfo.GetParameters(); 
                if ( parameterInfos.Length > 0)
                {

                }

                //Type propertyType = propertyInfo.PropertyType;
                //(bool relevantType, List<Type> dependantTypes) = propertyType.AnalyseType();
                //if (relevantType)
                //{
                //    PropertyDescriptor propertyDescriptor =
                //        new(IsStatic: true, propertyType, dependantTypes, methodName);
                //    this.PropertyDescriptors.Add(propertyDescriptor);
                //    Debug.WriteLine(debugString + " Method: " + methodName + "   Type: " + propertyType.ToString());
                //    if (dependantTypes.Count > 0)
                //    {
                //        Debug.Indent();
                //        foreach (var dependantType in dependantTypes)
                //        {
                //            Debug.WriteLine("Dependant Type: " + dependantType.ToString());
                //        }
                //        Debug.Unindent();
                //    }
                //}
            }

        }

        Load(BindingFlags.Static, "Static");
        Load(BindingFlags.Instance, "Instance");
    }

    private void LoadNestedTypes(Type type)
    {
    }
}
