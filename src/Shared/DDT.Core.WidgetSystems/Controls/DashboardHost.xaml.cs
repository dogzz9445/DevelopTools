using DDT.Core.WidgetSystems.Controls;
using DDT.Core.WidgetSystems.Controls.Models;
using DDT.Core.WidgetSystems.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using Brushes = System.Windows.Media.Brushes;
using Cursors = System.Windows.Input.Cursors;
using DataObject = System.Windows.DataObject;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using GiveFeedbackEventArgs = System.Windows.GiveFeedbackEventArgs;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using Size = System.Windows.Size;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using System.Data.Common;
using System.Windows.Media.Animation;
using System.Numerics;

namespace DDT.Core.WidgetSystems.Controls
{
    /// <summary>
    /// Custom ItemsControl that represents a dynamic Dashboard similar to the one used on TFS(Azure DevOps Server) Dashboards
    /// </summary>
    public partial class DashboardHost : ItemsControl
    {
        #region Public Fields

        public static readonly DependencyProperty EnableColumnLimitProperty = DependencyProperty.Register(
            nameof(EnableColumnLimit),
            typeof(bool),
            typeof(DashboardHost),
            new PropertyMetadata(true));

        public bool EnableColumnLimit
        {
            get => (bool)GetValue(EnableColumnLimitProperty);
            set => SetValue(EnableColumnLimitProperty, value);
        }

        public static readonly DependencyProperty MaxNumColumnsProperty = DependencyProperty.Register(
            nameof(MaxNumColumns),
            typeof(int),
            typeof(DashboardHost),
            new PropertyMetadata(16));

        public int MaxNumColumns
        {
            get => (int)GetValue(MaxNumColumnsProperty);
            set => SetValue(MaxNumColumnsProperty, value);
        }

        public static readonly DependencyProperty VisibleRowsProperty = DependencyProperty.Register(
            nameof(VisibleRows),
            typeof(int),
            typeof(DashboardHost),
            new PropertyMetadata(8));

        public int VisibleRows
        {
            get => (int)GetValue(VisibleRowsProperty);
            set => SetValue(VisibleRowsProperty, value);
        }

        /// <summary>
        /// The edit mode property
        /// </summary>
        public static readonly DependencyProperty EditModeProperty = DependencyProperty.Register(
            nameof(EditMode),
            typeof(bool),
            typeof(DashboardHost),
            new PropertyMetadata(false, (d, e) => ((DashboardHost)d).EditEnabler()));

        /// <summary>
        /// Gets or sets a value indicating whether the dashboard is in [edit mode].
        /// </summary>
        /// <value><c>true</c> if [edit mode]; otherwise, <c>false</c>.</value>
        public bool EditMode
        {
            get => (bool)GetValue(EditModeProperty);
            set => SetValue(EditModeProperty, value);
        }

        #endregion Public Fields

        #region Private Fields
        private const int ScrollIncrement = 15;
        private readonly PropertyChangeNotifier _itemsSourceChangeNotifier;
        private readonly List<WidgetHost> _widgetHosts = new List<WidgetHost>();
        private readonly List<WidgetHostData> _beforeDragWidgetHostDatas = new List<WidgetHostData>();
        private Canvas _canvasEditingBackground;
        private ScrollViewer _dashboardScrollViewer;
        private DragAdorner _draggingAdorner;
        private WidgetHost _draggingHost;
        private WidgetHostData _draggingHostData;
        private int _hostIndex;
        private Border _widgetDestinationHighlight;

        // To change the overall size of the widgets change the value here. This size is considered a block.
        private Size _widgetHostMinimumSize = new Size(64, 64);

        private List<WidgetHostData> _widgetHostsData = new List<WidgetHostData>();
        private Canvas _widgetsCanvasHost;

        #endregion Private Fields

        #region Private Properties

        /// <summary>
        /// Gets the canvas editing background that shows empty spaces (gray square in the UI) for editing.
        /// </summary>
        /// <value>The canvas editing background.</value>
        private Canvas CanvasEditingBackground => _canvasEditingBackground ??
                                                  (_canvasEditingBackground = this.FindChildElementByName<Canvas>("CanvasEditingBackground"));

        /// <summary>
        /// Gets the dashboard scroll viewer.
        /// </summary>
        /// <value>The dashboard scroll viewer.</value>
        private ScrollViewer DashboardScrollViewer => _dashboardScrollViewer ??
                                                      (_dashboardScrollViewer = this.FindChildElementByName<ScrollViewer>("DashboardHostScrollViewer"));

        /// <summary>
        /// Gets the widgets canvas host.
        /// </summary>
        /// <value>The widgets canvas host.</value>
        private Canvas WidgetsCanvasHost
        {
            get
            {
                if (_widgetsCanvasHost != null)
                    return _widgetsCanvasHost;

                // We have to **cheat** in order to get the ItemsHost of this ItemsControl by
                // using reflection to gain access to the NonPublic member
                _widgetsCanvasHost = (Canvas)typeof(ItemsControl).InvokeMember("ItemsHost",
                    BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance,
                    null, this, null);

                WidgetsCanvasHost.HorizontalAlignment = HorizontalAlignment.Left;
                WidgetsCanvasHost.VerticalAlignment = VerticalAlignment.Top;

                SetupCanvases();

                return _widgetsCanvasHost;
            }
        }

        #endregion Private Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardHost"/> class.
        /// </summary>
        public DashboardHost()
        {
            InitializeComponent();

            ItemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(Canvas)));
            Loaded += DashboardHost_Loaded;
            Unloaded += DashboardHost_Unloaded;

