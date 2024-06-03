using CommunityToolkit.Mvvm.ComponentModel;
using DDT.Core.WidgetSystems.Bases;
using DDT.Core.WidgetSystems.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    public void LoadWidgetsFromDLL(string pathDLL)
    {
        Assembly a = Assembly.LoadFrom(pathDLL);
        var types = a.GetTypes().Where(t => typeof(WidgetViewModelBase).IsAssignableFrom(t));
        List<WidgetGenerator> widgets = new List<WidgetGenerator>();

        foreach (var type in types)
        {
            System.Reflection.MemberInfo info = type;
            var attributes = info.GetCustomAttributes(true);
            if (attributes[0] is WidgetGeneratorAttribute)
            {
                var value = ((WidgetGeneratorAttribute)attributes[0]).WidgetGenerator;
                widgets.Add(value);
            }
        }

        RegisterWidgets(widgets);
    }
}
