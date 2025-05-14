using System.ComponentModel.Design;
using System.Reflection;
using System.Reflection.Metadata;

namespace Lyt.Reflector.Structures; 

public sealed class ClassVertex : IKeyProvider<string>
{
    private readonly AssemblyVertex assemblyVertex;
    private readonly Type classType;

    public ClassVertex(AssemblyVertex assemblyVertex, Type type)
    {
        this.assemblyVertex = assemblyVertex;
        this.classType = type;

        if (type.Name.StartsWith('<'))
        {
            // All computer generated classes should have been filtered out 
            Debugger.Break();
        }

        this.Key = type.SafeFullName();
        this.EventDescriptors = new(8);
        this.FieldDescriptors = new(8);

        if (type.IsGenericType)
        {
            // Debug.WriteLine("Generic: " + type.Name);
            // FullName is null for generics so we figure our way to provide a key for generics
            this.Key = string.Concat( assemblyVertex.Key + "." + type.Name) ; 
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
                if (fieldType.IsPrimitive)
                {
                    // The primitive types are Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr,
                    // Char, Double, and Single.
                    // If the current Type represents a generic type, or a type parameter in the definition of a generic type
                    // or generic method, this IsPrimitive always returns false.
                    // Primitive types do not create dependencies, we ignore them 
                    continue;
                }

                if (fieldType.IsGenericType)
                {
                    // TODO: Need to check generic types 
                }
                else
                {
                    if (fieldType.HasExcludedNamespace())
                    {
                        // Dependency to an ignore assembly: we can ignore 
                        continue;
                    }
                } 
                

                FieldDescriptor fieldDescriptor = new(IsStatic: true, fieldType, fieldName);
                this.FieldDescriptors.Add(fieldDescriptor);
                Debug.WriteLine( debugString + " Field: " + fieldName + "   Type: " + fieldType.ToString());
            }
        } 

        Load(BindingFlags.Static, "Static");
        Load(BindingFlags.Instance, "Instance");
    }

    private void LoadMethods(Type type)
    {
        void Load(BindingFlags bindingFlag, string debugString)
        {
        } 
        
        Load(BindingFlags.Static, "Static");
        Load(BindingFlags.Instance, "Instance");
    }

    private void LoadProperties(Type type)
    {
        void Load(BindingFlags bindingFlag, string debugString)
        {
        }

        Load(BindingFlags.Static, "Static");
        Load(BindingFlags.Instance, "Instance");
    }

    private void LoadNestedTypes(Type type)
    {
    }
}
