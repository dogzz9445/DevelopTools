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
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using DragDropEffects = System.Windows.DragDropEffects;
using Point = System.Windows.Point;

namespace DDT.Core.WidgetSystems.WPF;

/// <summary>
/// Interaction logic for WidgetBox.xaml
/// </summary>
public partial class WidgetBox : System.Windows.Controls.ListBox
{
    Point startPoint;

    public WidgetBox()
    {
        InitializeComponent();

        MouseDown += List_MouseDown;
        MouseMove += List_MouseMove;
    }

    void List_MouseDown(object sender, MouseButtonEventArgs e)
    {
        startPoint = e.GetPosition(null);
    }

    void List_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        // Get the current mouse position
        Point mousePos = e.GetPosition(null);
        Vector diff = startPoint - mousePos;

        if (e.LeftButton == MouseButtonState.Pressed
            && (
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance
                || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
        {
            // Get the dragged ListViewItem
            var widgets = SelectedItems.Cast<WidgetItem>().ToArray();
            //var items = string.Join(", ", SelectedItems.Cast<Widget>().ToArray());
            var texts = widgets.Select(w => w.Name).ToArray();

            // Initialize the drag & drop operation
            DataObject dragData = new DataObject(DataFormats.UnicodeText, widgets);
            DragDrop.DoDragDrop(this, dragData, DragDropEffects.Copy);
        }
    }
}
