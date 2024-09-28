namespace Lyt.Avalonia.Localizer;

public sealed class LocalizerConfiguration
{
    public const string LanguagesFileExtension = ".axaml";

    public HashSet<string> Languages { get; set; } = []; // Example: = ["en-US", "fr-FR", "it-IT"];

    public string AssemblyName { get; set; } = string.Empty; // Example: = "TextoCopier";

    public string AssetsFolder { get; set; } = "Assets";

    public string LanguagesSubFolder { get; set; } = "Languages";

    public string LanguagesFilePrefix { get; set; } = "Lang_";

    public bool IsLikelyValid =>
        this.Languages.Count > 0 &&
        !string.IsNullOrWhiteSpace(this.AssemblyName) &&
        !string.IsNullOrWhiteSpace(this.AssetsFolder) &&
        !string.IsNullOrWhiteSpace(this.LanguagesSubFolder) &&
        !string.IsNullOrWhiteSpace(this.LanguagesFilePrefix);

    public string ResourceFileUriString(string targetLanguage) =>
            string.Format(
                "avares://{0}/{1}/{2}/{3}{4}{5}",
                this.AssemblyName, this.AssetsFolder,
                this.LanguagesSubFolder, this.LanguagesFilePrefix,
                targetLanguage, LanguagesFileExtension);

    public string ResourceFolderUriString()
        => string.Format("avares://{0}/{1}/{2}", this.AssemblyName, this.AssetsFolder, this.LanguagesSubFolder);
}
