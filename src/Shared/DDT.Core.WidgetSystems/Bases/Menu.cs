using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.Bases;

public static class MenuConstants
{
    public static readonly string[] MenuItemRoles =
    {
    "undo", "redo", "cut", "copy", "paste", "pasteAndMatchStyle", "delete", "selectAll",
    "toggleSpellChecker", "togglefullscreen", "window", "minimize", "close", "help", "about",
    "services", "hide", "hideOthers", "unhide", "quit", "startSpeaking", "stopSpeaking",
    "appMenu", "fileMenu", "editMenu", "viewMenu", "shareMenu", "recentDocuments",
    "toggleTabBar", "selectNextTab", "selectPreviousTab", "mergeAllWindows",
    "clearRecentDocuments", "moveTabToNewWindow", "windowMenu", "reload", "forceReload",
    "toggleDevTools"
};

    public static readonly string[] MenuItemTypes =
    {
    "normal", "separator", "submenu", "checkbox", "radio"
};
}

public enum MenuItemRole
{
    Undo, Redo, Cut, Copy, Paste, PasteAndMatchStyle, Delete, SelectAll, ToggleSpellChecker,
    ToggleFullscreen, Window, Minimize, Close, Help, About, Services, Hide, HideOthers,
    Unhide, Quit, StartSpeaking, StopSpeaking, AppMenu, FileMenu, EditMenu, ViewMenu,
    ShareMenu, RecentDocuments, ToggleTabBar, SelectNextTab, SelectPreviousTab,
    MergeAllWindows, ClearRecentDocuments, MoveTabToNewWindow, WindowMenu, Reload,
    ForceReload, ToggleDevTools
}

public enum MenuItemType
{
    Normal, Separator, Submenu, Checkbox, Radio
}

public interface IMenuItemCommon<T>
    where T : IMenuItemCommon<T>
{
    string Accelerator { get; }
    string Label { get; }
    MenuItemRole? Role { get; }
    MenuItemType? Type { get; }
    string Icon { get; }
    bool? Enabled { get; }
    IReadOnlyList<T> Submenu { get; }
}

public interface IMenuItemIpc : IMenuItemCommon<IMenuItemIpc>
{
    int? ActionId { get; }
}

public interface IMenuItem : IMenuItemCommon<IMenuItem>
{
    Task DoAction { get; }
}

public class MenuItemIpc : IMenuItemIpc
{
    public string Accelerator { get; set; }
    public string Label { get; set; }
    public MenuItemRole? Role { get; set; }
    public MenuItemType? Type { get; set; }
    public string Icon { get; set; }
    public bool? Enabled { get; set; }
    public IReadOnlyList<IMenuItemIpc> Submenu { get; set; }
    public int? ActionId { get; set; }
}

public class MenuItem : IMenuItem
{
    public string Accelerator { get; set; }
    public string Label { get; set; }
    public MenuItemRole? Role { get; set; }
    public MenuItemType? Type { get; set; }
    public string Icon { get; set; }
    public bool? Enabled { get; set; }
    public IReadOnlyList<IMenuItem> Submenu { get; set; }
    public Task DoAction { get; set; }
}
