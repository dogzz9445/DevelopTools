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
using CommunityToolkit.Mvvm.ComponentModel;
using DDT.Core.WidgetSystems.Controls.Models;
using DDT.Core.WidgetSystems.Controls;
using DDT.Core.WidgetSystems.Bases;

namespace DDT.Core.WidgetSystems.DefaultWidgets.Widgets.Commanders
{
    [WidgetGenerator(
        name: "Create Commander",
        description: "Provides a one by one square widget.",
        menuPath: "Default/Commander",
        menuOrder: 0,
        targetType: typeof(CommanderWidgetViewModel)
        )]
    public partial class CommanderWidgetViewModel : WidgetViewModelBase
    {
        public static int WidgetNumber = 1;
        /// <summary>
        /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
        /// </summary>
        public CommanderWidgetViewModel(IServiceProvider services) : base(services)
        {
            WidgetTitle = $"Commander{WidgetNumber++}";
            RowSpanColumnSpan = new RowSpanColumnSpan(2, 2);
        }
    }

    /// <summary>
    /// CommanderWidget.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommanderWidget
    {
        public CommanderWidget()
        {
            InitializeComponent();
        }
    }
}
