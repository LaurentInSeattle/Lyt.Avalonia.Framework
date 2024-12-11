namespace Lyt.Avalonia.LocalizationTool;

public sealed record class GeneratorParameters
(
    string NeutralLanguage,         //  "en-US",
    string[] SupportedLanguages,    //  = [ "fr-FR", "es-ES", "it-IT", "de-DE", ]; 
    string ResourcesExtension,      // = ".resx",
    string ResourcesPath,           // = "CrankyGarage.Localization\\Resources",
    bool RootIsAbsolute,
    string RootPath,      
    string SolutionFolderName,                  // = "CrankyGarageUi",
    string AvaloniaApplicationLanguageFolder,   // = "CrankyGarageApp\\Assets\\Languages",
    string AvaloniaLanguageFileFormat           // = "Lang_{0}.axaml",
);
