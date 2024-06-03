
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
using System.Windows.Threading;
using DDT.Core.WidgetSystems.Contracts.Services;
using DDT.Core.WidgetSystems.Controls;
using DDT.Core.WidgetSystems.Controls.Models;
using DDT.Core.WPF.Appearances;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Wpf;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DDT.Core.WidgetSystems.DefaultWidgets.Widgets.Monacos;

public partial class MonacoWidgetViewModel : WidgetViewModelBase
{
    private MonacoController? _monacoController;

    private readonly IApplicationService _applicationService;

    [ObservableProperty]
    private WebView2 _webView;

    public MonacoWidgetViewModel(IServiceProvider services) : base()
    {
        WidgetTitle = $"Monaco";
        VisibleTitle = false;
        RowSpanColumnSpan = new RowSpanColumnSpan(3, 3);

        _applicationService = services.GetService<IApplicationService>();
        WebView = new WebView2();
        WebView.Loaded += (s, e) =>
        {
            SetWebView(WebView);
        };
    }

    public void SetWebView(WebView2 webView)
    {
        webView.NavigationCompleted += OnWebViewNavigationCompleted;
        webView.SetCurrentValue(FrameworkElement.UseLayoutRoundingProperty, true);
        webView.SetCurrentValue(WebView2.DefaultBackgroundColorProperty, System.Drawing.Color.Transparent);
        webView.SetCurrentValue(
            WebView2.SourceProperty,
            new Uri(
                System.IO.Path.Combine(
                    System.AppDomain.CurrentDomain.BaseDirectory,
                    @"Assets\Monaco\index.html"
                )
            )
        );

        _monacoController = new MonacoController(_applicationService, webView);
    }

    [RelayCommand]
    public void OnMenuAction(string parameter) { }

    private async Task InitializeEditorAsync()
    {
        if (_monacoController == null)
        {
            return;
        }

        await _monacoController.CreateAsync();
        await _monacoController.SetThemeAsync(ApplicationThemeManager.GetAppTheme());
        await _monacoController.SetLanguageAsync(MonacoLanguage.Csharp);
        await _monacoController.SetContentAsync(
            "// This Source Code Form is subject to the terms of the MIT License.\r\n// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.\r\n// Copyright (C) Leszek Pomianowski and WPF UI Contributors.\r\n// All Rights Reserved.\r\n\r\nnamespace Wpf.Ui.Gallery.Models.Monaco;\r\n\r\n[Serializable]\r\npublic record MonacoTheme\r\n{\r\n    public string Base { get; init; }\r\n\r\n    public bool Inherit { get; init; }\r\n\r\n    public IDictionary<string, string> Rules { get; init; }\r\n\r\n    public IDictionary<string, string> Colors { get; init; }\r\n}\r\n"
        );
    }

    private void OnWebViewNavigationCompleted(
        object? sender,
        Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e
    )
    {
        DispatchAsync(InitializeEditorAsync);
    }

    private DispatcherOperation<TResult> DispatchAsync<TResult>(Func<TResult> callback)
    {
        return _applicationService.Dispatcher.InvokeAsync(callback);
    }
}

/// <summary>
/// Interaction logic for MonacoWidget.xaml
/// </summary>
public partial class MonacoWidget
{
    public MonacoWidget()
    {
        InitializeComponent();
    }
}
