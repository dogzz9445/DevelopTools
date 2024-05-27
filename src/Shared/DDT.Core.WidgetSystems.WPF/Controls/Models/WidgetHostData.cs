using System;

namespace DDT.Core.WidgetSystems.WPF.Controls.Models;

/// <summary>
/// Represents widget host data
/// </summary>
public class WidgetHostData
{
    #region Public Properties

    /// <summary>
    /// Gets the index of the host.
    /// </summary>
    /// <value>The index of the host.</value>
    public int HostIndex { get; }

    /// <summary>
    /// Gets the widget base.
    /// </summary>
    /// <value>The widget base.</value>
    public WidgetHostViewModel WidgetBase { get; }

    ///// <summary>
    ///// Gets the widget spans.
    ///// </summary>
    ///// <value>The widget spans.</value>
    //public RowSpanColumnSpan WidgetSpans { get; }

    #endregion Public Properties

    #region Public Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="WidgetHostData"/> class.
    /// </summary>
    /// <param name="hostIndex">Index of the host.</param>
    /// <param name="widgetBase">The widget base.</param>
    /// <param name="widgetSpans">The widget spans.</param>
    public WidgetHostData(int hostIndex, WidgetHostViewModel widgetBase, RowSpanColumnSpan widgetSpans)
    {
        HostIndex = hostIndex;
        WidgetBase = widgetBase;
    }

    #endregion Public Constructors
}
