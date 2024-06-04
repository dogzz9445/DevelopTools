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
using DDT.Apps.DDTOrganizer.Views;
using MahApps.Metro.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DDT.Apps.DDTOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftAlt || e.Key == Key.RightAlt)
            {
                ShowTitleBar = true;
            }
        }
    }
}