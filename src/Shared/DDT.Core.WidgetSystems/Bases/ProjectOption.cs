﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDT.Core.WidgetSystems.Utils;

namespace DDT.Core.WidgetSystems.Bases;

public interface IProjectSettings
{
    string Name { get; }
}

public class ProjectSettings : IProjectSettings
{
    public string Name { get; set; }
}

public interface IProjectOption : IEntity
{
    IProjectSettings Settings { get; }
    List<Guid> WorkflowIds { get; }
    Guid CurrentWorkflowId { get; }
}

public class ProjectOption : IProjectOption
{
    public Guid Id { get; set; }
    public IProjectSettings Settings { get; set; }
    public List<Guid> WorkflowIds { get; set; }
    public Guid CurrentWorkflowId { get; set; }

    public static ProjectOption CreateProject(Guid id, string projectName)
    {
        return new ProjectOption
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

    public static ProjectOption UpdateProjectSettings(ProjectOption project, IProjectSettings settings)
    {
        return new ProjectOption
        {
            Id = project.Id,
            Settings = settings,
            WorkflowIds = project.WorkflowIds,
            CurrentWorkflowId = project.CurrentWorkflowId
        };
    }

    public static ProjectOption UpdateProjectWorkflows(ProjectOption project, List<Guid> workflowIds)
    {
        return new ProjectOption
        {
            Id = project.Id,
            Settings = project.Settings,
            WorkflowIds = workflowIds,
            CurrentWorkflowId = project.CurrentWorkflowId
        };
    }
}
