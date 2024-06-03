using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DDT.Apps.DDTOrganizer.Configurations;

public class AppOptions
{
    public const string Section = "WidgetSystems:App";

    /// <summary>
    /// Main Hot Key
    /// </summary>
    public string? MainHotKey { get; set; }

    /// <summary>
    /// Ui Theme
    /// </summary>
    public string? UiTheme { get; set; }
}
