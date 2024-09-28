namespace Lyt.Avalonia.Persistence;

public sealed class FileManagerConfiguration(
    string organization, string application, string rootNamespace, string assemblyName, string assetsFolder)
{
    public string Organization { get; private set; } = organization;

    public string Application { get; private set; } = application;

    public string RootNamespace { get; private set; } = rootNamespace;

    public string AssemblyName { get; set; } = assemblyName; // Example: = "TextoCopier";

    public string AssetsFolder { get; set; } = assetsFolder; // Example: = "Assets";

    public string AvaresUriString() => string.Format("avares://{0}/{1}/", this.AssemblyName, this.AssetsFolder);

}
