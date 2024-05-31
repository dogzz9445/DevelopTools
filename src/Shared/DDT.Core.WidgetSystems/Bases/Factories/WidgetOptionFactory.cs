using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.Bases.Factories;

public static class WidgetOptionFactory
{
    public static IWidgetOption<TSettings> CreateWidget<TSettings>(IWidgetType<TSettings> type, Guid id, string name)
    {
        return new WidgetOption<TSettings>
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

    public static IWidgetOption<TSettings> UpdateWidgetSettings<TSettings>(IWidgetOption<TSettings> widget, TSettings settings)
    {
        return new WidgetOption<TSettings>
        {
            Id = widget.Id,
            Type = widget.Type,
            CoreSettings = widget.CoreSettings,
            Settings = settings
        };
    }

    public static WidgetOption<TSettings> UpdateWidgetCoreSettings<TSettings>(IWidgetOption<TSettings> widget, IWidgetCoreSettings coreSettings)
    {
        return new WidgetOption<TSettings>
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

    public static string GetWidgetDisplayName<TSettings>(IWidgetOption<TSettings> widget = null, IWidgetType<TSettings> type = null)
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