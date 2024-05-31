using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.Bases.Factories;

public class WidgetApiOptionFactory
{
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