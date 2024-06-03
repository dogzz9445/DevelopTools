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
using DDT.Core.WidgetSystems.DefaultWidgets.Widgets.Monacos;

namespace DDT.Core.WidgetSystems.DefaultWidgets.Widgets.Notes;

[WidgetGenerator(
    name: "Create Note",
    description: "Provides a one by one square widget.",
    menuPath: "Default/Note",
    menuOrder: 0,
    targetType: typeof(NoteWidgetViewModel)
    )]
public partial class NoteWidgetViewModel : WidgetViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
    /// </summary>
    public NoteWidgetViewModel(IServiceProvider services) : base(services)
    {
        WidgetTitle = $"Commander";
        RowSpanColumnSpan = new RowSpanColumnSpan(2, 2);
    }
}

/// <summary>
/// Interaction logic for NoteWidget.xaml
/// </summary>
public partial class NoteWidget
{
    public NoteWidget()
    {
        InitializeComponent();
    }
}
