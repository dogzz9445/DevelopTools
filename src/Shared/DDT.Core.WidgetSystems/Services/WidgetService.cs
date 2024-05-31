using CommunityToolkit.Mvvm.ComponentModel;
using DDT.Core.WidgetSystems.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.Services;

public interface IWidgetService
{
    void RegisterWidgets(List<WidgetGenerator> widgets);
    List<WidgetGenerator> GetAvailableWidgets();
}

public class WidgetService : IWidgetService
{
    /// <summary>
    /// Gets or sets the available widgets.
    /// </summary>
    /// <value>The available widgets.</value>
    private readonly List<WidgetGenerator> _availableWidgets = new List<WidgetGenerator>();

    public void RegisterWidgets(List<WidgetGenerator> widgets)
    {
        _availableWidgets.AddRange(widgets);
    }
    public List<WidgetGenerator> GetAvailableWidgets()
    {
        return _availableWidgets;
    }
}
