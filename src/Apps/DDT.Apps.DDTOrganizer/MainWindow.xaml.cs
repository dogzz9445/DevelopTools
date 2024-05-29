using System.Text;
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
using DDT.Core.WidgetSystems.WPF.Components;
using MahApps.Metro.Controls;

namespace DDT.Apps.DDTOrganizer
{
    /// <summary>
    /// Represents the main viewmodel of the applications bound to the MainWindow view
    /// Implements the <see cref="ViewModelBase" />
    /// </summary>
    /// <seealso cref="ViewModelBase" />
    public partial class MainWindowViewModel : ObservableObject
    {
        #region Private Fields

        [ObservableProperty]
        private DashboardsViewModel _dashboardsContent;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            //DashboardsContent = new DashboardsViewModel();
            //DashboardsContent.Start();
        }

        #endregion Public Constructors
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}