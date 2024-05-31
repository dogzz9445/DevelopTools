using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDT.Core.WidgetSystems.Utils;

namespace DDT.Core.WidgetSystems.Bases;


public interface IWorkflowSettings
{
    string Name { get; }
}

public class WorkflowSettings : IWorkflowSettings
{
    public string Name { get; set; }
}

public interface IWorkflowOption : IEntity
{
    List<WidgetLayoutItem> Layout { get; }
    IWorkflowSettings Settings { get; }
}

public class WorkflowOption : IWorkflowOption
{
    public Guid Id { get; set; }
    public List<WidgetLayoutItem> Layout { get; set; }
    public IWorkflowSettings Settings { get; set; }

    public static WorkflowOption CreateWorkflow(Guid id, string name)
    {
        return new WorkflowOption
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

    public static WorkflowOption UpdateWorkflowSettings(WorkflowOption workflow, IWorkflowSettings settings)
    {
        return new WorkflowOption
        {
            Id = workflow.Id,
            Layout = workflow.Layout,
            Settings = settings
        };
    }

    public static WorkflowOption UpdateWorkflowLayout(WorkflowOption workflow, List<WidgetLayoutItem> layout)
    {
        return new WorkflowOption
        {
            Id = workflow.Id,
            Layout = layout,
            Settings = workflow.Settings
        };
    }
}
