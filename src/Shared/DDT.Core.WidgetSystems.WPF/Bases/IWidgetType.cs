using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.WPF.Bases;

public interface IWidgetTypeComponent
{
    // Define the members of WidgetTypeComponent as per your requirements
}

public delegate TSettings CreateSettingsState<TSettings>();

public interface IWidgetType<TSettings> : IEntity
{
    string Name { get; }
    string Icon { get; }
    (int w, int h) MinSize { get; }
    string Description { get; }
    bool? Maximizable { get; }
    IWidgetTypeComponent WidgetComp { get; }
    IWidgetTypeComponent SettingsEditorComp { get; }
    CreateSettingsState<TSettings> CreateSettingsState { get; }
    IEnumerable<string> RequiresApi { get; }
    IEnumerable<string> RequiresState { get; }
}
