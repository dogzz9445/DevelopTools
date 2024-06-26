﻿using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using DDT.Apps.DDTOrganizer.Controls;
using DDT.Apps.DDTOrganizer.Services;
using DDT.Apps.DDTOrganizer.Views;
using DDT.Core.WidgetSystems.Contracts.Services;
using DDT.Core.WidgetSystems.Services;
using DDT.Core.WPF.Appearances;
using DDT.Core.WPF.Extensions;
using DDT.Core.WPF.Utilities;
using MahApps.Metro.Controls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.ViewManagement.Core;
using Wpf.Ui.Appearance;
using ApplicationThemeManager = Wpf.Ui.Appearance.ApplicationThemeManager;


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
        var theme = System.Configuration.ConfigurationManager.AppSettings["Theme"];
        ThemeHelper.Register("Light", @"pack://application:,,,/DDT.Core.WidgetSystems;component/Themes/Light.xaml");
        ThemeHelper.Register("Dark", @"pack://application:,,,/DDT.Core.WidgetSystems;component/Themes/Dark.xaml");
        ThemeHelper.ChangeTheme(Resources, "Dark");
        ApplicationThemeManager.Apply(ApplicationTheme.Dark);

        Application.Current.DispatcherUnhandledException += (sender, args) =>
        {
            MessageBox.Show(args.Exception.Message, "Unhandled exception occured");
            //Logger.LogError(args.Exception, "Unhandled exception occured");
        };

        // 같은 이름의 다른 프로세스가 실행중인지 확인하고, 실행중이면 종료
        if (CheckIfProcessExists())
        {
            MessageBox.Show(
                "Another instance of the application is already running.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown();
        }

        Services = ConfigureServices(e.Args);

        // Create a new MainWindow and set its DataContext to a new MainWindowViewModel which binds the view to the viewmodel
        new MainWindow().Show();
    }

    private static IConfigurationRoot BuildConfiguration(string[] args)
    {
        // Create and build a configuration builder
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            //.AddAppSettingsJsonFileByEnvironmentVariables()
            .AddEnvironmentVariables()
            //.AddEntityConfiguration()
            .AddCommandLine(args);

        return builder.Build();
    }

    private static IServiceProvider ConfigureServices(string[] args)
    {
        var serviceCollection = new ServiceCollection();

        // Build the configuration
        var configuration = BuildConfiguration(args);
        serviceCollection.AddSingleton<IConfiguration>(configuration);

        // Register services
        serviceCollection.AddSingleton<IApplicationService, ApplicationService>();
        serviceCollection.AddSingleton<ISecretService, ModelVersionSecretService>();
        serviceCollection.AddSingleton<IAuthService, AuthService>();
        serviceCollection.AddSingleton<IWidgetService, WidgetService>();
        serviceCollection.AddSingleton<IWidgetSystemService, WidgetSystemService>();

        // Register viewmodels


        //Logger.Configure(configuration);
        //serviceCollection.AddSingleton<SettingsController>();
        //Localizer.Configure(configuration);

        return serviceCollection.BuildServiceProvider();
    }

    /// <summary>
    /// Checks if there is already an instance of Openhardwaremonitor running and brings up its window
    /// in case its minimized or as icon in taskbar
    /// </summary>
    private static bool CheckIfProcessExists()
    {
        bool processExists = false;
        Process thisInstance = Process.GetCurrentProcess();
        if (Process.GetProcessesByName(thisInstance.ProcessName).Length > 1)
        {
            processExists = true;
            using (var clientPipe = InterprocessCommunicationFactory.GetClientPipe())
            {
                clientPipe.Connect();
                clientPipe.Write(new byte[] { (byte)SecondInstanceService.SecondInstanceRequest.MaximizeWindow }, 0, 1);
            }
        }

        return processExists;
    }
}
