namespace Lyt.Avalonia.LocalizationTool;

public class AvaloniaLocGenerator
{
    // TODO : BEGIN 
    // 
    // All those strings (up to TODO : END ) should be parameters for the generator 

    //public const string NeutralLanguage = "en-US";
    //public string[] SupportedLanguages =
    //[
    //    "fr-FR",
    //    "es-ES",
    //    "it-IT",
    //    "de-DE",
    //];

    //public const string ResourcesExtension = ".resx";
    //public const string ResourcesPath = "CrankyGarage.Localization\\Resources";
    //public const string SolutionFolderName = "CrankyGarageUi";
    //public const string AvaloniaApplicationLanguageFolder = "CrankyGarageApp\\Assets\\Languages";
    //public const string AvaloniaLanguageFileFormat = "Lang_{0}.axaml";

    // TODO : END  

    private const string AvaloniaHeader =
    @"<ResourceDictionary 
    xmlns=""https://github.com/avaloniaui""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:system=""clr-namespace:System;assembly=System.Runtime""
    >";

    private const string AvaloniaFooter =
        @"
</ResourceDictionary>";

    private const string AvaloniaEntryFormat =
        @"    <system:String x:Key=""{0}"">{1}</system:String>";

    private readonly Assembly localizationAssembly;
    private readonly GeneratorParameters generatorParameters;
    private readonly string rootPath;

    private List<ResourceDescriptor> localizationResources = [];

    public AvaloniaLocGenerator(GeneratorParameters generatorParameters)
    {
        this.generatorParameters = generatorParameters;
        this.localizationAssembly = Assembly.GetExecutingAssembly();
        Debug.WriteLine("Running at: " + this.localizationAssembly.Location);

        if (this.generatorParameters.RootIsAbsolute)
        {
            this.rootPath = this.generatorParameters.RootPath;
        }
        else
        {
            this.rootPath = this.SetupRoot(this.localizationAssembly.Location);
        }
    }

    public void Run()
    {
        this.localizationResources = this.EnumerateResources();
        if (this.localizationResources.Count < 1 + this.generatorParameters.SupportedLanguages.Length)
        {
            throw new Exception("Missing localization files");
        }

        var allLanguages = new List<string>() { this.generatorParameters.NeutralLanguage } ;
        allLanguages.AddRange(this.generatorParameters.SupportedLanguages);
        foreach (string language in allLanguages)
        {
            this.ProcessLanguage(language);
        } 
    }

    private void ProcessLanguage(string language)
    {
        var resources = 
            ( from res in this.localizationResources 
              where res.Language == language 
              select res).ToList();
        List<Dictionary<string, string>> dictionaries = [];
        foreach (var resource in resources)
        {
            dictionaries.Add(LoadResx(resource.FullPath, resource.Folder));
        }

        string content = CreateAvaloniaResourceFileContent(language, dictionaries);
        this.SaveAvaloniaResourceFile(language, content);
    }

