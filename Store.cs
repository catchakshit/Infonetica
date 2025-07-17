using System.Text.Json;
using Models;

namespace Stores;

public class WorkflowStore
{
    public Dictionary<string, WorkflowDefinition> Workflows { get; private set; } = new();
    public Dictionary<string, WorkflowInstance> Instances { get; private set; } = new();

    private const string HistoryFile = "history.json";
    private const string WorkflowsFile = "workflows.json";

    public WorkflowStore()
    {
        if (File.Exists(WorkflowsFile))
        {
            var workflowsJson = File.ReadAllText(WorkflowsFile);
            var loadedWorkflows = JsonSerializer.Deserialize<Dictionary<string, WorkflowDefinition>>(workflowsJson);
            if (loadedWorkflows != null) Workflows = loadedWorkflows;
        }

        if (File.Exists(HistoryFile))
        {
            var instancesJson = File.ReadAllText(HistoryFile);
            var loadedInstances = JsonSerializer.Deserialize<Dictionary<string, WorkflowInstance>>(instancesJson);
            if (loadedInstances != null) Instances = loadedInstances;
        }
    }

    public void SaveInstances()
    {
        var json = JsonSerializer.Serialize(Instances, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(HistoryFile, json);
    }

    public void SaveWorkflows()
    {
        var json = JsonSerializer.Serialize(Workflows, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(WorkflowsFile, json);
    }
}
