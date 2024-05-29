using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using DDT.Core.WidgetSystems.WPF.Controls.Models;

namespace DDT.Core.WidgetSystems.WPF.Controls;

public partial class WidgetViewModelBase : ObservableObject
{
    [ObservableProperty]
    private RowIndexColumnIndex? _rowIndexColumnIndex;

    [ObservableProperty]
    private RowIndexColumnIndex? _previewRowIndexColumnIndex;

    [ObservableProperty]
    private RowSpanColumnSpan? _rowSpanColumnSpan;

    [ObservableProperty]
    private RowSpanColumnSpan? _previewRowSpanColumnSpan;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _type;

    public WidgetViewModelBase()
    {
        RowIndexColumnIndex = new RowIndexColumnIndex(0, 0);
        PreviewRowIndexColumnIndex = new RowIndexColumnIndex(0, 0);
        RowSpanColumnSpan = new RowSpanColumnSpan(1, 1);
        PreviewRowSpanColumnSpan = new RowSpanColumnSpan(1, 1);
    }
}
