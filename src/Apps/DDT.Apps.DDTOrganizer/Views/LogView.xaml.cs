using CommunityToolkit.Mvvm.ComponentModel;
using DDT.Core;
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

namespace DDT.Apps.DDTOrganizer.Views
{

    public partial class Log : ObservableObject
    {
        [ObservableProperty]
        private string _message;
    }

    public partial class LogViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Log> _logs;

        public LogViewModel()
        {
            _logs = new ObservableCollection<Log>();
            Logger.Instance.LogEvent += new EventHandler<string>((s, e) => _logs.Add(new Log() { Message = e }));
        }
    }

    /// <summary>
    /// LogView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LogView : Page
    {
        public LogViewModel ViewModel;

        public LogView()
        {
            InitializeComponent();

            DataContext = ViewModel = new LogViewModel();
        }
    }
}