            _itemsSourceChangeNotifier = new PropertyChangeNotifier(this, ItemsSourceProperty);
            _itemsSourceChangeNotifier.ValueChanged += ItemsSource_Changed;
        }

        #endregion Public Constructors

        #region Protected Methods

        /// <summary>
        /// When overridden in a derived class, undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)" /> method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            if (!(element is WidgetHost widgetHost))
                return;

            //widgetHost.MouseOver -= WidgetHost_MouseOver;
            widgetHost.DragStarted -= WidgetHost_DragStarted;
            _widgetHosts.Remove(widgetHost);
            _widgetHostsData = _widgetHostsData.Where(widgetData => widgetData.HostIndex != widgetHost.HostIndex)
                .ToList();

            //if (EditMode)
            //    FixArrangements();
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new WidgetHost { HostIndex = _hostIndex++ };
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns><see langword="true" /> if the item is (or is eligible to be) its own container; otherwise, <see langword="false" />.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is WidgetHost;
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (!(element is WidgetHost widgetHost) || WidgetsCanvasHost == null)
                return;

            var widgetBase = widgetHost.DataContext as WidgetViewModelBase;

            Debug.Assert(widgetBase != null, nameof(widgetBase) + " != null");

            var widgetSpans = widgetBase.RowSpanColumnSpan;

            // Set min/max dimensions of host so it isn't allowed to grow any larger or smaller
            var hostHeight = _widgetHostMinimumSize.Height * widgetSpans.RowSpan - widgetHost.Margin.Top - widgetHost.Margin.Bottom;
            var hostWidth = _widgetHostMinimumSize.Width * widgetSpans.ColumnSpan - widgetHost.Margin.Left - widgetHost.Margin.Right;

            widgetHost.MinHeight = hostHeight;
            widgetHost.MaxHeight = hostHeight;
            widgetHost.MinWidth = hostWidth;
            widgetHost.MaxWidth = hostWidth;

            // Subscribe to the widgets drag started and add the widget
            // to the _widgetHosts to keep tabs on it
            //widgetHost.MouseOver += WidgetHost_MouseOver;
            widgetHost.DragStarted += WidgetHost_DragStarted;
            _widgetHosts.Add(widgetHost);
            _widgetHostsData.Add(new WidgetHostData(widgetHost.HostIndex, widgetBase, widgetSpans));

            // Check if widget is new by seeing if ColumnIndex or RowIndex are set
            // If it isn't new then just set its location
            if (widgetBase.RowIndexColumnIndex != null)
            {
                var widgetAlreadyThere = WidgetAtLocation(widgetSpans, widgetBase.RowIndexColumnIndex);

                if (widgetAlreadyThere == null || !widgetAlreadyThere.Any())
                {
                    SetWidgetRowAndColumn(widgetHost, widgetBase.RowIndexColumnIndex, widgetSpans, false);
                    return;
                }
            }

            // widget is new. Find the next available row and column and place the
            // widget then scroll to it if it's offscreen
            var nextAvailable = GetNextAvailableRowColumn(widgetSpans);

            SetWidgetRowAndColumn(widgetHost, nextAvailable, widgetSpans, false);

            // Scroll to the new item if it is off screen
            var widgetsHeight = widgetSpans.RowSpan * _widgetHostMinimumSize.Height;
            var widgetEndVerticalLocation = nextAvailable.Row * _widgetHostMinimumSize.Height + widgetsHeight; 

            var scrollViewerVerticalScrollPosition =
                DashboardScrollViewer.ViewportHeight + DashboardScrollViewer.VerticalOffset;

            if (!(widgetEndVerticalLocation >= DashboardScrollViewer.VerticalOffset) ||
                !(widgetEndVerticalLocation <= scrollViewerVerticalScrollPosition))
                DashboardScrollViewer.ScrollToVerticalOffset(
                    widgetEndVerticalLocation - widgetsHeight - ScrollIncrement);
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Handles the GiveFeedback event of the DraggingHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="GiveFeedbackEventArgs"/> instance containing the event data.</param>
        private static void DraggingHost_GiveFeedback(object sender, GiveFeedbackEventArgs args)
        {
            // Due to the DragDrop we have to use the GiveFeedback on the first parameter DependencyObject
            // passed into the DragDrop.DoDragDrop to force the cursor to show SizeAll
            args.UseDefaultCursors = false;
            Mouse.SetCursor(Cursors.SizeAll);
            args.Handled = true;
        }

        /// <summary>
        /// Adds a column to the editing background canvas.
        /// </summary>
        private void AddCanvasEditingBackgroundColumn(int rowCount, int columnCount)
        {
            CanvasEditingBackground.Width += _widgetHostMinimumSize.Width;

            //for (var i = 0; i < rowCount; i++)
            //{
            //    var rectangleBackground = CreateGrayRectangleBackground();
            //    CanvasEditingBackground.Children.Add(rectangleBackground);
            //    Canvas.SetTop(rectangleBackground, i * _widgetHostMinimumSize.Height);
            //    Canvas.SetLeft(rectangleBackground, columnCount * _widgetHostMinimumSize.Width);
            //}
        }

        /// <summary>
        /// Adds a row to the editing background canvas.
        /// </summary>
        private void AddCanvasEditingBackgroundRow(int rowCount, int columnCount)
        {
            CanvasEditingBackground.Height += _widgetHostMinimumSize.Height;

            //for (var i = 0; i < columnCount; i++)
            //{
            //    var rectangleBackground = CreateGrayRectangleBackground();
            //    CanvasEditingBackground.Children.Add(rectangleBackground);
            //    Canvas.SetTop(rectangleBackground, rowCount * _widgetHostMinimumSize.Height);
            //    Canvas.SetLeft(rectangleBackground, i * _widgetHostMinimumSize.Width);
            //}
        }

        /// <summary>
        /// Returns a Rectangle that has a background that is gray. Used for the CanvasEditingBackground Canvas.
        /// </summary>
        /// <returns>Rectangle.</returns>
        private Rectangle CreateGrayRectangleBackground()
        {
            return new Rectangle
            {
                Height = Math.Floor(_widgetHostMinimumSize.Height * 90 / 100),
                Width = Math.Floor(_widgetHostMinimumSize.Width * 90 / 100),
                Fill = Brushes.LightGray
            };
        }

        /// <summary>
        /// Handles the Loaded event of the DashboardHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DashboardHost_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= DashboardHost_Loaded;

            // We only check WidgetsCanvasHost to initialize just in case it wasn't initialized
            // with pre-existing widgets being generated before load
            if (WidgetsCanvasHost == null)
                return;

            SizeChanged += DashboardHost_SizeChanged;
            PreviewDragOver += DashboardHost_PreviewDragOver;
            //DragLeave
        }

        /// <summary>
        /// Handles the PreviewDragOver event of the DashboardHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        private void DashboardHost_PreviewDragOver(object sender, DragEventArgs e)
        {
            // Only continue if the item being dragged over the DashboardHost is a WidgetHost and
            // the widget host is within the _widgetHosts list
            if (!(e.Data.GetData(typeof(WidgetHost)) is WidgetHost draggingWidgetHost) ||
                _widgetHostsData.FirstOrDefault(widgetData => widgetData.HostIndex == draggingWidgetHost.HostIndex) == null)
                return;

            // Move the adorner to the appropriate position
            _draggingAdorner.LeftOffset = e.GetPosition(WidgetsCanvasHost).X;
            _draggingAdorner.TopOffset = e.GetPosition(WidgetsCanvasHost).Y;

            var adornerPosition = _draggingAdorner.TransformToVisual(WidgetsCanvasHost).Transform(new Point(0, 0));

            // The adorner will typically start out at X == 0 and Y == 0 which causes an unwanted effect of re-positioning
            // items when it isn't necessary.
            if (adornerPosition.X == 0 && adornerPosition.Y == 0)
                return;

            // When dragging and the adorner gets close to the sides of the scroll viewer then have the scroll viewer
            // automatically scroll in the direction of adorner's edges
            var adornerPositionRelativeToScrollViewer =
                _draggingAdorner.TransformToVisual(DashboardScrollViewer).Transform(new Point(0, 0));

            if (adornerPositionRelativeToScrollViewer.Y + _draggingAdorner.ActualHeight + ScrollIncrement >= DashboardScrollViewer.ViewportHeight)
                DashboardScrollViewer.ScrollToVerticalOffset(DashboardScrollViewer.VerticalOffset + ScrollIncrement);
            if (adornerPositionRelativeToScrollViewer.X + _draggingAdorner.ActualWidth + ScrollIncrement >= DashboardScrollViewer.ViewportWidth)
                DashboardScrollViewer.ScrollToHorizontalOffset(DashboardScrollViewer.HorizontalOffset + ScrollIncrement);
            if (adornerPositionRelativeToScrollViewer.Y - ScrollIncrement <= 0 && DashboardScrollViewer.VerticalOffset >= ScrollIncrement)
                DashboardScrollViewer.ScrollToVerticalOffset(DashboardScrollViewer.VerticalOffset - ScrollIncrement);
            if (adornerPositionRelativeToScrollViewer.X - ScrollIncrement <= 0 && DashboardScrollViewer.HorizontalOffset >= ScrollIncrement)
                DashboardScrollViewer.ScrollToHorizontalOffset(DashboardScrollViewer.HorizontalOffset - ScrollIncrement);

            // We need to get the adorner imaginary position or position in which we'll use to determine what cell it is hovering over.
            // We do this by getting the width of the host and then divide this by the span + 1
            // In a 1x1 widget this would essentially give us the half way point to which would change the _closestRowColumn
            // In a larger widget (2x2) this would give us the point at 1/3 of the size ensuring the widget can get to its destination more seamlessly
            var addToPositionX = draggingWidgetHost.ActualWidth / (_draggingHostData.WidgetBase.RowSpanColumnSpan.ColumnSpan + 1);
            var addToPositionY = draggingWidgetHost.ActualHeight / (_draggingHostData.WidgetBase.RowSpanColumnSpan.RowSpan + 1);

            // Get the closest row/column to the adorner "imaginary" position
            var closestRowColumn =
                GetClosestRowColumn(new Point(adornerPosition.X + addToPositionX, adornerPosition.Y + addToPositionY));

            // If there is no change to the stored closestRowColumn against the dragging RowIndex and ColumnIndex then there isn't
            // anything to set or arrange.
            //if (_draggingHostData.WidgetBase.RowIndexColumnIndex.Row == closestRowColumn.Row &&
            //    _draggingHostData.WidgetBase.RowIndexColumnIndex.Column == closestRowColumn.Column)
            //    return;

            // Use the canvas to draw a square around the closestRowColumn to indicate where the _draggingWidgetHost will be when mouse is released
            var top = closestRowColumn.Row < 0 ? 0 : closestRowColumn.Row * _widgetHostMinimumSize.Height;
            var left = closestRowColumn.Column < 0 ? 0 : closestRowColumn.Column * _widgetHostMinimumSize.Width;

            Canvas.SetTop(_widgetDestinationHighlight, top);
            Canvas.SetLeft(_widgetDestinationHighlight, left);

            // Set the _dragging host into its dragging position
            SetWidgetRowAndColumn(_draggingHost, closestRowColumn, _draggingHostData.WidgetBase.RowSpanColumnSpan);

            // Get all the widgets in the path of where the _dragging host will be set
            var movingWidgets = GetWidgetMoveList(_widgetHostsData.FirstOrDefault(widgetData => widgetData == _draggingHostData), closestRowColumn, null)
                .OrderBy(widgetData => widgetData.WidgetBase?.RowIndexColumnIndex.Row)
                .ToList();

            // Move the movingWidgets down in rows the same amount of the _dragging hosts row span
            // unless there is a widget already there in that case increment until there isn't. We
            // used the OrderBy on the movingWidgets to make this work against widgets that have
            // already moved
            var movedWidgets = new List<WidgetHostData>();
            foreach (var widgetData in movingWidgets.ToArray())
            {
                // Use the initial amount the dragging widget row size is
                var rowIncrease = 1;

                // Find a row to move it
                Debug.Assert(widgetData.WidgetBase.RowIndexColumnIndex.Row != null, "widgetData.WidgetBase.RowIndexColumnIndex.Row != null");
                Debug.Assert(widgetData.WidgetBase.RowIndexColumnIndex.Column != null, "widgetData.WidgetBase.RowIndexColumnIndex.Column != null");

                while (true)
                {
                    var widgetAtLoc = WidgetAtLocation(widgetData.WidgetBase.RowSpanColumnSpan,
                        new RowIndexColumnIndex(widgetData.WidgetBase.RowIndexColumnIndex.Row + rowIncrease, widgetData.WidgetBase.RowIndexColumnIndex.Column))
                        .Where(widgetHostData => !movingWidgets.Contains(widgetHostData) || movedWidgets.Contains(widgetHostData));

                    if (!widgetAtLoc.Any())
                        break;

                    rowIncrease++;
                }

                var movingHost = _widgetHosts.FirstOrDefault(widgetHost => widgetHost.HostIndex == widgetData.HostIndex);

                var proposedRow = widgetData.WidgetBase.RowIndexColumnIndex.Row + rowIncrease;
                for (int row = widgetData.WidgetBase.PreviewRowIndexColumnIndex.Row; row <= proposedRow; row++)
                {
                    var reArragnedIndex = new RowIndexColumnIndex(row, widgetData.WidgetBase.PreviewRowIndexColumnIndex.Column);

                    var widgetAlreadyThere = WidgetAtLocation(widgetData.WidgetBase.RowSpanColumnSpan, reArragnedIndex)
                        .Where(widgetHostDataThere => widgetData != widgetHostDataThere && !movingWidgets.Contains(widgetHostDataThere));

                    if (widgetAlreadyThere.Any())
                        continue;

                    var widgetHost = _widgetHosts.FirstOrDefault(widgetHost => widgetHost.HostIndex == widgetData.HostIndex);

                    SetWidgetRowAndColumn(widgetHost, reArragnedIndex, widgetData.WidgetBase.RowSpanColumnSpan);
                    movingWidgets.Remove(widgetData);
                    break;
                }

                movedWidgets.Add(widgetData);
            }

            ReArrangeToPreviewLocation();
            // FixArrangements();
        }

        /// <summary>
        /// Handles the SizeChanged event of the DashboardHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
        private void DashboardHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!EditMode)
                return;

            RemoveExcessCanvasSize(CanvasEditingBackground);
            FillCanvasEditingBackground();
        }

        /// <summary>
        /// Handles the Unloaded event of the DashboardHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DashboardHost_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= DashboardHost_Unloaded;
            SizeChanged -= DashboardHost_SizeChanged;
            PreviewDragOver -= DashboardHost_PreviewDragOver;

            if (_itemsSourceChangeNotifier != null)
                _itemsSourceChangeNotifier.ValueChanged -= ItemsSource_Changed;
        }

        /// <summary>
        /// Enabled/Disables edit functionality
        /// </summary>
        private void EditEnabler()
        {
            // Show or hide the CanvasEditingBackground depending on the EditMode
            CanvasEditingBackground.Visibility = EditMode ? Visibility.Visible : Visibility.Collapsed;

            if (EditMode)
                return;

            // We then need to remove all the extra row and column we no longer need
            RemoveExcessCanvasSize(CanvasEditingBackground);
            RemoveExcessCanvasSize(WidgetsCanvasHost);
        }

        /// <summary>
        /// Fills the canvas editing background.
        /// </summary>
        private void FillCanvasEditingBackground()
        {
            var visibleColumns = EnableColumnLimit ? MaxNumColumns + 1 : GetFullyVisibleColumn() + 1;
            var visibleRows = GetFullyVisibleRow() + 1;
            // FIXME:
            // 제대로 백그라운드를 수정하지 않는 오류가 있음 수정 필요
            // Fill Visible Columns
            var rowCountForColumnAdditions = GetCanvasEditingBackgroundRowCount();
            while (true)
            {
                var columnCount = GetCanvasEditingBackgroundColumnCount();

                if (columnCount >= visibleColumns)
                    break;
                AddCanvasEditingBackgroundColumn(rowCountForColumnAdditions, columnCount);
            }

            // Fill Visible Rows
            var columnCountForRowAdditions = GetCanvasEditingBackgroundColumnCount();
            while (true)
            {
                var rowCount = GetCanvasEditingBackgroundRowCount();

                if (rowCount >= visibleRows)
                    break;

                AddCanvasEditingBackgroundRow(rowCount, columnCountForRowAdditions);
            }
        }

        private void FixArrangements()
        {
            var arrangementNecessary = true;

            //Need to check for empty spots to see if widgets in rows down from it can possible be placed in those empty spots
            //Once there are no more available to move we set arrangementNecessary to false and we're done
            while (arrangementNecessary)
                arrangementNecessary = ReArrangeFirstEmptySpot();
        }

        /// <summary>
        /// Gets the canvas editing background column count.
        /// </summary>
        /// <returns>System.Int32.</returns>
        private int GetCanvasEditingBackgroundColumnCount()
        {
            return (int)Math.Floor(CanvasEditingBackground.Width / _widgetHostMinimumSize.Width);
        }

        /// <summary>
        /// Gets the canvas editing background row count.
        /// </summary>
        /// <returns>System.Int32.</returns>
        private int GetCanvasEditingBackgroundRowCount()
        {
            return (int)Math.Floor(CanvasEditingBackground.Height / _widgetHostMinimumSize.Height);
        }

        /// <summary>
        /// Gets the closest row column from adornerPosition.
        /// </summary>
        /// <param name="adornerPosition">The adorner position.</param>
        /// <returns>RowAndColumn.</returns>
        private RowIndexColumnIndex GetClosestRowColumn(Point adornerPosition)
        {
            // First lets get the "real" closest row and column to the adorner Position
            // This is exact location to the square to which the adornerPosition point provided
            // is within
            var realClosestRow = (int)Math.Floor(adornerPosition.Y / _widgetHostMinimumSize.Height);
            var realClosestColumn = (int)Math.Floor(adornerPosition.X / _widgetHostMinimumSize.Width);

            // If the closest row and column are negatives that means the position is off screen
            // at this point we can return 0's and prevent extra calculations by ending this one here
            if (realClosestRow < 0 && realClosestColumn < 0)
                return new RowIndexColumnIndex(0, 0);

            // We need to set any negatives to 0 since we can't place anything off screen
            realClosestRow = realClosestRow < 0 ? 0 : realClosestRow;
            realClosestColumn = realClosestColumn < 0 ? 0 : realClosestColumn;
            if (EnableColumnLimit)
            {
                realClosestColumn = realClosestColumn + _draggingHostData.WidgetBase.RowSpanColumnSpan.ColumnSpan > MaxNumColumns ? MaxNumColumns - _draggingHostData.WidgetBase.RowSpanColumnSpan.ColumnSpan : realClosestColumn;
            }

            var realClosestRowAndColumn = new RowIndexColumnIndex(realClosestRow, realClosestColumn);

            if (_draggingHostData.WidgetBase.RowIndexColumnIndex.Row == realClosestRow && _draggingHostData.WidgetBase.RowIndexColumnIndex.Column == realClosestColumn)
                return realClosestRowAndColumn;
            return realClosestRowAndColumn;
            // We need to find all the widgets that land within the column that the adorner is currently
            // placed and if that dragging widget has a span we need to calculate that into this.
            // Once we have all those widgets we need to get the max row out of all the widgets
            int lastRowForColumn = 0;
            var lastRowForColumns = _widgetHostsData
                .Where(widgetData =>
                {
                    if (widgetData == _draggingHostData)
                        return false;

                    // Loop through each span of the draggingBase
                    for (var i = 0; i < _draggingHostData.WidgetBase.RowSpanColumnSpan.ColumnSpan; i++)
                    {
                        // Then loop through each span of the current widget being evaluated
                        for (var j = 0; j < widgetData.WidgetBase.RowSpanColumnSpan.ColumnSpan; j++)
                        {
                            // If the column is within the adorner position and its span then include it
                            if (widgetData.WidgetBase.RowIndexColumnIndex.Column + j == realClosestColumn + i)
                                return true;
                        }
                    }

                    return false;
                })
                // Get the row index and its span and calculated that number as being the row it's actually on
                // this helps in finding the max row the dragging widget can reside
                .Select(widgetData => widgetData.WidgetBase.RowIndexColumnIndex.Row + widgetData.WidgetBase.RowSpanColumnSpan.RowSpan - 1);

            // If there aren't any widgets is when this comes back null. In that case return 0 to the variable
            if (lastRowForColumns.Any())
            {
                lastRowForColumn = lastRowForColumns.Max() + 1;
            }

            // If the adorner position is on the outside of other widgets and within the columns of that
            // position then return back the last used row + 1 (equates to being lastRowForColumn)
            if (realClosestRow >= lastRowForColumn)
                return new RowIndexColumnIndex(lastRowForColumn, realClosestColumn);

            // First lets see if we're moving down in rows and our column hasn't changed. If this is true lets
            // see if any of the widgets at the location can fit where the now dragging widget was.
            // If so then we can assume the ReArrangeFirstEmptySpot will move a widget into the old location
            // and we can just return that realClosestRowAndColumn
            if (realClosestColumn == _draggingHostData.WidgetBase.RowIndexColumnIndex.Column &&
                realClosestRow > _draggingHostData.WidgetBase.RowIndexColumnIndex.Row)
            {
                var widgetsAtLocation = WidgetAtLocation(_draggingHostData.WidgetBase.RowSpanColumnSpan, realClosestRowAndColumn)
                    .Where(widgetData => widgetData != _draggingHostData)
                    .OrderBy(widgetData => widgetData.WidgetBase.RowIndexColumnIndex.Row);

                int? checkedGoodRow = null;
                foreach (var widgetHostData in widgetsAtLocation)
                {
                    if (checkedGoodRow != null && widgetHostData.WidgetBase.RowIndexColumnIndex.Row != checkedGoodRow)
                        continue;

                    Debug.Assert(widgetHostData.WidgetBase.RowIndexColumnIndex.Row != null, "widgetHostData.WidgetBase.RowIndexColumnIndex.Row != null");
                    Debug.Assert(widgetHostData.WidgetBase.RowIndexColumnIndex.Column != null, "widgetHostData.WidgetBase.RowIndexColumnIndex.Column != null");

                    var difference = realClosestRow + (int)_draggingHostData.WidgetBase.RowIndexColumnIndex.Row +
                        _draggingHostData.WidgetBase.RowSpanColumnSpan.RowSpan - 1;
                    for (var i = difference; i >= realClosestRow; i--)
                    {
                        var widgetsAtWidgetHostDataLocation = WidgetAtLocation(widgetHostData.WidgetBase.RowSpanColumnSpan,
                            new RowIndexColumnIndex(i, (int)widgetHostData.WidgetBase.RowIndexColumnIndex.Column))
                            .Where(widgetData => widgetData != _draggingHostData)
                            .ToArray();

                        if (widgetsAtWidgetHostDataLocation.Contains(widgetHostData))
                        {
                            if (_draggingHostData.WidgetBase.RowIndexColumnIndex.Row + widgetHostData.WidgetBase.RowSpanColumnSpan.RowSpan <=
                                realClosestRow)
                            {
                                checkedGoodRow = widgetHostData.WidgetBase.RowIndexColumnIndex.Row;
                                if (!WidgetAtLocation(widgetHostData.WidgetBase.RowSpanColumnSpan,
                                    new RowIndexColumnIndex((int)_draggingHostData.WidgetBase.RowIndexColumnIndex.Row,
                                        (int)widgetHostData.WidgetBase.RowIndexColumnIndex.Column)).Any(widgetData =>
                                   widgetData != widgetHostData && widgetData != _draggingHostData))
                                    return realClosestRowAndColumn;
                            }

                            //continue;
                        }

                        if (_draggingHostData.WidgetBase.RowIndexColumnIndex.Row + 1 >= realClosestRow &&
                            realClosestRow <= _draggingHostData.WidgetBase.RowIndexColumnIndex.Row +
                            _draggingHostData.WidgetBase.RowSpanColumnSpan.RowSpan - 1)
                            continue;

                        if (!widgetsAtWidgetHostDataLocation.Any() &&
                            i < realClosestRow + _draggingHostData.WidgetBase.RowSpanColumnSpan.RowSpan - 1)
                            return realClosestRowAndColumn;
                    }
                }
            }

            // The adorner position is within other widgets. We need to see if the row(s) above the closest are available
            // to fit the draggingHost into it and place it there if we can.
            var potentialMovingWidgets =
                GetWidgetMoveList(
                    _widgetHostsData.FirstOrDefault(widgetData => widgetData == _draggingHostData),
                    realClosestRowAndColumn, null).ToArray();

            RowIndexColumnIndex foundBetterSpot = null;
            for (var i = realClosestRow - 1; i >= 0; i--)
            {
                var rowAndColumnToCheck = new RowIndexColumnIndex(i, realClosestColumn);

                // See if we can use the location. We keep iterating after this to see if there is even a better spot we can occupy
                var widgetAtLocation = WidgetAtLocation(_draggingHostData.WidgetBase.RowSpanColumnSpan, rowAndColumnToCheck)
                    .Where(widgetData =>
                        !potentialMovingWidgets.Contains(widgetData) && widgetData != _draggingHostData);

                if (widgetAtLocation.Any())
                    break;

                foundBetterSpot = new RowIndexColumnIndex(i, realClosestColumn);
            }

            return foundBetterSpot ?? realClosestRowAndColumn;
        }

        /// <summary>
        /// Gets the count of the Columns that are fully visible taking into account
        /// the DashboardScrollViewer could have a horizontal offset
        /// </summary>
        /// <returns>System.Int32.</returns>
        private int GetFullyVisibleColumn()
        {
            return Convert.ToInt32(Math.Floor(
                (ActualWidth + DashboardScrollViewer.HorizontalOffset) / _widgetHostMinimumSize.Width));
        }

        /// <summary>
        /// Gets the count of the Rows that are fully visible taking into account
        /// the DashboardScrollViewer could have a vertical offset
        /// </summary>
        /// <returns>System.Int32.</returns>
        private int GetFullyVisibleRow()
        {
            return Convert.ToInt32(Math.Floor(
                (ActualHeight + DashboardScrollViewer.VerticalOffset) / _widgetHostMinimumSize.Height));
        }

        /// <summary>
        /// Gets the maximum rows and columns of a grid including their spans as placement.
        /// </summary>
        /// <returns>WpfDashboardControl.Models.RowAndColumn.</returns>
        private RowIndexColumnIndex GetMaxRowsAndColumns()
        {
            // Need to get all rows adding their row spans and columns adding their column spans returned back
            // as an array of RowAndColumns
            var widgetsRowsAndColumns = _widgetHostsData
                .Select(widgetData =>
                {
                    Debug.Assert(widgetData.WidgetBase.RowIndexColumnIndex.Row != null, "widgetData.WidgetBase.RowIndexColumnIndex.Row != null");
                    Debug.Assert(widgetData.WidgetBase.RowIndexColumnIndex.Column != null, "widgetData.WidgetBase.RowIndexColumnIndex.Column != null");

                    return new RowIndexColumnIndex((int)widgetData.WidgetBase.RowIndexColumnIndex.Row + widgetData.WidgetBase.RowSpanColumnSpan.RowSpan,
                        (int)widgetData.WidgetBase.RowIndexColumnIndex.Column + widgetData.WidgetBase.RowSpanColumnSpan.ColumnSpan);
                })
                .ToArray();

            if (!widgetsRowsAndColumns.Any())
                return new RowIndexColumnIndex(1, 1);

            // Need to get the max row and max columns from the list of RowAndColumns
            var maxRows = widgetsRowsAndColumns
                .Select(rowColumn => rowColumn.Row)
                .Max();
            var maxColumns = EnableColumnLimit ? MaxNumColumns : widgetsRowsAndColumns
                .Select(rowColumn => rowColumn.Column)
                .Max();

            return new RowIndexColumnIndex(maxRows, maxColumns);
        }

        /// <summary>
        /// Gets the next available row/column.
        /// </summary>
        /// <param name="widgetSpans">The widget spans.</param>
        /// <returns>RowAndColumn.</returns>
        private RowIndexColumnIndex GetNextAvailableRowColumn(RowSpanColumnSpan widgetSpans)
        {
            if (_widgetHostsData.Count == 1)
                return new RowIndexColumnIndex(0, 0);

            // Get fully visible column count
            var fullyVisibleColumns = GetFullyVisibleColumn();

            // We need to loop through each row and in each row loop through each column
            // to see if the space is currently occupied. When it is available then return
            // what Row and Column the new widget will occupy
            var rowCount = 0;
            while (true)
            {
                for (var column = 0; column < fullyVisibleColumns; column++)
                {
                    var widgetAlreadyThere = WidgetAtLocation(widgetSpans, new RowIndexColumnIndex(rowCount, column));

                    if (widgetAlreadyThere != null && widgetAlreadyThere.Any())
                        continue;

                    // Need to check if the new widget when placed would be outside
                    // the visible columns. If so then we move onto the next row/column
                    var newWidgetSpanOutsideVisibleColumn = false;
                    for (var i = 0; i < widgetSpans.ColumnSpan + 1; i++)
                    {
                        if (column + i <= fullyVisibleColumns)
                            continue;

                        newWidgetSpanOutsideVisibleColumn = true;
                        break;
                    }

                    // The newest widget won't cover up an existing row/column so lets
                    // return the specific row/column the widget can occupy
                    if (!newWidgetSpanOutsideVisibleColumn)
                        return new RowIndexColumnIndex(rowCount, column);
                }

                rowCount++;
            }
        }

        /// <summary>
        /// Recursively gets all the widgets in the path of the provided widgetHost into a list.
        /// </summary>
        /// <param name="widgetData">The widget data.</param>
        /// <param name="rowAndColumnPlacement">The row and column placement.</param>
        /// <param name="widgetsThatNeedToMove">The widgets that need to move.</param>
        /// <returns>List&lt;WidgetHost&gt;.</returns>
        private IEnumerable<WidgetHostData> GetWidgetMoveList(WidgetHostData widgetData, RowIndexColumnIndex rowAndColumnPlacement, List<WidgetHostData> widgetsThatNeedToMove)
        {
            if (widgetsThatNeedToMove == null)
                widgetsThatNeedToMove = new List<WidgetHostData>();

            var widgetsAtLocation = new List<WidgetHostData>();

            // If the widgetHost is the _draggingHost then we only need to get the direct widgets that occupy the
            // provided rowAndColumnPlacement
            if (widgetData == _draggingHostData)
            {
                widgetsAtLocation
                    .AddRange(WidgetAtLocation(widgetData.WidgetBase.RowSpanColumnSpan, rowAndColumnPlacement)
                        .Where(widgetAtLocationData => widgetAtLocationData != _draggingHostData)
                        .ToList());
            }
            else
            {
                // If we're a widget at the designated widgetHost we need to check how many spaces
                // we're moving and check each widget that could potentially be in those spaces
                var widgetRowMovementCount = rowAndColumnPlacement.Row - widgetData.WidgetBase.RowIndexColumnIndex.Row + 1;

                Debug.Assert(widgetData.WidgetBase.RowIndexColumnIndex.Row != null, "widgetData.WidgetBase.RowIndexColumnIndex.Row != null");

                for (var i = 0; i < widgetRowMovementCount; i++)
                {
                    widgetsAtLocation
                        .AddRange(WidgetAtLocation(widgetData.WidgetBase.RowSpanColumnSpan, new RowIndexColumnIndex(widgetData.WidgetBase.RowIndexColumnIndex.Row + i, rowAndColumnPlacement.Column))
                        .Where(widgetHostData => widgetHostData != widgetData && widgetHostData != _draggingHostData));
                }
            }

            // If there aren't any widgets at the location then just return the list we've been maintaining
            if (widgetsAtLocation.Count < 1)
                return widgetsThatNeedToMove.Distinct();

            // Since we have widgets at the designated location we need add to the list any widgets that
            // could potentially move as a result of the widgetHost movement
            for (var widgetAtLocationIndex = 0; widgetAtLocationIndex < widgetsAtLocation.Count; widgetAtLocationIndex++)
            {
                // If we're already tracking the widget then continue to the next
                if (widgetsThatNeedToMove.IndexOf(widgetsAtLocation[widgetAtLocationIndex]) >= 0)
                    continue;

                var widgetDataAtLocation = widgetsAtLocation[widgetAtLocationIndex];

                Debug.Assert(widgetDataAtLocation.WidgetBase.RowIndexColumnIndex.Column != null, "widgetDataAtLocation.WidgetBase.RowIndexColumnIndex.Column != null");

                // Need to recursively check if any widgets that are now in the place that this widget was also get moved down to
                // make room
                var proposedRowAndColumn = new RowIndexColumnIndex(rowAndColumnPlacement.Row + widgetData.WidgetBase.RowSpanColumnSpan.RowSpan,
                    (int)widgetDataAtLocation.WidgetBase.RowIndexColumnIndex.Column);

                // Get the widgets at the new location this one is moving to
                var currentWidgetsAtNewLocation =
                    WidgetAtLocation(widgetDataAtLocation.WidgetBase.RowSpanColumnSpan, proposedRowAndColumn)
                        .Where(widget =>
                        {
                            if (widget == widgetsAtLocation[widgetAtLocationIndex])
                                return false;

                            var widgetLocationIndex = widgetsAtLocation.IndexOf(widget);

                            // We check here if the potential widget is already scheduled to move and return false in that case
                            return widgetLocationIndex >= 0 && widgetLocationIndex <= widgetAtLocationIndex;
                        })
                        .ToArray();

                // If there are no widgets at the location then we can just add the widget and continue
                if (!currentWidgetsAtNewLocation.Any())
                {
                    widgetsThatNeedToMove.Add(widgetsAtLocation[widgetAtLocationIndex]);
                    GetWidgetMoveList(widgetsAtLocation[widgetAtLocationIndex], proposedRowAndColumn, widgetsThatNeedToMove);
                    continue;
                }

                // We need to get the max row span or size we're dealing with to offset the change
                var maxAdditionalRows = currentWidgetsAtNewLocation
                    .Select(widgetAtNewLocationData => widgetAtNewLocationData.WidgetBase.RowSpanColumnSpan.RowSpan)
                    .Max();

                // Add the widget to the list and move to the next item
                widgetsThatNeedToMove.Add(widgetsAtLocation[widgetAtLocationIndex]);
                GetWidgetMoveList(widgetsAtLocation[widgetAtLocationIndex],
                    new RowIndexColumnIndex(proposedRowAndColumn.Row + maxAdditionalRows, proposedRowAndColumn.Column),
                    widgetsThatNeedToMove);
            }

            // Return the list we've been maintaining
            return widgetsThatNeedToMove.Distinct();
        }

        /// <summary>
        /// Handles the Changed event of the ItemsSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="InvalidOperationException"></exception>
        private void ItemsSource_Changed(object sender, EventArgs args)
        {
            // Enforce the ItemsSource be of type ICollect<WidgetBase> as most of the code behind
            // relies on there being a WidgetBase as the item
            if (!(ItemsSource is ICollection<WidgetViewModelBase>))
                throw new InvalidOperationException(
                    $"{nameof(DashboardHost)} ItemsSource binding must be an ICollection of {nameof(WidgetViewModelBase)} type");
        }

        /// <summary>
        /// Finds the first empty spot and if a widget in a row down from the empty spot can be placed there
        /// it will automatically move that widget there. If it can't it will keep going until it finds a widget
        /// that can move. If it gets to the end and no more widgets can be moved it returns false indicated
        /// there aren't any more moves that can occur
        /// </summary>
        /// <returns><c>true</c> if a widget was moved, <c>false</c> otherwise.</returns>
        private bool ReArrangeFirstEmptySpot()
        {
            // Need to get all rows and columns taking up space
            var maxRowsAndColumn = GetMaxRowsAndColumns();

            // We loop through each row and column on the hunt for a blank space
            for (var mainRowIndex = 0; mainRowIndex < maxRowsAndColumn.Row; mainRowIndex++)
            {
                for (var mainColumnIndex = 0; mainColumnIndex < maxRowsAndColumn.Column; mainColumnIndex++)
                {
                    if (WidgetAtLocation(new RowSpanColumnSpan(1, 1), new RowIndexColumnIndex(mainRowIndex, mainColumnIndex)).Any())
                        continue;

                    // We need to peak to the next columns to see if they are blank as well
                    var additionalBlankColumnsOnMainRowIndex = 0;
                    for (var subColumnIndex = mainColumnIndex + 1; subColumnIndex < maxRowsAndColumn.Column; subColumnIndex++)
                    {
                        if (WidgetAtLocation(new RowSpanColumnSpan(1, 1),
                            new RowIndexColumnIndex(mainRowIndex, subColumnIndex)).Any())
                            break;

                        additionalBlankColumnsOnMainRowIndex++;
                    }

                    var stopChecking = false;

                    // Once we find an empty space we start looping from each row after the mainRowIndex using the same mainColumnIndex + additionalColumnNumber
                    // to find a widget that is a potential candidate to be moved up in rows
                    for (var subRowIndex = mainRowIndex + 1; subRowIndex < maxRowsAndColumn.Row; subRowIndex++)
                    {
                        for (var additionalColumnNumber = 0; additionalColumnNumber < additionalBlankColumnsOnMainRowIndex + 1; additionalColumnNumber++)
                        {
                            var secondaryWidgetAtLocation = WidgetAtLocation(new RowSpanColumnSpan(1, 1),
                                new RowIndexColumnIndex(subRowIndex, mainColumnIndex + additionalColumnNumber))
                                .ToArray();

                            // We can move on to next row if there is no widget in the space
                            if (!secondaryWidgetAtLocation.Any() || _draggingHostData != null &&
                                secondaryWidgetAtLocation.First() == _draggingHostData)
                                continue;

                            var possibleCandidateWidgetData = secondaryWidgetAtLocation.First();

                            // Once we find the next widget in the row we're checking, we need to see if that widget has the same RowIndex and ColumnIndex
                            // of the loops above. If it doesn't we can end looking for a replacement for this spot and find the next empty spot
                            if (possibleCandidateWidgetData.WidgetBase.RowIndexColumnIndex.Row != subRowIndex ||
                                possibleCandidateWidgetData.WidgetBase.RowIndexColumnIndex.Column != mainColumnIndex)
                            {
                                stopChecking = true;
                                break;
                            }

                            // Now we have a good candidate lets see if it'll fit at the location of the empty spot from mainRowIndex and
                            // mainColumnIndex + additionalColumnNumber
                            var mainRowColumnIndex = new RowIndexColumnIndex(mainRowIndex, mainColumnIndex);
                            var canSecondaryWidgetBePlacedMainRowColumn =
                                WidgetAtLocation(possibleCandidateWidgetData.WidgetBase.RowSpanColumnSpan, mainRowColumnIndex)
                                    .All(widget => widget == secondaryWidgetAtLocation.First());

                            if (!canSecondaryWidgetBePlacedMainRowColumn)
                            {
                                stopChecking = true;
                                break;
                            }

                            // Everything looks good and the widget can be placed in the empty spot
                            // We also return true here to say that a widget was moved due to this process being ran
                            var movingWidgetHost = _widgetHosts.FirstOrDefault(widgetHost =>
                                widgetHost.HostIndex == possibleCandidateWidgetData.HostIndex);

                            SetWidgetRowAndColumn(movingWidgetHost, mainRowColumnIndex, possibleCandidateWidgetData.WidgetBase.RowSpanColumnSpan);
                            return true;
                        }

                        if (stopChecking)
                            break;
                    }
                }
            }

            // No more widgets can be moved to fill in empty spots
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns><c>true</c> if a widget was moved, <c>false</c> otherwise.</returns>
        private bool ReArrangeToPreviewLocation()
        {
            foreach (var widgetHostData in _widgetHostsData.OrderBy(widgetHostData => widgetHostData.WidgetBase?.RowIndexColumnIndex.Row))
            {
                if (widgetHostData == _draggingHostData)
                    continue;

                for (int row = widgetHostData.WidgetBase.PreviewRowIndexColumnIndex.Row; row < widgetHostData.WidgetBase.RowIndexColumnIndex.Row; row++)
                {
                    var reArragnedIndex = new RowIndexColumnIndex(row, widgetHostData.WidgetBase.PreviewRowIndexColumnIndex.Column);

                    var widgetAlreadyThere = WidgetAtLocation(widgetHostData.WidgetBase.RowSpanColumnSpan, reArragnedIndex)
                        .Where(widgetHostDataThere => widgetHostData != widgetHostDataThere);

                    if (widgetAlreadyThere.Any())
                        continue;

                    var widgetHost = _widgetHosts.FirstOrDefault(widgetHost => widgetHost.HostIndex == widgetHostData.HostIndex);

                    SetWidgetRowAndColumn(widgetHost, reArragnedIndex, widgetHostData.WidgetBase.RowSpanColumnSpan);
                    widgetHostData.WidgetBase.RowIndexColumnIndex = reArragnedIndex;

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the excess canvas size that is no longer needed.
        /// </summary>
        private void RemoveExcessCanvasSize(Canvas canvas)
        {
            var rowAndColumnMax = GetMaxRowsAndColumns();

            if (canvas == CanvasEditingBackground)
            {
                var removeRectangles = CanvasEditingBackground.Children.OfType<Rectangle>()
                    .Where(child =>
                    {
                        var canvasRow = Canvas.GetTop(child) / _widgetHostMinimumSize.Height;
                        var canvasColumn = Canvas.GetLeft(child) / _widgetHostMinimumSize.Width;

                        return canvasRow >= rowAndColumnMax.Row || canvasColumn >= rowAndColumnMax.Column;
                    })
                    .ToArray();

                for (var i = removeRectangles.Length - 1; i >= 0; i--)
                    CanvasEditingBackground.Children.Remove(removeRectangles[i]);

                canvas.Width = rowAndColumnMax.Column * _widgetHostMinimumSize.Width;
                canvas.Height = rowAndColumnMax.Row * _widgetHostMinimumSize.Height;
                return;
            }

            var canvasHasChildren = canvas.Children.Count > 0;
            canvas.Width = (canvasHasChildren ? rowAndColumnMax.Column : 0) * _widgetHostMinimumSize.Width;
            canvas.Height = (canvasHasChildren ? rowAndColumnMax.Row : 0) * _widgetHostMinimumSize.Height;
        }

        /// <summary>
        /// Sets up the canvases within the DashboardHost.
        /// </summary>
        private void SetupCanvases()
        {
            // Get the canvas used to show where a dragging widget host will land when dragging ends
            var highlightWidgetCanvas = this.FindChildElementByName<Canvas>("HighlightWidgetCanvas");

            // Add a border control to the canvas which will be manually position manipulated when there is a dragging host
            _widgetDestinationHighlight = new Border
            {
                BorderBrush = Brushes.DeepSkyBlue,
                Background = Brushes.LightBlue,
                Opacity = 0.4,
                BorderThickness = new Thickness(2),
                Visibility = Visibility.Hidden
            };

            highlightWidgetCanvas.Children.Add(_widgetDestinationHighlight);

            WidgetsCanvasHost.Height = 0;
            WidgetsCanvasHost.Width = 0;

            // Add first rectangle for CanvasEditingBackground
            CanvasEditingBackground.Height = _widgetHostMinimumSize.Height;
            CanvasEditingBackground.Width = _widgetHostMinimumSize.Width;

            var rectangle = CreateGrayRectangleBackground();
            CanvasEditingBackground.Children.Add(rectangle);
            Canvas.SetTop(rectangle, 0);
            Canvas.SetLeft(rectangle, 0);
        }

        private void AnimateWidget(WidgetHost widgetHost, double fromLeft, double toLeft, double fromTop, double toTop, int durationFromTo)
        {
            DoubleAnimation animationLeft = new DoubleAnimation(fromLeft, toLeft, TimeSpan.FromMilliseconds(200 * (int)Math.Log10(durationFromTo * 10)));
            DoubleAnimation animationTop = new DoubleAnimation(fromTop, toTop, TimeSpan.FromMilliseconds(200 * (int)Math.Log10(durationFromTo * 10)));
            widgetHost.BeginAnimation(Canvas.LeftProperty, animationLeft);
            widgetHost.BeginAnimation(Canvas.TopProperty, animationTop);
        }

        /// <summary>
        /// Sets the widget row and column for the WidgetsCanvasHost and changes the RowIndex and ColumnIndex of
        /// the widgetHost's WidgetBase context.
        /// </summary>
        /// <param name="widgetHost">The widget host.</param>
        /// <param name="rowNumber">The row number.</param>
        /// <param name="columnNumber">The column number.</param>
        /// <param name="rowColumnSpan">The row column span.</param>
        private void SetWidgetRowAndColumn(
            WidgetHost widgetHost,
            RowIndexColumnIndex rowColumnIndex,
            RowSpanColumnSpan rowColumnSpan,
            bool withAnimate = true)
        {
            int rowNumber = rowColumnIndex.Row;
            int columnNumber = rowColumnIndex.Column;
            var widgetBase = widgetHost.DataContext as WidgetViewModelBase;

            Debug.Assert(widgetBase != null, nameof(widgetBase) + " != null");

            int originalRowNumber = widgetBase.RowIndexColumnIndex.Row;
            int originalColumnNumber = widgetBase.RowIndexColumnIndex.Column;
            widgetBase.RowIndexColumnIndex.Row = rowNumber;
            widgetBase.RowIndexColumnIndex.Column = columnNumber;
            int distanceFromTo = Math.Abs(rowNumber - originalRowNumber) + Math.Abs(columnNumber - originalColumnNumber);

            var maxRowsAndColumns = GetMaxRowsAndColumns();
            WidgetsCanvasHost.Height = maxRowsAndColumns.Row * _widgetHostMinimumSize.Height;
            WidgetsCanvasHost.Width = maxRowsAndColumns.Column * _widgetHostMinimumSize.Width;

            if (withAnimate)
            {
                AnimateWidget(widgetHost,
                    originalColumnNumber * _widgetHostMinimumSize.Width,
                    columnNumber * _widgetHostMinimumSize.Width,
                    originalRowNumber * _widgetHostMinimumSize.Height,
                    rowNumber * _widgetHostMinimumSize.Height,
                    distanceFromTo);
            }
            else
            {
                Canvas.SetTop(widgetHost, rowNumber * _widgetHostMinimumSize.Height);
                Canvas.SetLeft(widgetHost, columnNumber * _widgetHostMinimumSize.Width);
            }

            var columnCount = GetCanvasEditingBackgroundColumnCount();
            if (!EnableColumnLimit)
            {
                var rowCountForColumnAdditions = GetCanvasEditingBackgroundRowCount();
                while (true)
                {

                    if (columnCount - 1 >= columnNumber + rowColumnSpan.ColumnSpan - 1)
                        break;

                    AddCanvasEditingBackgroundColumn(rowCountForColumnAdditions, columnCount);
                }
            }

            var columnCountForRowAdditions = GetCanvasEditingBackgroundColumnCount();
            while (GetCanvasEditingBackgroundRowCount() - 1 < rowNumber + rowColumnSpan.RowSpan - 1)
            {
                var rowCount = GetCanvasEditingBackgroundRowCount();

                if (rowCount - 1 >= rowNumber + rowColumnSpan.RowSpan - 1)
                    break;

                AddCanvasEditingBackgroundRow(rowCount, columnCountForRowAdditions);
            }
        }

        /// <summary>
        /// Gets a list of WidgetHosts that occupy the provided rowAndColumnToCheck
        /// </summary>
        /// <param name="widgetSpan">The widget span.</param>
        /// <param name="rowAndColumnToCheck">The row and column to check.</param>
        /// <returns>List<WidgetHost></returns>
        private IEnumerable<WidgetHostData> WidgetAtLocation(RowSpanColumnSpan widgetSpan, RowIndexColumnIndex rowAndColumnToCheck)
        {
            // Need to see if a widget is already at the specific row and column
            return _widgetHostsData
                .Where(widgetData =>
                {
                    // If there is a specific widget there then return true
                    if (widgetData.WidgetBase.RowIndexColumnIndex.Row == rowAndColumnToCheck.Row &&
                        widgetData.WidgetBase.RowIndexColumnIndex.Column == rowAndColumnToCheck.Column)
                        return true;

                    // We need to look at the widgetHost being checked right now
                    // to see if its spans cover up a specific row/column
                    for (var i = 0; i < widgetData.WidgetBase.RowSpanColumnSpan.RowSpan; i++)
                    {
                        for (var j = 0; j < widgetData.WidgetBase.RowSpanColumnSpan.ColumnSpan; j++)
                        {
                            // If the span of the widgetHost covers up the next available
                            // row or column then we should consider this widget row/column
                            // already being used
                            if (widgetData.WidgetBase.RowIndexColumnIndex.Row + i == rowAndColumnToCheck.Row &&
                                widgetData.WidgetBase.RowIndexColumnIndex.Column + j == rowAndColumnToCheck.Column)
                                return true;

                            // Now, lets check how big the widget going to be added will be
                            // and see if this will cover up an already existing widget
                            // and if so then consider that row/column being used
                            for (var k = 0; k < widgetSpan.RowSpan; k++)
                            {
                                for (var l = 0; l < widgetSpan.ColumnSpan; l++)
                                {
                                    if (widgetData.WidgetBase.RowIndexColumnIndex.Row + i == rowAndColumnToCheck.Row + k &&
                                        widgetData.WidgetBase.RowIndexColumnIndex.Column + j == rowAndColumnToCheck.Column + l)
                                        return true;
                                }
                            }
                        }
                    }

                    return false;
                });
        }

        /// <summary>
        /// Handles the DragStarted event of a WidgetHost.
        /// </summary>
        /// <param name="widgetHost">The widget host.</param>
        private void WidgetHost_DragStarted(WidgetHost widgetHost)
        {
            if (!EditMode)
                return;

            try
            {
                // We need to make the DashboardHost allowable to have items dropped on it
                AllowDrop = true;

                _draggingHost = widgetHost;
                _draggingHostData =
                    _widgetHostsData.FirstOrDefault(widgetData => widgetData.HostIndex == _draggingHost.HostIndex);

                _widgetDestinationHighlight.Width =
                    _draggingHost.ActualWidth + _draggingHost.Margin.Left + _draggingHost.Margin.Right;
                _widgetDestinationHighlight.Height =
                    _draggingHost.ActualHeight + _draggingHost.Margin.Top + _draggingHost.Margin.Bottom;
                _widgetDestinationHighlight.Visibility = Visibility.Visible;

                Debug.Assert(_draggingHostData.WidgetBase.RowIndexColumnIndex.Row != null, "_draggingHostData.WidgetBase.RowIndexColumnIndex.Row != null");
                Debug.Assert(_draggingHostData.WidgetBase.RowIndexColumnIndex.Column != null, "_draggingHostData.WidgetBase.RowIndexColumnIndex.Column != null");
                Canvas.SetTop(_widgetDestinationHighlight, (int)_draggingHostData.WidgetBase.RowIndexColumnIndex.Row * _widgetHostMinimumSize.Height);
                Canvas.SetLeft(_widgetDestinationHighlight, (int)_draggingHostData.WidgetBase.RowIndexColumnIndex.Column * _widgetHostMinimumSize.Width);

                // Need to create the adorner that will be used to drag a control around the DashboardHost
                _draggingAdorner = new DragAdorner(_draggingHost, Mouse.GetPosition(_draggingHost));
                _draggingHost.GiveFeedback += DraggingHost_GiveFeedback;
                AdornerLayer.GetAdornerLayer(_draggingHost)?.Add(_draggingAdorner);

                // Need to hide the _draggingHost to give off the illusion that we're moving it somewhere
                _draggingHost.Visibility = Visibility.Hidden;
                _widgetHostsData.ForEach(widgetHostData =>
                {
                    widgetHostData.WidgetBase.PreviewRowIndexColumnIndex =
                        new RowIndexColumnIndex(widgetHostData.WidgetBase.RowIndexColumnIndex.Row, widgetHostData.WidgetBase.RowIndexColumnIndex.Column);
                });

                DragDrop.DoDragDrop(_draggingHost, new DataObject(_draggingHost), DragDropEffects.Move);
            }
            finally
            {
                // Need to cleanup after the DoDragDrop ends by setting back everything to its default state
                _draggingHost.GiveFeedback -= DraggingHost_GiveFeedback;
                Mouse.SetCursor(Cursors.Arrow);
                AllowDrop = false;
                AdornerLayer.GetAdornerLayer(_draggingHost)?.Remove(_draggingAdorner);
                _draggingHost.Visibility = Visibility.Visible;
                _draggingHostData = null;
                _draggingHost = null;
                _widgetDestinationHighlight.Visibility = Visibility.Hidden;
            }
        }

        private void WidgetHost_MouseOver(WidgetHost widgetHost)
        {
            if (!EditMode)
                return;

            try
            {
                // We need to make the DashboardHost allowable to have items dropped on it
                AllowDrop = true;

                _draggingHost = widgetHost;
                _draggingHostData =
                    _widgetHostsData.FirstOrDefault(widgetData => widgetData.HostIndex == _draggingHost.HostIndex);

                _widgetDestinationHighlight.Width =
                    _draggingHost.ActualWidth + _draggingHost.Margin.Left + _draggingHost.Margin.Right;
                _widgetDestinationHighlight.Height =
                    _draggingHost.ActualHeight + _draggingHost.Margin.Top + _draggingHost.Margin.Bottom;
                _widgetDestinationHighlight.Visibility = Visibility.Visible;

                Debug.Assert(_draggingHostData.WidgetBase.RowIndexColumnIndex.Row != null, "_draggingHostData.WidgetBase.RowIndexColumnIndex.Row != null");
                Debug.Assert(_draggingHostData.WidgetBase.RowIndexColumnIndex.Column != null, "_draggingHostData.WidgetBase.RowIndexColumnIndex.Column != null");

                // Need to create the adorner that will be used to drag a control around the DashboardHost
                var adorner = new ResizingAdorner(_draggingHost, _draggingHost.BorderThickness);
                AdornerLayer.GetAdornerLayer(_draggingHost)?.Add(adorner);

                // Need to hide the _draggingHost to give off the illusion that we're moving it somewhere
                //_draggingHost.Visibility = Visibility.Hidden;

                //DragDrop.DoDragDrop(_draggingHost, new DataObject(_draggingHost), DragDropEffects.Move);
            }
            finally
            {
                // Need to cleanup after the DoDragDrop ends by setting back everything to its default state
                Mouse.SetCursor(Cursors.Arrow);
                //AllowDrop = false;
                //_draggingHost.Visibility = Visibility.Visible;
                _draggingHostData = null;
                _draggingHost = null;
                //_widgetDestinationHighlight.Visibility = Visibility.Hidden;
            }
        }

        #endregion Private Methods
    }
}