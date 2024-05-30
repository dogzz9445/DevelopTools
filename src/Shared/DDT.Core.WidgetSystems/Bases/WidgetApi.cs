using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DDT.Core.WidgetSystems.Bases.IWidgetApiModules;

namespace DDT.Core.WidgetSystems.Bases;


public delegate void UpdateActionBarDelegate(List<IActionBarItem> actionBarItems);
public delegate void SetContextMenuFactoryDelegate(WidgetContextMenuFactory factory);

public interface IWidgetApiCommon
{
    UpdateActionBarDelegate UpdateActionBar { get; }
    SetContextMenuFactoryDelegate SetContextMenuFactory { get; }
}

public interface IClipboard
{
    Task WriteBookmark(string title, string url);
    Task WriteText(string text);
}

public interface IDataStorage
{
    Task<string?> GetText(string key);
    Task SetText(string key, string value);
    Task<object?> GetJson(string key);
    Task SetJson(string key, object value);
    Task Remove(string key);
    Task Clear();
    Task<IEnumerable<string>> GetKeys();
}

public interface IProcess
{
    Process GetProcessInfo();
}

public interface IShell
{
    Task OpenApp(string appPath, string[]? args = null);
    Task OpenExternalUrl(string url);
    Task<string> OpenPath(string path);
}

public interface ITerminal
{
    void ExecCmdLines(IEnumerable<string> cmdLines, string? cwd = null);
}

public interface IWidgetApiModules
{
    public IClipboard Clipboard { get; }
    public IDataStorage DataStorage { get; }
    public IProcess Process { get; }
    public IShell Shell { get; }
    public ITerminal Terminal { get; }
}

public enum WidgetApiModuleName
{
    Clipboard,
    DataStorage,
    Process,
    Shell,
    Terminal
}

public class WidgetApi : IWidgetApiCommon, IWidgetApiModules
{
    public UpdateActionBarDelegate UpdateActionBar { get; }
    public SetContextMenuFactoryDelegate SetContextMenuFactory { get; }

    public IClipboard Clipboard { get; }
    public IDataStorage DataStorage { get; }
    public IProcess Process { get; }
    public IShell Shell { get; }
    public ITerminal Terminal { get; }

    public WidgetApi(
        UpdateActionBarDelegate updateActionBar,
        SetContextMenuFactoryDelegate setContextMenuFactory,
        IClipboard clipboard,
        IDataStorage dataStorage,
        IProcess process,
        IShell shell,
        ITerminal terminal)
    {
        UpdateActionBar = updateActionBar ?? throw new ArgumentNullException(nameof(updateActionBar));
        SetContextMenuFactory = setContextMenuFactory ?? throw new ArgumentNullException(nameof(setContextMenuFactory));
        Clipboard = clipboard ?? throw new ArgumentNullException(nameof(clipboard));
        DataStorage = dataStorage ?? throw new ArgumentNullException(nameof(dataStorage));
        Process = process ?? throw new ArgumentNullException(nameof(process));
        Shell = shell ?? throw new ArgumentNullException(nameof(shell));
        Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
    }
}

//public delegate IWidgetApiCommon WidgetApiCommonFactory(Guid widgetId, UpdateActionBarDelegate updateActionBarHandler, SetContextMenuFactoryDelegate setContextMenuFactoryHandler);
//public delegate IWidgetApiModules.IWidgetApiModule WidgetApiModuleFactory(Guid widgetId);
//public interface IWidgetApiModuleFactories
//{
//    Dictionary<WidgetApiModuleName, WidgetApiModuleFactory> ModuleFactories { get; }
//}

//public delegate WidgetApi WidgetApiFactory(Guid widgetId, UpdateActionBarDelegate updateActionBarHandler, SetContextMenuFactoryDelegate setContextMenuFactoryHandler, IEnumerable<WidgetApiModuleName> availableModules);

//public class WidgetApiFactory
//{
//    private readonly WidgetApiCommonFactory _commonFactory;
//    private readonly IWidgetApiModuleFactories _moduleFactories;

//    public WidgetApiFactory(WidgetApiCommonFactory commonFactory, IWidgetApiModuleFactories moduleFactories)
//    {
//        _commonFactory = commonFactory ?? throw new ArgumentNullException(nameof(commonFactory));
//        _moduleFactories = moduleFactories ?? throw new ArgumentNullException(nameof(moduleFactories));
//    }

//    public WidgetApi CreateWidgetApi(Guid widgetId, UpdateActionBarDelegate updateActionBarHandler, SetContextMenuFactoryDelegate setContextMenuFactoryHandler, IEnumerable<WidgetApiModuleName> availableModules)
//    {
//        var common = _commonFactory(widgetId, updateActionBarHandler, setContextMenuFactoryHandler);
//        var modules = new Dictionary<WidgetApiModuleName, WidgetApiModules.WidgetApiModule>();
//        foreach (var featName in availableModules)
//        {
//            modules[featName] = _moduleFactories.ModuleFactories[featName](widgetId);
//        }
//        return new WidgetApi(updateActionBarHandler, setContextMenuFactoryHandler, modules.IClipboard, modules.DataStorage, modules.Process, modules.Shell, modules.Terminal);
//    }
//}

//public interface WidgetSettingsApi<TSettings>
//{
//    void UpdateSettings(TSettings newSettings);

//    public interface Dialog
//    {
//        void ShowAppManager();
//        Task<OpenDialogResult> ShowOpenFileDialog(OpenFileDialogConfig cfg);
//        Task<OpenDialogResult> ShowOpenDirDialog(OpenDirDialogConfig cfg);
//    }

//    Dialog Dialogs { get; }
//}