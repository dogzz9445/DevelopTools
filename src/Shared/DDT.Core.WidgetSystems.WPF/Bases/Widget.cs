using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DDT.Core.WidgetSystems.WPF.Bases;
public interface IWidgetCoreSettings
{
    string Name { get; }
}

public class WidgetSettings : Dictionary<string, object> { }

public enum MenuItemRole
{
    // Define all the roles here excluding the ones specified in the TypeScript code
}

public class WidgetMenuItem : MenuItem
{
    public MenuItemRole? Role { get; set; }
}

public class WidgetMenuItems : ReadOnlyCollection<WidgetMenuItem>
{
    public WidgetMenuItems(IList<WidgetMenuItem> list) : base(list) { }
}

public delegate WidgetMenuItems WidgetContextMenuFactory(string contextId, object contextData);

public interface IWidget<TSettings> : IEntity
{
    string Type { get; }
    IWidgetCoreSettings CoreSettings { get; }
    TSettings Settings { get; }
}

public interface IWidgetEnvCommon
{
    bool? IsPreview { get; }
}

public interface IWidgetEnvAreaShelf : IWidgetEnvCommon
{
    string Area { get; }
}

public interface IWidgetEnvAreaWorkflow : IWidgetEnvCommon
{
    string Area { get; }
    Guid ProjectId { get; }
    Guid WorkflowId { get; }
}

public interface IWidgetEnv : IWidgetEnvAreaShelf, IWidgetEnvAreaWorkflow
{
    // This interface can be used as a marker interface for WidgetEnvAreaShelf and WidgetEnvAreaWorkflow
}

public class WidgetInEnv<TSettings>
{
    public IWidgetEnv Env { get; set; }
    public IWidget<TSettings> Widget { get; set; }
}

public class Widget<TSettings> : IWidget<TSettings>
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public IWidgetCoreSettings CoreSettings { get; set; }
    public TSettings Settings { get; set; }

    public static IWidget<TSettings> CreateWidget<TSettings>(IWidgetType<TSettings> type, Guid id, string name)
    {
        return new Widget<TSettings>
        {
            Id = id,
            Type = type.Id.ToString(),
            CoreSettings = new WidgetCoreSettings { Name = name },
            Settings = type.CreateSettingsState()
        };
    }

    public static string GenerateWidgetName(string widgetTypeName, IEnumerable<string> usedNames)
    {
        // Implement the GenerateUniqueName method according to your requirements
        return GenerateUniqueName(widgetTypeName, usedNames);
    }

    public static IWidget<TSettings> UpdateWidgetSettings<TSettings>(IWidget<TSettings> widget, TSettings settings)
    {
        return new Widget<TSettings>
        {
            Id = widget.Id,
            Type = widget.Type,
            CoreSettings = widget.CoreSettings,
            Settings = settings
        };
    }

    public static IWidget<TSettings> UpdateWidgetCoreSettings<TSettings>(IWidget<TSettings> widget, IWidgetCoreSettings coreSettings)
    {
        return new Widget<TSettings>
        {
            Id = widget.Id,
            Type = widget.Type,
            CoreSettings = coreSettings,
            Settings = widget.Settings
        };
    }

    public static IWidgetEnvAreaShelf CreateWidgetEnv(IWidgetEnvAreaShelf envData)
    {
        var widgetEnv = new WidgetEnvAreaShelf
        {
            Area = envData.Area,
            IsPreview = envData.IsPreview
        };
        return widgetEnv;
    }

    public static IWidgetEnvAreaWorkflow CreateWidgetEnv(IWidgetEnvAreaWorkflow envData)
    {
        var widgetEnv = new WidgetEnvAreaWorkflow
        {
            Area = envData.Area,
            IsPreview = envData.IsPreview,
            ProjectId = envData.ProjectId,
            WorkflowId = envData.WorkflowId
        };
        return widgetEnv;
    }

    public static string GetWidgetDisplayName<TSettings>(IWidget<TSettings> widget = null, IWidgetType<TSettings> type = null)
    {
        if (widget != null && !string.IsNullOrEmpty(widget.CoreSettings.Name))
        {
            return widget.CoreSettings.Name;
        }
        if (type != null && !string.IsNullOrEmpty(type.Name))
        {
            return type.Name;
        }
        return string.Empty;
    }

    private static string GenerateUniqueName(string baseName, IEnumerable<string> usedNames)
    {
        // Implement the unique name generation logic
        return baseName; // Placeholder
    }
}

public class WidgetCoreSettings : IWidgetCoreSettings
{
    public string Name { get; set; }
}

public class WidgetEnvAreaShelf : IWidgetEnvAreaShelf
{
    public string Area { get; set; }
    public bool? IsPreview { get; set; }
}

public class WidgetEnvAreaWorkflow : IWidgetEnvAreaWorkflow
{
    public string Area { get; set; }
    public bool? IsPreview { get; set; }
    public Guid ProjectId { get; set; }
    public Guid WorkflowId { get; set; }
}
