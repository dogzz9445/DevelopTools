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
using DDT.Core.WidgetSystems.WPF.Controls.Models;
using DDT.Core.WidgetSystems.WPF.Controls;
using UserControl = System.Windows.Controls.UserControl;

namespace DDT.Core.WidgetSystems.WPF.Widgets.Commanders
{
    public partial class CommanderWidgetViewModel : WidgetViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
        /// </summary>
        public CommanderWidgetViewModel(int widgetNumber) : base()
        {
            Title = $"Commander{widgetNumber}";
            RowSpanColumnSpan = new RowSpanColumnSpan(6, 6);
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