    private void SaveAvaloniaResourceFile(string language, string content)
    {
        try
        {
            string fileName = 
                string.Format(this.generatorParameters.AvaloniaLanguageFileFormat, language);
            string avaloniaAppLanguageFolderPath = 
                Path.Combine(this.rootPath, this.generatorParameters.AvaloniaApplicationLanguageFolder);
            if (Directory.Exists(avaloniaAppLanguageFolderPath))
            {
                string path = Path.Combine(avaloniaAppLanguageFolderPath, fileName);
                File.WriteAllText(path, content);
            }
            else
            {
                Debug.WriteLine("Missing Asset Folder");
                if (Debugger.IsAttached) { Debugger.Break(); }
                throw new Exception("Missing Asset Folder");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            if (Debugger.IsAttached) { Debugger.Break(); }
            throw;
        }
    }

    private static string CreateAvaloniaResourceFileContent(string language, List<Dictionary<string, string>> dictionaries)
    {
        var stringBuilder = new StringBuilder(2048);
        stringBuilder.AppendLine(AvaloniaHeader);
        stringBuilder.AppendLine("");
        stringBuilder.AppendLine("    <!-- " + language + " -->");
        foreach (var dictionary in dictionaries)
        {
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("    <!-- O_O -->");
            foreach (var kvp in dictionary)
            {
                stringBuilder.AppendLine(string.Format(AvaloniaEntryFormat, kvp.Key, kvp.Value));
            }
        }

        stringBuilder.AppendLine(AvaloniaFooter);
        return stringBuilder.ToString();   
    }

    private string SetupRoot(string exePath)
    {
        int index = exePath.IndexOf(this.generatorParameters.SolutionFolderName);
        if (index == -1)
        {
            throw new Exception("Missing Solution Folder Name");
        }

        int length = index + this.generatorParameters.SolutionFolderName.Length;
        string rootPath = exePath[..length];
        Debug.WriteLine("Root Path: " + rootPath);
        return rootPath;
    }

    private List<ResourceDescriptor> EnumerateResources()
    {
        try
        {
            string documentFolder = Path.Combine(this.rootPath, this.generatorParameters.ResourcesPath);
            if (Directory.Exists(documentFolder))
            {
                // Enumerates files 
                var enumerationOptions = new EnumerationOptions()
                {
                    IgnoreInaccessible = true,
                    RecurseSubdirectories = true,
                    MatchType = MatchType.Simple,
                    MaxRecursionDepth = 8,
                };
                string extension = "*" + this.generatorParameters.ResourcesExtension;
                var files =
                    Directory.EnumerateFiles(documentFolder, extension, enumerationOptions);
                var list = new List<ResourceDescriptor>(32);
                foreach (string file in files)
                {
                    // Drop full path
                    var fileInfo = new FileInfo(file);
                    string folder = Folder(documentFolder, file, fileInfo.Name);
                    string language = this.Language(fileInfo.Name);
                    list.Add(new ResourceDescriptor(file, folder, language));
                }

                return list;
            }
            else
            {
                Debug.WriteLine("Missing Resource Folder");
                if (Debugger.IsAttached) { Debugger.Break(); }
                throw new Exception("Missing Resource Folder");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            if (Debugger.IsAttached) { Debugger.Break(); }
            throw;
        }
    }

    private static string Folder(string documentFolder, string path, string fileName)
    {
        string folder = path.Replace(documentFolder, string.Empty);
        folder = folder.Replace(fileName, string.Empty);
        folder = folder.Trim('\\');
        return folder;
    }

    private string Language(string fileName)
    {
        string[] tokens = fileName.Split('.');
        if (tokens.Length == 2)
        {
            if (tokens[0] == this.generatorParameters.NeutralLanguage)
            {
                return this.generatorParameters.NeutralLanguage;
            }
        }
        else if (tokens.Length == 3)
        {
            if (tokens[0] == this.generatorParameters.NeutralLanguage)
            {
                string language = tokens[1];
                if (this.generatorParameters.SupportedLanguages.Contains(language))
                {
                    return language;
                }
            }
        }

        string msg = "Invalid file name";
        Debug.WriteLine(msg);
        if (Debugger.IsAttached) { Debugger.Break(); }
        throw new Exception(msg);
    }

    private static Dictionary<string, string> LoadResx(string path, string folder)
    {
        try
        {
            Dictionary<string, string> dictionary = [];
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            if ((xmlDoc is null) ||
                (xmlDoc.DocumentElement is null) ||
                (xmlDoc.DocumentElement.ChildNodes is null) ||
                (xmlDoc.DocumentElement.ChildNodes.Count == 0))
            {
                throw new Exception("Invalid XML");
            }

            foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
            {
                if ((node.Name == "data") && (node.Attributes is XmlAttributeCollection xmlAttributes))
                {
                    var attribute = xmlAttributes["name"];
                    if (attribute is not null)
                    {
                        string key = attribute.InnerText;
                        key = string.Concat(folder, ".", key);
                        string inner = node.InnerText;
                        string text = inner.Trim(['\r', '\n', ' ']);

                        Debug.WriteLine(key + ":  " + text);
                        dictionary.Add(key, text);
                    }
                }
            }

            return dictionary;
        }
        catch (Exception ex)
        {
            string msg = ex.ToString();
            Debug.WriteLine(msg);
            if (Debugger.IsAttached) { Debugger.Break(); }
            throw;
        }
    }

    private sealed record class ResourceDescriptor(string FullPath, string Folder, string Language);
}
