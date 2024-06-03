using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using DDT.Core.WidgetSystems.Controls.Models;

namespace DDT.Core.WidgetSystems.Controls;

public partial class WidgetViewModelBase : ObservableRecipient
{
    private readonly IServiceProvider _services;

    [ObservableProperty]
    private Guid _uid;

    [ObservableProperty]
    private RowIndexColumnIndex? _rowIndexColumnIndex;

    [ObservableProperty]
    private RowIndexColumnIndex? _previewRowIndexColumnIndex;

    [ObservableProperty]
    private RowSpanColumnSpan? _rowSpanColumnSpan;

    [ObservableProperty]
    private RowSpanColumnSpan? _previewRowSpanColumnSpan;

    [ObservableProperty]
    private string _widgetTitle;

    [ObservableProperty]
    private bool? _visibleTitle;

    [ObservableProperty]
    private string _type;

    [ObservableProperty]
    private int? _rowIndex;
    [ObservableProperty]
    private int? _columnIndex;
    [ObservableProperty]
    private int? _rowSpan;
    [ObservableProperty]
    private int? _columnSpan;
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private bool? _isSelecting;
    [ObservableProperty]
    private bool? _isDragging;
    [ObservableProperty]
    private bool? _isResizing;
    [ObservableProperty]
    private bool? _isEditing;

    public WidgetViewModelBase()
    {
        RowIndexColumnIndex = new RowIndexColumnIndex(0, 0);
        PreviewRowIndexColumnIndex = new RowIndexColumnIndex(0, 0);
        RowSpanColumnSpan = new RowSpanColumnSpan(1, 1);
        PreviewRowSpanColumnSpan = new RowSpanColumnSpan(1, 1);
        RowIndex = 0;
        ColumnIndex = 0;
        RowSpan = 1;
        ColumnSpan = 1;
        Name = "Widget";
        IsSelecting = false;
        IsDragging = false;
        IsResizing = false;
        IsEditing = false;
        VisibleTitle = true;
        WidgetTitle = "Widget";
    }

    public WidgetViewModelBase(IServiceProvider services) : this()
    {
        _services = services;
    }
}
