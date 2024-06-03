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
using DDT.Core.WidgetSystems.DefaultWidgets.Widgets.WebPages;

namespace DDT.Core.WidgetSystems.DefaultWidgets.Widgets.WebQueries;

[WidgetGenerator(
    name: "Create Web Query",
    description: "Provides a one by one square widget.",
    menuPath: "Default/Web Query",
    menuOrder: 0,
    targetType: typeof(WebQueryViewModel)
    )]
public partial class WebQueryViewModel : WidgetViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
    /// </summary>
    public WebQueryViewModel(IServiceProvider services) : base(services)
    {
        WidgetTitle = $"ToDoList";
        RowSpanColumnSpan = new RowSpanColumnSpan(2, 2);
    }
}

/// <summary>
/// Interaction logic for WebQueryWidget.xaml
/// </summary>
public partial class WebQueryWidget
{
    public WebQueryWidget()
    {
        InitializeComponent();
    }
}
