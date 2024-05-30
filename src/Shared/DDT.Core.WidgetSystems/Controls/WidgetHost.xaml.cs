using DDT.Core.WidgetSystems.Controls.Models;
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
using MouseButtonEventArgs = System.Windows.Input.MouseButtonEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;

namespace DDT.Core.WidgetSystems.Controls
{


    /// <summary>
    /// Delegate for creating drag events providing a widgetHost as the parameter
    /// </summary>
    /// <param name="widgetHost">The widget host.</param>
    public delegate void DragEventHandler(WidgetHost widgetHost);

    /// <summary>
    /// Delegate for creating drag events providing a widgetHost as the parameter
    /// </summary>
    /// <param name="widgetHost">The widget host.</param>
    public delegate void MouseEnterEventHandler(WidgetHost widgetHost);

    /// <summary>
    /// WidgetHost.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WidgetHost : ContentControl
    {
        #region Private Fields

        private Point? _mouseDownPoint;
        private bool? _isInOutlineArea;

        #endregion Private Fields

        #region Public Events

        /// <summary>
        /// Occurs when [drag started].
        /// </summary>
        public event DragEventHandler DragMoveStarted;

        /// <summary>
        /// Occurs when [drag started].
        /// </summary>
        public event DragEventHandler DragResizeStarted;

        /// <summary>
        /// 
        /// </summary>
        public event MouseEnterEventHandler MouseOver;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets the index of the host.
        /// </summary>
        /// <value>The index of the host.</value>
        public int HostIndex { get; set; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetHost"/> class.
        /// </summary>
        public WidgetHost()
        {
            InitializeComponent();

            Loaded += WidgetHost_Loaded;
            Unloaded += WidgetHost_Unloaded;

            MouseEnter += (s, e) => MouseOver?.Invoke(this);
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the Host control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Host_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mouseDownPoint = e.GetPosition(this);

            // MyBorder의 위치와 크기를 가져옵니다.
            Rect borderRect = new Rect(new Point(0, 0), this.RenderSize);

            // 외곽선 영역을 정의합니다.
            double borderThickness = 5; // 좌우 두께가 동일하다고 가정

            Rect outerRect = new Rect(borderRect.X, borderRect.Y,
                                      borderRect.Width, borderRect.Height);

            Rect innerRect = new Rect(borderRect.X + 4, borderRect.Y + 4,
                                      borderRect.Width - 8, borderRect.Height - 8);

            // 클릭한 위치가 외곽선 영역에 있는지 확인합니다.
            _isInOutlineArea = outerRect.Contains(
                _mouseDownPoint.Value.X,
                _mouseDownPoint.Value.Y)
                && !innerRect.Contains(_mouseDownPoint.Value.X, _mouseDownPoint.Value.Y);
            
            if (_isInOutlineArea == true)
                DragResizeStarted?.Invoke(this);
        }

        /// <summary>
        /// Handles the MouseMove event of the Host control. Used to invoke a drag started if the proper
        /// conditions have been met
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void Host_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var mouseMovePoint = e.GetPosition(this);

            // Check if we're "dragging" this control around. If not the return, otherwise invoke DragStarted event.
            if (!(_mouseDownPoint.HasValue) ||
                e.LeftButton == MouseButtonState.Released ||
                Point.Subtract(_mouseDownPoint.Value, mouseMovePoint).Length < SystemParameters.MinimumHorizontalDragDistance &&
                Point.Subtract(_mouseDownPoint.Value, mouseMovePoint).Length < SystemParameters.MinimumVerticalDragDistance)
                return;

            if (_isInOutlineArea == false)
                DragMoveStarted?.Invoke(this);
        }

        /// <summary>
        /// Handles the Loaded event of the WidgetHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void WidgetHost_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= WidgetHost_Loaded;

            PreviewMouseLeftButtonDown += Host_MouseLeftButtonDown;
            PreviewMouseMove += Host_PreviewMouseMove;
            MouseLeave += WidgetHost_MouseLeave;
        }

        private void WidgetHost_MouseLeave(object sender, MouseEventArgs e)
        {
            Console.WriteLine("");
        }

        /// <summary>
        /// Handles the Unloaded event of the WidgetHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void WidgetHost_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= WidgetHost_Unloaded;

            PreviewMouseLeftButtonDown -= Host_MouseLeftButtonDown;
            PreviewMouseMove -= Host_PreviewMouseMove;
            MouseLeave -= WidgetHost_MouseLeave;
        }

        #endregion Private Methods
    }
}
