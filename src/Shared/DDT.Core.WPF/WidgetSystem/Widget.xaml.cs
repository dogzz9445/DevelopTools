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

namespace DDT.Core.WPF.WidgetSystem;

/// <summary>
/// Interaction logic for Widget.xaml
/// </summary>
public partial class Widget : ListBoxItem
{
    public string Name { get; set; } = "A";
    protected bool _deferSelection = false;

    public Widget()
    {
        InitializeComponent();

    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1 && IsSelected)
        {
            // the user may start a drag by clicking into selected items
            // delay destroying the selection to the Up event
            _deferSelection = true;
        }
        else
        {
            base.OnMouseLeftButtonDown(e);
        }
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        if (_deferSelection)
        {
            try
            {
                base.OnMouseLeftButtonDown(e);
            }
            finally
            {
                _deferSelection = false;
            }
        }
        base.OnMouseLeftButtonUp(e);
    }

    protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
    {
        // abort deferred Down
        _deferSelection = false;
        base.OnMouseLeave(e);
    }
}
