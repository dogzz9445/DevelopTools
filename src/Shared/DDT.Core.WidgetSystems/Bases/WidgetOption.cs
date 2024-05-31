using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DDT.Core.WidgetSystems.Bases;

public interface IWidgetCoreSettings
{
    string Name { get; }
}

public class WidgetSettings : Dictionary<string, object> { }

public class WidgetMenuItem : MenuItem
{
    public MenuItemRole? Role { get; set; }
}

public class WidgetMenuItems : ReadOnlyCollection<WidgetMenuItem>
{
    public WidgetMenuItems(IList<WidgetMenuItem> list) : base(list) { }
}

public delegate WidgetMenuItems WidgetContextMenuFactory(string contextId, object contextData);

public interface IWidgetOption<TSettings> : IEntity
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
    public IWidgetOption<TSettings> Widget { get; set; }
}

public class WidgetOption<TSettings> : IWidgetOption<TSettings>
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public IWidgetCoreSettings CoreSettings { get; set; }
    public TSettings Settings { get; set; }
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
