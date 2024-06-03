using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DDT.Core.WidgetSystems.Bases;
using DDT.Core.WidgetSystems.Configurations;
using DDT.Core.WidgetSystems.Contracts.Services;
using Microsoft.Extensions.Configuration;

namespace DDT.Core.WidgetSystems.Services;

public interface IWidgetSystemService
{
    bool TryGetWidgetOption<T>(Guid guid, out T option);
}

public class WidgetSystemOption
{
    public const string Section = "WidgetSystem";
    public const string WidgetSystemFilename = "widgets.json";

    public ProjectOption currentProject { get; set; }
    public WorkflowOption currentWorkflow { get; set; }
}

//{
//    dragDrop: {},
//    editMode: false,
//    menuBar: true,
//    appConfig:
//{
//mainHotkey: 'CmdOrCtrl+Shift+F',
//      uiTheme: 'dark'
//    },
//    apps:
//{
//appIds: []
//    },
//    copy:
//{
//widgets:
//    {
//    entities: { },
//        list: []
//      },
//      workflows:
//    {
//    entities: { },
//        list: []
//      }
//},
//    modalScreens:
//{
//data:
//    {
//    appManager:
//        {
//        currentAppId: '',
//          deleteAppIds: null,
//          apps: null,
//          appIds: null
//        },
//        applicationSettings:
//        {
//        appConfig: null
//        },
//        projectManager:
//        {
//        currentProjectId: '',
//          deleteProjectIds: null,
//          projects: null,
//          projectIds: null,
//          duplicateProjectIds: null
//        },
//        widgetSettings:
//        {
//        widgetInEnv: null
//        },
//        workflowSettings:
//        {
//        workflow: null
//        },
//      },
//      order: []
//    },
//    palette:
//{
//widgetTypeIds: []
//    },
//    projectSwitcher:
//{
//currentProjectId: '',
//      projectIds: []
//    },
//    shelf:
//{
//widgetList: []
//    },
//    worktable: { }
//  }

public class WidgetSystemService : IWidgetSystemService
{
    public Dictionary<Guid, string> GuidToEntityJson = new();

    public WidgetSystemService(IConfiguration configuration)
    {
        var option = new WidgetSystemOption();
        configuration.GetSection(WidgetSystemOption.Section).Bind(option);
    }

    public bool TryGetWidgetOption<T>(Guid guid, out T option)
    {
        option = default;
        if (!GuidToEntityJson.TryGetValue(guid, out var json))
            return false;
        option = JsonSerializer.Deserialize<T>(json);
        return true;
    }

    private async Task InitializeAsync(string path, Task<AuthenticationInfo?> requestAuthentication)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile(path)
            .AddEntityConfiguration();
        var tempConfig = builder.Build();
        var entityConfiguration = new EntityConfigurationOption();
        tempConfig.GetSection(EntityConfigurationOption.Section).Bind(entityConfiguration);
        var dbContext = new EntityConfigurationContext(entityConfiguration);
        dbContext.Database.EnsureCreated();

        if (File.Exists(entityConfiguration.SqliteDatabaseFile))
        {
            var authenticationInfo = await requestAuthentication;
        }
        //foreach (var databaseFile in File.Exists(BloggingContext.DEFAULTDBFILE)
        //    ? new[] { BloggingContext.DEFAULTDBFILE, newDatabaseFile }// If the default existing database exists
        //    : new[] { newDatabaseFile, BloggingContext.DEFAULTDBFILE }// The default existing database will be a copy of the new database from the first test
        //    )
        //{
        //    // Prepare the test
        //    if (databaseFile == BloggingContext.DEFAULTDBFILE)
        //    {
        //        // Use the existing database
        //        if (!File.Exists(databaseFile))
        //        {
        //            // Copy the previously created new database as default existing database
        //            Console.WriteLine("Use \"{0}\" as default existing database \"{1}\"", newDatabaseFile, databaseFile);
        //            File.Copy(newDatabaseFile, databaseFile);
        //        }
        //        Console.WriteLine("Update existing database \"{0}\"", databaseFile);
        //    }
        //    else
        //    {
        //        // Create a new database
        //        if (File.Exists(databaseFile))
        //        {
        //            // Delete the previously created database (ensure we're creating a new database)
        //            Console.WriteLine("Delete existing database \"{0}\"", databaseFile);
        //            File.Delete(databaseFile);
        //        }
        //        Console.WriteLine("Test creating new database \"{0}\"", databaseFile);
        //    }

        //    // Initialize the database context (ensure the database is created, if it's a new database)
        //    using var db = new BloggingContext(databaseFile);
        //    if (databaseFile == newDatabaseFile) db.Database.EnsureCreated();

        //    // Add a blog URI
        //    db.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
        //    Console.WriteLine("{0} records saved to database", db.SaveChanges());

        //    // Display all blog URIs from the current database
        //    Console.WriteLine("All {0} blogs in database:", db.Blogs.Count());
        //    foreach (var blog in db.Blogs) Console.WriteLine(" - #{0} {1}", blog.BlogId, blog.Url);
        //}

        //return builder.Build();
    }
}
