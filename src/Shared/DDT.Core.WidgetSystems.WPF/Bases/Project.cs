using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.WPF.Bases;

public interface IProjectSettings
{
    string Name { get; }
}

public class ProjectSettings : IProjectSettings
{
    public string Name { get; set; }
}

public interface IProject : IEntity
{
    IProjectSettings Settings { get; }
    List<Guid> WorkflowIds { get; }
    Guid CurrentWorkflowId { get; }
}

public class Project : IProject
{
    public Guid Id { get; set; }
    public IProjectSettings Settings { get; set; }
    public List<Guid> WorkflowIds { get; set; }
    public Guid CurrentWorkflowId { get; set; }

    public static Project CreateProject(Guid id, string projectName)
    {
        return new Project
        {
            Id = id,
            Settings = new ProjectSettings
            {
                Name = projectName
            },
            WorkflowIds = new List<Guid>(),
            CurrentWorkflowId = Guid.Empty
        };
    }

    public static string GenerateProjectName(List<string> usedNames)
    {
        return NameGenerationHelper.GenerateUniqueName("Project", usedNames);
    }

    public static Project UpdateProjectSettings(Project project, IProjectSettings settings)
    {
        return new Project
        {
            Id = project.Id,
            Settings = settings,
            WorkflowIds = project.WorkflowIds,
            CurrentWorkflowId = project.CurrentWorkflowId
        };
    }

    public static Project UpdateProjectWorkflows(Project project, List<Guid> workflowIds)
    {
        return new Project
        {
            Id = project.Id,
            Settings = project.Settings,
            WorkflowIds = workflowIds,
            CurrentWorkflowId = project.CurrentWorkflowId
        };
    }
}
