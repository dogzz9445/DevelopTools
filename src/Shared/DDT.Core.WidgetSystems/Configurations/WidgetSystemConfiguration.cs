using DDT.Core.WidgetSystems.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.Configurations;

public class WidgetSystemConfigurationOption
{
    public const string Section = "WidgetSystem";
    public const string DefaultWidgetSystemFilename = "widgets.json";

    public bool UseAppDataPath { get; set; } = false;
    public string WidgetSystemFilename { get; set; }
}

public class WidgetSystemConfigurationSource(WidgetSystemConfigurationOption? option) : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder) => new WidgetSystemConfigurationProvider(builder, option);
}

public class WidgetSystemConfigurationProvider(IConfigurationBuilder builder, WidgetSystemConfigurationOption? option) : ConfigurationProvider
{
    public override void Load()
    {
        string filename = null;
        if (string.IsNullOrEmpty(option.WidgetSystemFilename))
            option.WidgetSystemFilename = WidgetSystemConfigurationOption.DefaultWidgetSystemFilename;
        else

        if (string.IsNullOrEmpty(filename))
            filename = WidgetSystemConfigurationOption.DefaultWidgetSystemFilename;

        string filepath = null;
        if (option.UseAppDataPath)
            filepath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DDT_Widgets");
        else
            filepath = "";

        string fullFilename = Path.Combine(filepath, filename);

        if (!File.Exists(fullFilename)) 
        {
            string jsonString = JsonSerializer.Serialize(new WidgetSystemOption());
            File.WriteAllText(fullFilename, jsonString);
        }

        builder.AddJsonFile(fullFilename);
    }
}

public static class WidgetSystemConfigurationBuilderExtensions
{
    public static IConfigurationBuilder MigrationIfNeeded(this IConfigurationBuilder builder)
    {
        var tempConfiguration = builder.Build();
        var option = new WidgetSystemConfigurationOption();

        // If version changed
        tempConfiguration
            .GetSection(WidgetSystemConfigurationOption.Section)
            .Bind(option);

        builder.Add(new WidgetSystemConfigurationSource(option));
        return builder;
    }
}
