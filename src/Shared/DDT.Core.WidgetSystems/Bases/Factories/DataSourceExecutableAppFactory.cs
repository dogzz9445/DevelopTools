using DDT.Core.WidgetSystems.Utils;

namespace DDT.Core.WidgetSystems.Bases.Factories;

public static class DataSourceExecutableAppFactory
{

    public static T CreateExecutableAppDataSource<T>(Guid id, string appName) where T : IDataSourceExecutableApp, new()
    {
        return new T()
        {
            Id = id,
            Settings = new DataSourceExecutableAppSettings(appName, "", ""),
        };
    }

    public static T UpdateExecutableAppDataSourceSettings<T>(T app, DataSourceExecutableAppSettings settings) where T : IDataSourceExecutableApp, new()
    {
        return new T()
        {
            Id = app.Id,
            Settings = settings,
        };
    }

    public static T DuplicateExecutableAppDataSource<T>(T app, Guid newId, string newName) where T : IDataSourceExecutableApp, new()
    {
        return new T()
        {
            Id = newId,
            Settings = new DataSourceExecutableAppSettings(newName, app.Settings.ExecPath, app.Settings.CmdArgs),
        };
    }

    public static string GenerateExecutableAppDataSourceName(string[] usedNames)
    {
        return NameGenerationHelper.GenerateUniqueName("ExecutableAppDataSource", usedNames.ToList());
    }
}
