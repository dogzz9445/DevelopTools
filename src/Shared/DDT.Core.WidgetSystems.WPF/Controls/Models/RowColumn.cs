using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDT.Core.WidgetSystems.WPF.Bases;

namespace DDT.Core.WidgetSystems.WPF.Controls.Models
{
    public struct RowColumn : IWidgetLayoutItemRect
    {
        public RowIndexColumnIndex Index { get; set; }
        public RowSpanColumnSpan Span { get; set; }

        public int X => throw new NotImplementedException();

        public int Y => throw new NotImplementedException();

        public int W => throw new NotImplementedException();

        public int H => throw new NotImplementedException();

        public RowColumn(RowIndexColumnIndex index, RowSpanColumnSpan span)
        {
            Index = index;
            Span = span;
        }

        public static bool operator ==(RowColumn left, RowColumn right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RowColumn left, RowColumn right)
        {
            return !(left == right);
        }
    }
}
