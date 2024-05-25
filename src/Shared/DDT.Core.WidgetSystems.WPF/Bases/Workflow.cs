using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.WPF.Bases;


public interface IWorkflowSettings
{
    string Name { get; }
}

public class WorkflowSettings : IWorkflowSettings
{
    public string Name { get; set; }
}

public interface IWorkflow : IEntity
{
    List<WidgetLayoutItem> Layout { get; }
    IWorkflowSettings Settings { get; }
}

public class Workflow : IWorkflow
{
    public Guid Id { get; set; }
    public List<WidgetLayoutItem> Layout { get; set; }
    public IWorkflowSettings Settings { get; set; }

    public static Workflow CreateWorkflow(Guid id, string name)
    {
        return new Workflow
        {
            Id = id,
            Layout = new List<WidgetLayoutItem>(),
            Settings = new WorkflowSettings
            {
                Name = name
            }
        };
    }

    public static string GenerateWorkflowName(List<string> usedNames)
    {
        return NameGenerationHelper.GenerateUniqueName("Workflow", usedNames);
    }

    public static Workflow UpdateWorkflowSettings(Workflow workflow, IWorkflowSettings settings)
    {
        return new Workflow
        {
            Id = workflow.Id,
            Layout = workflow.Layout,
            Settings = settings
        };
    }

    public static Workflow UpdateWorkflowLayout(Workflow workflow, List<WidgetLayoutItem> layout)
    {
        return new Workflow
        {
            Id = workflow.Id,
            Layout = layout,
            Settings = workflow.Settings
        };
    }
}
