namespace Lyt.Avalonia.Persistence;

public sealed class FileManagerModel : ModelBase, IModel
{
    public enum Area
    {
        Logs,
        Settings,
        Configuration,
        User,
        Resources,
    }

    public enum Kind
    {
        Json,
        Text,
        Binary,
    }

    private const string JsonExtension = ".json";
    private const string TextExtension = ".txt";
    private const string BinaryExtension = ".data";

    private const string LogsFolder = "Logs";
    private const string ConfigurationFolder = "Configuration";

    private static readonly string[] LogFilesFilters = ["*.log", "*.csv", "*.txt"];

    // LATER
    // private const string SettingsFolder = "Settings";
    // private static readonly string[] ApplicationFolders = [LogsFolder, SettingsFolder, ConfigurationFolder, UserFolder];

    private readonly JsonSerializerOptions jsonSerializerOptions;

    public FileManagerModel(IMessenger messenger, ILogger logger) : base(messenger, logger)
    {
        this.Configuration = new FileManagerConfiguration(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        this.jsonSerializerOptions = new JsonSerializerOptions { AllowTrailingCommas = true };
        this.jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public FileManagerConfiguration Configuration { get; private set; }

    public string ApplicationFolderPath { get; private set; } = string.Empty;

    public string ApplicationUserFolderPath { get; private set; } = string.Empty;

    public string ApplicationLogsFolderPath { get; private set; } = string.Empty;

    public string ApplicationConfigurationFolderPath { get; private set; } = string.Empty;

    public override Task Initialize() => Task.CompletedTask;

    public override Task Shutdown() => Task.CompletedTask;

    public override Task Configure(object? modelConfiguration)
    {
        if (modelConfiguration is not FileManagerConfiguration configuration)
        {
            throw new ArgumentNullException(nameof(modelConfiguration));
        }

        if (string.IsNullOrWhiteSpace(configuration.Organization) ||
            string.IsNullOrWhiteSpace(configuration.Application) ||
            string.IsNullOrWhiteSpace(configuration.RootNamespace) ||
            string.IsNullOrWhiteSpace(configuration.AssemblyName) ||
            string.IsNullOrWhiteSpace(configuration.AssetsFolder))
        {
            throw new Exception("Invalid File Manager Configuration");
        }

        this.Configuration = configuration;
        try
        {
            this.SetupEnvironment();
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex.ToString());
            throw new Exception("File Manager failed to setup environment", ex);
        }

        if (string.IsNullOrWhiteSpace(this.ApplicationFolderPath) ||
            string.IsNullOrWhiteSpace(this.ApplicationLogsFolderPath) ||
            string.IsNullOrWhiteSpace(this.ApplicationUserFolderPath))
        {
            throw new Exception("File Manager failed to setup environment");
        }

        this.CleanupOldLogs();
        return Task.CompletedTask;
    }

    private void SetupEnvironment()
    {
        string directory =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                this.Configuration.Organization);
        this.CreateFolderIfNeeded(directory);

        string subDirectory = Path.Combine(directory, this.Configuration.Application);
        this.CreateFolderIfNeeded(subDirectory);
        this.ApplicationFolderPath = subDirectory;

        string configurationFolder = Path.Combine(subDirectory, FileManagerModel.ConfigurationFolder);
        this.CreateFolderIfNeeded(configurationFolder);
        this.ApplicationConfigurationFolderPath = configurationFolder;

        string logsFolder = Path.Combine(subDirectory, FileManagerModel.LogsFolder);
        this.CreateFolderIfNeeded(logsFolder);
        this.ApplicationLogsFolderPath = logsFolder;
        this.CleanupOldLogs();

        string userDirectory =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                this.Configuration.Organization);
        this.CreateFolderIfNeeded(userDirectory);

