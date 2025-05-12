namespace Lyt.Reflector.Structures; 

public sealed class AssemblyVertex : IKeyProvider<string>
{
    public readonly AssemblyName AssemblyName;

    public AssemblyVertex(AssemblyName assemblyName)
    {
        this.AssemblyName = assemblyName;        
    }

    public string Key => this.AssemblyName.Name!;

    public Assembly? Assembly { get; set; }

    public bool IsLoaded => this.Assembly is not null;

    public bool Load()
    {
        try
        {
            this.Assembly = Assembly.Load(this.AssemblyName);
            return true;
        }
        catch (Exception ex)
        {
            this.Assembly = null;
            Debug.WriteLine(ex);
            return false;
        }
    } 
}
