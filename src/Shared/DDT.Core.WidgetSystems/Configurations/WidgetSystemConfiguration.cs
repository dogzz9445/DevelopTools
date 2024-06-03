using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.Configurations;

public class WidgetSystemConfigurationOption
{
    public const string Section = "WidgetSystem";

    public string WidgetSystemFilename { get; set; }
}

public class WidgetSystemConfigurationSource(WidgetSystemConfigurationOption? option) : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder) => new WidgetSystemConfigurationProvider(option);
}

public class WidgetSystemConfigurationProvider(WidgetSystemConfigurationOption? option) : ConfigurationProvider
{
    public override void Load()
    {

    }
}

public static class WidgetSystemConfigurationBuilderExtensions
{
    public static IConfigurationBuilder MigrationIfNeeded(this IConfigurationBuilder builder)
    {
        var tempConfiguration = builder.Build();
        var option = new WidgetSystemConfigurationOption();

        // If version changed
        tempConfiguration.GetSection(WidgetSystemConfigurationOption.Section).Bind(option);

        builder.Add(new WidgetSystemConfigurationSource(option));
        if (!string.IsNullOrEmpty(option.WidgetSystemFilename))
            builder.AddJsonFile(option.WidgetSystemFilename);
        return builder;
    }
}
