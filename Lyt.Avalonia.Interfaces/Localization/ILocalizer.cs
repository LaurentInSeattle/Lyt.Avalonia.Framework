namespace Lyt.Avalonia.Interfaces.Localization;

public interface ILocalizer
{
    /// <summary> Configures the localizer </summary>
    Task Configure(LocalizerConfiguration localizerConfiguration); 

    /// <summary> Returns true if the requested language exists and gets selected </summary>
    bool SelectLanguage(string targetLanguage);

    /// <summary> Returns the current language, if one is selected </summary>
    string? CurrentLanguage { get; }

    /// <summary> Returns a localized string from the provided key </summary>
    /// <remarks> Returns the key if no translation can be found. </remarks>
    string Lookup(string localizationKey, bool failSilently = false);

    /// <summary> 
    /// Returns a (potentially long) localized string from the provided key, 
    /// requiring access to resources, files, database, etc... 
    /// </summary>
    string LookupResource(string localizationKey); 
}
