using DDT.Core.WPF.Utilities;
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
        ThemeHelper.ChangeTheme(Resources, "Dark");

        // Create a new MainWindow and set its DataContext to a new MainWindowViewModel which binds the view to the viewmodel
        new MainWindow { DataContext = new MainWindowViewModel() }.Show();
    }
}
