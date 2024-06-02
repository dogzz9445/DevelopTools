using CommunityToolkit.Mvvm.ComponentModel;
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

namespace DDT.Apps.DDTOrganizer.Views
{
    public partial class WidgetConfigurationViewModel : ObservableObject
    {
        [ObservableProperty]
        private WidgetHost _widgetHost;

        /// <summary>
        /// Gets or sets the configuring widget.
        /// </summary>
        /// <value>The configuring widget.</value>
        [ObservableProperty]
        private UserControl? _widgetPreview;

        /// <summary>
        /// Gets or sets the configuring widget.
        /// </summary>
        /// <value>The configuring widget.</value>
        [ObservableProperty]
        private UserControl? _widgetSettings;


        public WidgetConfigurationViewModel(WidgetHost widgetHost)
        {
            _widgetHost = widgetHost;

            WidgetPreview = new UserControl
            {
                DataContext = widgetHost.DataContext,
                Content = widgetHost.DataContext,
                MaxWidth = widgetHost.MaxWidth,
                MaxHeight = widgetHost.MaxHeight,
                Width = widgetHost.ActualWidth,
                Height = widgetHost.ActualHeight
            };
        }
    }

    /// <summary>
    /// WidgetConfigurationView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WidgetConfigurationView : Page
    {
        public WidgetConfigurationView(WidgetHost widgetHost)
        {
            InitializeComponent();

            DataContext = new WidgetConfigurationViewModel(widgetHost);
        }
    }
}
