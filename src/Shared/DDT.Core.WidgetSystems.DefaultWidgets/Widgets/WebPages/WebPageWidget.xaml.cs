using DDT.Core.WidgetSystems.Controls.Models;
using DDT.Core.WidgetSystems.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DDT.Core.WidgetSystems.Bases;
using DDT.Core.WidgetSystems.DefaultWidgets.Widgets.ToDoLists;
using CommunityToolkit.Mvvm.ComponentModel;
using DDT.Core.WidgetSystems.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Wpf;

namespace DDT.Core.WidgetSystems.DefaultWidgets.Widgets.WebPages;

public enum WebPageViewMode
{
    Mobile,
    Desktop,
}

public enum SessionScope
{
    Application,
    Project,
    Workflow,
    Widget,
}

public enum SessionPersistence
{
    Persistent,

}

public partial class WebPageOption : WidgetOptionBase
{
    [ObservableProperty]
    private string _url;

    [ObservableProperty]
    private SessionScope _sessionScope;

    [ObservableProperty]
    private SessionPersistence _sessionPersistence;

    [ObservableProperty]
    private WebPageViewMode _viewMode;

    [ObservableProperty]
    private int? autoReloadSeconds;
}

[WidgetGenerator(
    name: "Create Web Page",
    description: "Provides a one by one square widget.",
    menuPath: "Default/Web Page",
    menuOrder: 0,
    targetType: typeof(WebPageViewModel)
    )]
public partial class WebPageViewModel : WidgetViewModelBase
{
    [ObservableProperty]
    private WebPageOption? _options;

    [ObservableProperty]
    private WebView2 _webView;

    /// <summary>
    /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
    /// </summary>
    public WebPageViewModel(IServiceProvider services) : base(services)
    {
        var widgetSystemService = services.GetService<IWidgetSystemService>();
        if (!widgetSystemService.TryGetWidgetOption<WebPageOption>(Uid, out var option))
        WidgetTitle = option.Name;
        RowSpanColumnSpan = new RowSpanColumnSpan(2, 2);
    }
}

/// <summary>
/// Interaction logic for WebPageWidget.xaml
/// </summary>
public partial class WebPageWidget
{
    public WebPageWidget()
    {
        InitializeComponent();
    }
}
