using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.Bases;

public class AppSettings
{
    public string Name { get; }
    public string ExecPath { get; }
    public string CmdArgs { get; }

    public AppSettings(string name, string execPath, string cmdArgs)
    {
        Name = name;
        ExecPath = execPath;
        CmdArgs = cmdArgs;
    }
}

public interface IApp : IEntity
{
    public AppSettings Settings { get; set; }
}

public static class AppFactroy
{
    public static T CreateApp<T>(Guid id, string appName) where T : IApp, new()
    {
        return new T()
        {
            Id = id,
            Settings = new AppSettings(appName, "", ""),
        };
    }

    public static T UpdateAppSettings<T>(T app, AppSettings settings) where T : IApp, new()
    {
        return new T()
        {
            Id = app.Id,
            Settings = settings,
        };
    }

    public static T DuplicateApp<T>(T app, Guid newId, string newName) where T : IApp, new()
    {
        return new T()
        {
            Id = newId,
            Settings = new AppSettings(newName, app.Settings.ExecPath, app.Settings.CmdArgs),
        };
    }

    public static string GenerateAppName(string[] usedNames)
    {
        return NameGenerationHelper.GenerateUniqueName("App", usedNames.ToList());
    }
}
