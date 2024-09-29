using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using static Lyt.Avalonia.Themes.StyleManager;

namespace Lyt.Avalonia.Themes;

public sealed class StyleManager
{
    public const string SelfUriString = "resm:Styles?assembly=Lyt.Avalonia.Themes";

    public enum Theme { Citrus, Sea, Rust, Candy, Magma }

    // Default 
    private readonly StyleInclude rustStyle;
    private readonly StyleInclude magmaStyle;
    private readonly StyleInclude candyStyle;
    private readonly StyleInclude citrusStyle;
    private readonly StyleInclude seaStyle;

    private readonly Window window;

    public StyleManager(Window window)
    {
        this.window = window;

        // TODO ~ Consider: Create styles on demand 
        this.rustStyle = StyleManager.CreateStyle("avares://Lyt.Avalonia.Themes/Rust.xaml");
        this.magmaStyle = StyleManager.CreateStyle("avares://Lyt.Avalonia.Themes/Magma.xaml");
        this.candyStyle = StyleManager.CreateStyle("avares://Lyt.Avalonia.Themes/Candy.xaml");
        this.citrusStyle = StyleManager.CreateStyle("avares://Lyt.Avalonia.Themes/Citrus.xaml");
        this.seaStyle = StyleManager.CreateStyle("avares://Lyt.Avalonia.Themes/Sea.xaml");

        // We add the style to the window styles section, so it will override the default style defined in App.xaml. 
        if (window.Styles.Count == 0)
        {
            window.Styles.Add(this.rustStyle);
        }
        else
        {
            // If there are styles defined already, we assume that the first style imported it related to default.
            // This allows one to override citrus styles.
            window.Styles[0] = this.rustStyle;
        }

        this.CurrentTheme = Theme.Rust;
    }

    public Theme CurrentTheme { get; private set; }

    public void Use(Theme theme)
    {
        // Here, we change the first style in the main window styles
        // section, and the main window instantly refreshes. Remember
        // to invoke such methods from the UI thread.
        this.window.Styles[0] = theme switch
        {
            Theme.Citrus => this.citrusStyle,
            Theme.Sea => this.seaStyle,
            Theme.Rust => this.rustStyle,
            Theme.Candy => this.candyStyle,
            Theme.Magma => this.magmaStyle,
            _ => throw new ArgumentOutOfRangeException(nameof(theme))
        };

        this.CurrentTheme = theme;
    }

    private static StyleInclude CreateStyle(string url)
        => new(new Uri(StyleManager.SelfUriString)) { Source = new Uri(url) };
}
