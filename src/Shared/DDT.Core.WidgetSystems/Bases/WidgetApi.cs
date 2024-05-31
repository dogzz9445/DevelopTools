using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DDT.Core.WidgetSystems.Bases.IWidgetApiModules;

namespace DDT.Core.WidgetSystems.Bases;

public interface IWidgetApiCommon
{
    Action<List<IActionBarItem>> UpdateActionBar { get; }
    Action<WidgetContextMenuFactory> SetContextMenuFactory { get; }
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
    public Action<List<IActionBarItem>> UpdateActionBar { get; }
    public Action<WidgetContextMenuFactory> SetContextMenuFactory { get; }

    public IClipboard Clipboard { get; }
    public IDataStorage DataStorage { get; }
    public IProcess Process { get; }
    public IShell Shell { get; }
    public ITerminal Terminal { get; }

    public WidgetApi(
        Action<List<IActionBarItem>> updateActionBar,
        Action<WidgetContextMenuFactory> setContextMenuFactory,
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
