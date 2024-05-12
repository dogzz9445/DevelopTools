using System.Configuration;
using System.Data;
using System.Windows;

namespace DDT.Apps.DDTOrganizer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Gets the current <see cref="App"/>  instance of the application
    /// </summary>
    public new static App Current => (App)Application.Current;

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance of the application
    /// </summary>
    public IServiceProvider? Services { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Set the theme
        var theme = ConfigurationManager.AppSettings["Theme"];
        if (!string.IsNullOrEmpty(theme))
        {
            var themeUri = new Uri($"/Themes/{theme}.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(new ResourceDictionary { Source = themeUri });
        }
    }
}
