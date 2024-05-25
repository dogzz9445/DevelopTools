using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.WPF.Controls.Models;

/// <summary>
/// Represents a Row and Column position
/// </summary>
public class RowIndexColumnIndex
{
    #region Public Properties

    /// <summary>
    /// Gets the column.
    /// </summary>
    /// <value>The column.</value>
    public int Column { get; set; }

    /// <summary>
    /// Gets the row.
    /// </summary>
    /// <value>The row.</value>
    public int Row { get; set; }

    #endregion Public Properties

    #region Public Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RowAndColumn"/> class.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    public RowIndexColumnIndex(int row, int column)
    {
        Row = row;
        Column = column;
    }

    #endregion Public Constructors
}
