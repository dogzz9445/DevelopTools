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

namespace DDT.Core.WidgetSystems.WPF.Widgets.FileOpeners
{
    public partial class FileOpenerWidgetViewModel : WidgetHostViewModel
    {
        [ObservableProperty]
        private string? _filePath;

        [ObservableProperty]
        private string? _fileType;

        [ObservableProperty]
        private string? _fileDescription;

        [ObservableProperty]
        private string? _fileSize;

        [ObservableProperty]
        private string? _fileLastModified;

        [ObservableProperty]
        private string? _fileCreated;

        [ObservableProperty]
        private string? _fileAccessed;

        [ObservableProperty]
        private string? _fileAttributes;

        [ObservableProperty]
        private string? _fileOwner;

        [ObservableProperty]
        private string? _fileGroup;

        [ObservableProperty]
        private string? _filePermissions;

        [ObservableProperty]
        private string? _fileContent;

        [ObservableProperty]
        private string? _fileContentLength;

        [ObservableProperty]
        private string? _fileContentEncoding;

        [ObservableProperty]
        private string? _fileContentHash;

        [ObservableProperty]
        private string? _fileContentHashAlgorithm;

        [ObservableProperty]
        private string? _fileContentHashLength;

        [ObservableProperty]
        private string? _fileContentHashEncoding;

        [ObservableProperty]
        private string? _fileContentHashSalt;

        [ObservableProperty]
        private string? _fileContentHashPepper;

        [ObservableProperty]
        private string? _fileContentHashIterations;

        [ObservableProperty]
        private string? _fileContentHashKeySize;

        [ObservableProperty]
        private string? _fileContentHashBlockSize;

        [ObservableProperty]
        private string? _fileContentHashMode;

        [ObservableProperty]
        private string? _fileContentHashPadding;

        [ObservableProperty]
        private string? _fileContentHashFeedbackSize;

        [ObservableProperty]
        private string? _fileContentHashSaltLength;

        [ObservableProperty]
        private string? _fileContentHashPepperLength;

        [ObservableProperty]
        private string? _fileContentHashSaltEncoding;

        [ObservableProperty]
        private string? _fileContentHashPepperEncoding;

        [ObservableProperty]
        private string? _fileContentHashSaltHash;

        [ObservableProperty]
        private string? _fileContentHashPepperHash;

        [ObservableProperty]
        private string? _fileContentHashSaltHashAlgorithm;

        [ObservableProperty]
        private string? _fileContentHashPepperHashAlgorithm;

        [ObservableProperty]
        private string? _fileContentHashSaltHashLength;


        /// <summary>
        /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
        /// </summary>
        public FileOpenerWidgetViewModel(int widgetNumber) : base()
        {
            Title = $"FileOpener{widgetNumber}";
            RowSpanColumnSpan = new RowSpanColumnSpan(1, 1);
        }
    }

    /// <summary>
    /// FileOpenerWidget.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FileOpenerWidget : System.Windows.Controls.UserControl
    {
        public FileOpenerWidget()
        {
            InitializeComponent();
        }
    }
}