        string userSubDirectory =
            Path.Combine(userDirectory, this.Configuration.Application);
        this.CreateFolderIfNeeded(userSubDirectory);
        this.ApplicationUserFolderPath = userSubDirectory;
    }

    public static string TimestampString()
    {
        var now = DateTime.Now.ToLocalTime();
        return
            string.Format(
                "{0:D2}_{1:D2}_{2:D2}_{3:D2}_{4:D2}_{5:D2}",
                now.Year - 2000, now.Month, now.Day,
                now.Hour, now.Minute, now.Second);
    }

    public static string ShortTimestampString()
    {
        var now = DateTime.Now.ToLocalTime();
        return string.Format("{0:D1}_{1:D2}_{2:D2}", now.Year - 2020, now.Month, now.Day);
    }

    public void CreateFolderIfNeeded(string folderName)
    {
        string _ = FileManagerModel.ValidPathName(folderName, out bool changed);
        if (changed)
        {
            string msg = "Invalid folder name: " + folderName;
            this.Logger.Error(msg);
            throw new Exception(msg);
        }

        if (!Directory.Exists(folderName))
        {
            Directory.CreateDirectory(folderName);
        }
    }

    // Use only for path names, not including file names  
    public static string ValidPathName(string fileName, out bool changed)
    {
        string validFileName = fileName;
        foreach (char c in Path.GetInvalidPathChars())
        {
            validFileName = validFileName.Replace(c, '_');
        }

        // This is causing more troubles than solutions
        // // Spaces are valid in Windows path names but often cause problems, so avoid...
        // // validFileName = validFileName.Replace(' ', '_');

        changed = validFileName != fileName;
        return validFileName;
    }

    // Use for file names, not including folder path 
    public static string ValidFileName(string fileName, out bool changed)
    {
        string validFileName = fileName;
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            validFileName = validFileName.Replace(c, '_');
        }

        // Spaces are valid in Windows file names but often cause problems, so avoid...
        validFileName = validFileName.Replace(' ', '_');
        changed = validFileName != fileName;
        return validFileName;
    }

    private void CleanupOldLogs()
    {
        // Launch a fire & forget task to cleanup old logs 
        // If we have a debugger attached, we can delete old stuff somewhat more agressively 
        TimeSpan timeSpan = Debugger.IsAttached ? TimeSpan.FromDays(7) : TimeSpan.FromDays(31);
        Task.Run(async () =>
        {
            // Wait: Don't bog down startup 
            await Task.Delay(420);
            foreach (string filter in FileManagerModel.LogFilesFilters)
            {
                await this.CleanupOldFiles(timeSpan, this.ApplicationLogsFolderPath, filter);
            }
        });
    }

    public async Task CleanupOldFiles(TimeSpan since, string path, string filter)
    {
        if (!Directory.Exists(path))
        {
            return;
        }

        // Enumerates files 
        var enumerationOptions = new EnumerationOptions()
        {
            IgnoreInaccessible = true,
            RecurseSubdirectories = true,
            MatchType = MatchType.Simple,
            MaxRecursionDepth = 8,
        };
        var files = Directory.EnumerateFiles(path, filter, enumerationOptions);

        // Wait... 
        await Task.Delay(42);

        DateTime now = DateTime.Now;
        foreach (string file in files)
        {
            TimeSpan timeSpan = now - File.GetCreationTime(file);
            if (timeSpan > since)
            {
                try
                {
                    File.Delete(file);
                    this.Logger.Info(file + " deleted.");
                }
                catch (Exception ex)
                {
                    // Log and swallow 
                    this.Logger.Error(ex.ToString());
                }
                finally
                {
                    // Wait: Don't bog down startup 
                    await Task.Delay(42);
                }
            }
        }
    }

    public string Serialize<T>(T deserialized) where T : class
    {
        try
        {
            string serialized = JsonSerializer.Serialize(deserialized, typeof(T), this.jsonSerializerOptions);
            if (!string.IsNullOrWhiteSpace(serialized))
            {
                return serialized;
            }

            throw new Exception();
        }
        catch (Exception ex)
        {
            string msg = "Failed to serialize " + typeof(T).FullName + "\n" + ex.ToString();
            this.Logger.Error(msg);
            throw new Exception(msg, ex);
        }
    }

    public T Deserialize<T>(string serialized) where T : class
    {
        try
        {
            object? deserialized = JsonSerializer.Deserialize(serialized, typeof(T), this.jsonSerializerOptions);
            if (deserialized is T deserializedOfT)
            {
                return deserializedOfT;
            }

            throw new Exception();
        }
        catch (Exception ex)
        {
            string msg = "Failed to deserialize " + typeof(T).FullName + "\n" + ex.ToString();
            this.Logger.Fatal(msg);
            throw new Exception(msg, ex);
        }
    }

    private string PathFromArea(Area area)
    {
        return area switch
        {
            Area.Logs => this.ApplicationLogsFolderPath,
            Area.Settings => this.ApplicationUserFolderPath,
            Area.User => this.ApplicationUserFolderPath,
            Area.Configuration => this.ApplicationConfigurationFolderPath,
            _ => throw new ArgumentException("Unknown area", nameof(area)),
        };
    }

    private static string ExtensionFromKind(Kind kind)
    {
        return kind switch
        {
            Kind.Json => JsonExtension,
            Kind.Text => TextExtension,
            Kind.Binary => BinaryExtension,
            _ => throw new ArgumentException("Unknown kind", nameof(kind)),
        };
    }

    public string MakePath(Area area, Kind kind, string name)
    {
        try
        {
            return Path.Combine(this.PathFromArea(area), string.Concat(name, FileManagerModel.ExtensionFromKind(kind)));
        }
        catch (Exception ex)
        {
            string msg = "Failed to build path for " + area.ToString() + " - " + name + kind.ToString();
            this.Logger.Error(msg);
            throw new Exception(msg, ex);
        }
    }

    public bool Exists(Area area, Kind kind, string name)
    {
        string path = this.MakePath(area, kind, name);
        return Path.Exists(path);
    }

    public T Load<T>(Area area, Kind kind, string name) where T : class
    {
        try
        {
            if (area == Area.Resources)
            {
                throw new NotSupportedException("Use LoadResourceFromStream<T>");
            }

            string path =
                Path.Combine(this.PathFromArea(area), string.Concat(name, FileManagerModel.ExtensionFromKind(kind)));
            switch (kind)
            {
                default:
                case Kind.Text:
                    if (typeof(T) == typeof(string))
                    {
                        string content = File.ReadAllText(path);
                        return (content as T)!;
                    }

                    throw new NotSupportedException("string type mismatch");

                case Kind.Json:
                    string serialized = File.ReadAllText(path);
                    T deserialized = this.Deserialize<T>(serialized);
                    return deserialized;

                case Kind.Binary:
                    throw new NotSupportedException("No binaries for now");
            }
        }
        catch (Exception ex)
        {
            string msg = "Failed to load " + area.ToString() + " - " + name + kind.ToString();
            this.Logger.Fatal(msg);
            throw new Exception(msg, ex);
        }
    }

    public T LoadResourceFromStream<T>(Kind kind, StreamReader streamReader) where T : class
    {
        switch (kind)
        {
            default:
            case Kind.Text:
                if (typeof(T) == typeof(string))
                {
                    string content = streamReader.ReadToEnd();
                    return (content as T)!;
                }

                throw new NotSupportedException("string type mismatch");

            case Kind.Json:
                string serialized = streamReader.ReadToEnd();
                T deserialized = this.Deserialize<T>(serialized);
                return deserialized;

            case Kind.Binary:
                throw new NotSupportedException("No binaries for now");
        }
    }

    public void Save<T>(Area area, Kind kind, string name, T content) where T : class
    {
        try
        {
            if (area == Area.Resources)
            {
                throw new NotSupportedException("Cant save resource files");
            }

            string path =
                Path.Combine(this.PathFromArea(area), string.Concat(name, FileManagerModel.ExtensionFromKind(kind)));
            switch (kind)
            {
                default:
                case Kind.Text:
                    if (content is string contentString)
                    {
                        File.WriteAllText(path, contentString);
                    }
                    else
                    {
                        throw new NotSupportedException("string type mismatch");
                    }

                    break;

                case Kind.Json:
                    string serialized = this.Serialize<T>(content);
                    File.WriteAllText(path, serialized);
                    break;

                case Kind.Binary:
                    throw new NotSupportedException("No binaries for now");
            }
        }
        catch (Exception ex)
        {
            string msg = "Failed to save for " + area.ToString() + " - " + name + kind.ToString();
            this.Logger.Fatal(msg);
            throw new Exception(msg, ex);
        }
    }
}