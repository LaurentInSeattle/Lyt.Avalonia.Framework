namespace Lyt.Avalonia.Interfaces.Localization;

public interface ILocalizer
{
    /// <summary> Returns a localized string from the provided key </summary>
    string Lookup(string localizationKey);

    /// <summary> 
    /// Returns a (potentially long) localized string from the provided key, 
    /// requiring access to resources, files, database, etc... 
    /// </summary>
    string LookupResource(string localizationKey); 
}
