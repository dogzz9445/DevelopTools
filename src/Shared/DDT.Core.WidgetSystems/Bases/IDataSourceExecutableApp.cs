using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDT.Core.WidgetSystems.Utils;

namespace DDT.Core.WidgetSystems.Bases;

public class DataSourceExecutableAppSettings
{
    public string Name { get; }
    public string ExecPath { get; }
    public string CmdArgs { get; }

    public DataSourceExecutableAppSettings(string name, string execPath, string cmdArgs)
    {
        Name = name;
        ExecPath = execPath;
        CmdArgs = cmdArgs;
    }
}

public interface IDataSourceExecutableApp : IEntity
{
    public DataSourceExecutableAppSettings Settings { get; set; }
}
