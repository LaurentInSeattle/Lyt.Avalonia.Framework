namespace Lyt.Avalonia.Localizer;

public sealed class LocalizerModel : ModelBase, ILocalizer
{
    private readonly Application application;
    private readonly FileManagerModel fileManagerModel;

    private LocalizerConfiguration configuration;

    private string? currentLanguage;
    private ResourceInclude? currentLanguageResource;

    public LocalizerModel(
        IApplicationBase application, IMessenger messenger, ILogger logger, FileManagerModel fileManagerModel) : base(messenger, logger)
    {
        if (application is not Application avaloniaApplication)
        {
            string msg = "No valid application object";
            this.Logger.Error(msg);
            throw new Exception(msg);
        }

        this.application = avaloniaApplication;
        this.fileManagerModel = fileManagerModel;
        this.configuration = new ();
    }

    public override Task Initialize() => Task.CompletedTask;

    public override Task Configure(object? modelConfiguration)
    {
        if (modelConfiguration is not LocalizerConfiguration configuration)
        {
            throw new ArgumentNullException(nameof(modelConfiguration));
        }

        if (configuration.IsLikelyValid)
        {
            this.configuration = configuration;
            this.DetectAvailableLanguages();
        }
        else
        {
            this.Logger.Fatal("Invalid configuration object");
        } 

        return Task.CompletedTask;
    }

    public bool DetectAvailableLanguages()
    {
        // Returns nothing :(   Possible bug ? 
        string uriString = this.configuration.ResourceFolderUriString();
        _ = AssetLoader.GetAssets(new Uri(uriString), null).ToList();
        return false;
    }

    public bool SelectLanguage(string targetLanguage)
    {
        if (!this.configuration.Languages.Contains(targetLanguage))
        {
            this.Logger.Error(targetLanguage + "is not a supported language.");
            return false;
        }

        var mergedDictionaries = this.application.Resources.MergedDictionaries.ToList();
        if (mergedDictionaries is null)
        {
            this.Logger.Warning("Failed get the MergedDictionaries");
            return false;
        }

        var translations =
            mergedDictionaries.OfType<ResourceInclude>()
            .FirstOrDefault(x => x.Source?.OriginalString?.Contains(this.configuration.LanguagesSubFolder) ?? false);
        if (translations is not null)
        {
            this.Logger.Info("Removed current language");
            mergedDictionaries.Remove(translations);
        }
        else
        {
            this.Logger.Warning("Failed get any Resource Includes");
        }

        try
        {
            string uriString = this.configuration.ResourceFileUriString(targetLanguage);
            var uri = new Uri(uriString);
            var newLanguage = new ResourceInclude(uri) { Source = uri };
            this.application.Resources.MergedDictionaries.Add(newLanguage);
            this.currentLanguageResource = newLanguage;
            this.currentLanguage = targetLanguage;
            this.Logger.Info("Added new language: " + targetLanguage);
            return true;
        }
        catch (Exception ex)
        {
            this.Logger.Error("Exception thrown trying to switch language\n" + ex.ToString());
            return false;
        }
    }

    public string Lookup(string localizationKey)
    {
        if (string.IsNullOrWhiteSpace(this.currentLanguage) || this.currentLanguageResource is null)
        {
            this.Logger.Warning("No language loaded");
            return string.Empty;
        }

        if (this.currentLanguageResource.TryGetResource(localizationKey, this.application.ActualThemeVariant, out object? resource))
        {
            if (resource is string localized)
            {
                return localized;
            }
        }

        this.Logger.Warning("Failed to translate: " + localizationKey + " for language: " + this.currentLanguage);
        return localizationKey;
    }

    public string LookupResource(string localizationKey)
    {
        if (string.IsNullOrWhiteSpace(this.currentLanguage) || this.currentLanguageResource is null)
        {
            this.Logger.Warning("No language loaded");
            return string.Empty;
        }

        try
        {
            string name = 
                string.Format(
                    "{0}/{1}/{1}_{2}.txt", this.configuration.LanguagesSubFolder, localizationKey, this.currentLanguage);
            string uriString = string.Format("{0}{1}", this.fileManagerModel.Configuration.AvaresUriString(), name);
            var streamReader = new StreamReader(AssetLoader.Open(new Uri(uriString)));
            string localized = 
                this.fileManagerModel.LoadResourceFromStream<string>(FileManagerModel.Kind.Text, streamReader);
            if (string.IsNullOrWhiteSpace(localized)) 
            {
                throw new Exception("No localized data"); 
            }

            return localized;
        }
        catch (Exception ex)
        {
            this.Logger.Warning("Failed to translate resource: " + localizationKey + " for language: " + this.currentLanguage);
            this.Logger.Warning("Exception thrown: \n" + ex.ToString());
            return string.Empty;
        }
    }
}
