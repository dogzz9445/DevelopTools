using DDT.Core.WidgetSystems.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.Bases;

public delegate TWidgetBase CreateWidgetViewModelBase<TWidgetBase>();

public class WidgetGenerator
{
    #region Private Fields

    private readonly CreateWidgetViewModelBase<WidgetViewModelBase> _createWidget;

    #endregion Private Fields

    #region Public Properties

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; }

    /// <summary>
    /// Gets the MenuPath
    /// </summary>
    public string MenuPath { get; }

    public int MenuOrder { get; }

    #endregion Public Properties

    #region Public Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Widget"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="description">The description.</param>
    /// <param name="createWidget">The create widget.</param>
    public WidgetGenerator(
        string name,
        string description,
        string menuPath,
        int menuOrder,
        CreateWidgetViewModelBase<WidgetViewModelBase> createWidget)
    {
        Name = name;
        MenuPath = menuPath;
        Description = description;
        MenuOrder = menuOrder;
        _createWidget = createWidget;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    /// Creates the widget.
    /// </summary>
    /// <returns>WidgetBase.</returns>
    public WidgetViewModelBase CreateWidget()
    {
        return _createWidget.Invoke();
    }

    #endregion Public Methods
}
