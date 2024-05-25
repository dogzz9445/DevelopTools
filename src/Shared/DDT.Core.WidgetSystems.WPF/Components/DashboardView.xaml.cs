using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DDT.Core.WidgetSystems.WPF.Bases;
using DDT.Core.WidgetSystems.WPF.Controls;
using DDT.Core.WidgetSystems.WPF.Widgets.FileOpeners;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DDT.Core.WidgetSystems.WPF.Components
{
    /// <summary>
    /// Represents the type of configuration for a Dashboard. New meaning a new one is being generated, and
    /// existing when the dashboard already exists
    /// </summary>
    public enum DashboardConfigurationType
    {
        /// <summary>
        /// New dashboard being generated
        /// </summary>
        New,

        /// <summary>
        /// Existing dashboard being configured
        /// </summary>
        Existing
    }
    /// <summary>
    /// Represents a dashboard model containing widgets and a title
    /// Implements the <see cref="Infrastructure.ViewModelBase" />
    /// </summary>
    /// <seealso cref="Infrastructure.ViewModelBase" />
    public partial class DashboardModel : ObservableObject
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [ObservableProperty]
        private string _title;

        /// <summary>
        /// Gets or sets the widgets.
        /// </summary>
        /// <value>The widgets.</value>
        [ObservableProperty]
        private ObservableCollection<WidgetHostViewModel> _widgets;

        public DashboardModel()
        {
            Widgets = new ObservableCollection<WidgetHostViewModel>();
        }

        #endregion Public Properties
    }

    /// <summary>
    /// Provides properties detailing the validity of a dashboard name
    /// </summary>
    public class DashboardNameValidResponse
    {
        #region Public Properties

        /// <summary>
        /// Gets the invalid reason.
        /// </summary>
        /// <value>The invalid reason.</value>
        public string InvalidReason { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="DashboardNameValidResponse"/> is valid.
        /// </summary>
        /// <value><c>true</c> if valid; otherwise, <c>false</c>.</value>
        public bool Valid { get; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardNameValidResponse"/> class.
        /// </summary>
        /// <param name="valid">if set to <c>true</c> [valid].</param>
        /// <param name="invalidReason">The invalid reason.</param>
        public DashboardNameValidResponse(bool valid, string invalidReason = null)
        {
            Valid = valid;
            InvalidReason = invalidReason;
        }

        #endregion Public Constructors
    }

    /// <summary>
    /// Interface IDashboardConfigurationHandler
    /// </summary>
    public interface IDashboardConfigurationHandler
    {
        #region Public Methods

        /// <summary>
        /// Complete the dashboard configuration.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="save">if set to <c>true</c> [save].</param>
        /// <param name="newName">The new name.</param>
        void DashboardConfigurationComplete(DashboardConfigurationType type, bool save, string newName);

        /// <summary>
        /// Validate dashboard name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>DashboardNameValidResponse.</returns>
        DashboardNameValidResponse DashboardNameValid(string name);

        #endregion Public Methods
    }
    /// <summary>
    /// Model representation of a widget
    /// </summary>
    public class Widget
    {
        #region Private Fields

        private readonly Func<WidgetHostViewModel> _createWidget;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Widget"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="createWidget">The create widget.</param>
        public Widget(string name, string description, Func<WidgetHostViewModel> createWidget)
        {
            Name = name;
            Description = description;
            _createWidget = createWidget;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Creates the widget.
        /// </summary>
        /// <returns>WidgetBase.</returns>
        public WidgetHostViewModel CreateWidget()
        {
            return _createWidget.Invoke();
        }

        #endregion Public Methods
    }

    /// <summary>
    /// View model for dashboards
    /// Implements the <see cref="Infrastructure.ViewModelBase" />
    /// Implements the <see cref="WpfDashboardControl.Dashboards.IDashboardConfigurationHandler" />
    /// </summary>
    /// <seealso cref="Infrastructure.ViewModelBase" />
    /// <seealso cref="WpfDashboardControl.Dashboards.IDashboardConfigurationHandler" />
    public partial class DashboardsViewModel : ObservableObject, IDashboardConfigurationHandler
    {
        #region Private Fields

        //private DashboardSettingsPromptViewModel _configuringDashboard;
        private DashboardModel _selectedDashboard;
        private int _widgetNumber;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the available widgets.
        /// </summary>
        /// <value>The available widgets.</value>
        [ObservableProperty]
        private List<Widget> _availableWidgets = new List<Widget>();

        /// <summary>
        /// Gets the command add widget.
        /// </summary>
        /// <value>The command add widget.</value>
        public ICommand CommandAddWidget => new RelayCommand<Widget>(o =>
        {
            var widgetToAdd = (Widget)o;

            SelectedDashboard.Widgets.Add(widgetToAdd.CreateWidget());
            EditMode = true;
        });

        /// <summary>
        /// Gets the command configure widget.
        /// </summary>
        /// <value>The command configure widget.</value>
        public ICommand CommandConfigureWidget => new RelayCommand<WidgetHost>(o =>
        {
            var widgetHost = (WidgetHost)o;
            ConfiguringWidget = new TempWidgetHost
            {
                DataContext = widgetHost.DataContext,
                Content = widgetHost.DataContext,
                MaxWidth = widgetHost.MaxWidth,
                MaxHeight = widgetHost.MaxHeight,
                Width = widgetHost.ActualWidth,
                Height = widgetHost.ActualHeight
            };
        });

        /// <summary>
        /// Gets the command done configuring widget.
        /// </summary>
        /// <value>The command done configuring widget.</value>
        public ICommand CommandDoneConfiguringWidget => new RelayCommand(() => ConfiguringWidget = null);

        /// <summary>
        /// Gets the command edit dashboard.
        /// </summary>
        /// <value>The command edit dashboard.</value>
        public ICommand CommandEditDashboard => new RelayCommand<string>(o => EditMode = o.ToString() == "True", o => ConfiguringWidget == null);

        ///// <summary>
        ///// Gets the command manage dashboard.
        ///// </summary>
        ///// <value>The command manage dashboard.</value>
        //public ICommand CommandManageDashboard => new RelayCommand(() =>
        //    ConfiguringDashboard =
        //        new DashboardSettingsPromptViewModel(DashboardConfigurationType.Existing, this,
        //            SelectedDashboard.Title));

        ///// <summary>
        ///// Gets the command new dashboard.
        ///// </summary>
        ///// <value>The command new dashboard.</value>
        //public ICommand CommandNewDashboard => new RelayCommand(() =>
        //    ConfiguringDashboard = new DashboardSettingsPromptViewModel(DashboardConfigurationType.New, this));

        /// <summary>
        /// Gets the command remove widget.
        /// </summary>
        /// <value>The command remove widget.</value>
        public ICommand CommandRemoveWidget => new RelayCommand<WidgetHostViewModel>(o => SelectedDashboard.Widgets.Remove((WidgetHostViewModel)o));

        ///// <summary>
        ///// Gets or sets the configuring dashboard.
        ///// </summary>
        ///// <value>The configuring dashboard.</value>
        //public DashboardSettingsPromptViewModel ConfiguringDashboard
        //{
        //    get => _configuringDashboard;
        //    set => RaiseAndSetIfChanged(ref _configuringDashboard, value);
        //}

        /// <summary>
        /// Gets or sets the configuring widget.
        /// </summary>
        /// <value>The configuring widget.</value>
        [ObservableProperty]
        private TempWidgetHost? _configuringWidget;

        /// <summary>
        /// Gets or sets the dashboards.
        /// </summary>
        /// <value>The dashboards.</value>
        [ObservableProperty]
        private ObservableCollection<DashboardModel>? _dashboards;

        /// <summary>
        /// Gets or sets a value indicating whether [dashboard selector uncheck].
        /// </summary>
        /// <value><c>true</c> if [dashboard selector uncheck]; otherwise, <c>false</c>.</value>
        [ObservableProperty]
        private bool? _dashboardSelectorUncheck;

        /// <summary>
        /// Gets or sets a value indicating whether [edit mode].
        /// </summary>
        /// <value><c>true</c> if [edit mode]; otherwise, <c>false</c>.</value>
        [ObservableProperty]
        private bool? _editMode;

        /// <summary>
        /// Gets or sets the selected dashboard.
        /// </summary>
        /// <value>The selected dashboard.</value>
        public DashboardModel SelectedDashboard
        {
            get => _selectedDashboard;
            set
            {
                if (!SetProperty(ref _selectedDashboard, value))
                    return;

                DashboardSelectorUncheck = true;
                DashboardSelectorUncheck = false;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Completes dashboard configuration
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="save">if set to <c>true</c> [save].</param>
        /// <param name="newName">The new name.</param>
        public void DashboardConfigurationComplete(DashboardConfigurationType type, bool save, string newName)
        {
            //ConfiguringDashboard = null;

            if (!save)
                return;

            switch (type)
            {
                case DashboardConfigurationType.New:
                    var dashboardModel = new DashboardModel { Title = newName };
                    Dashboards.Add(dashboardModel);
                    SelectedDashboard = dashboardModel;
                    return;
                case DashboardConfigurationType.Existing:
                    SelectedDashboard.Title = newName;
                    return;
            }
        }

        /// <summary>
        /// Dashboards the name valid.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>DashboardNameValidResponse.</returns>
        public DashboardNameValidResponse DashboardNameValid(string name)
        {
            return Dashboards.Any(dashboard => dashboard.Title == name)
                ? new DashboardNameValidResponse(false, $"That Dashboard Name [{name}] already exists.")
                : new DashboardNameValidResponse(true);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns>Task.</returns>
        public Task Start()
        {
            Dashboards = new ObservableCollection<DashboardModel>();
            Dashboards.Add(new DashboardModel { Title = "My Dashboard" });
            SelectedDashboard = Dashboards[0];

            AvailableWidgets = new List<Widget> {
                new Widget("One By One", "Provides a one by one square widget.",
                    () => new FileOpenerWidgetViewModel(_widgetNumber++)),
                new Widget("One By One", "Provides a one by one square widget.",
                    () => new FileOpenerWidgetViewModel(_widgetNumber++)),
                new Widget("One By One", "Provides a one by one square widget.",
                    () => new FileOpenerWidgetViewModel(_widgetNumber++)),
                new Widget("One By One", "Provides a one by one square widget.",
                    () => new FileOpenerWidgetViewModel(_widgetNumber++)),
                new Widget("One By One", "Provides a one by one square widget.",
                    () => new FileOpenerWidgetViewModel(_widgetNumber++)),
                new Widget("One By One", "Provides a one by one square widget.",
                    () => new FileOpenerWidgetViewModel(_widgetNumber++)),
            };

            return Task.CompletedTask;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// DashboardView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DashboardView : System.Windows.Controls.UserControl
    {
        public DashboardsViewModel ViewModel;

        public DashboardView()
        {
            InitializeComponent();

            DataContext = ViewModel = new DashboardsViewModel();

            Loaded += (s, e) =>
            {
                ViewModel.Start();
            };
        }
    }
}
