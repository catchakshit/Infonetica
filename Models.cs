namespace Models;

public class State
{
    public string Id { get; init; } = default!;
    public bool IsInitial { get; set; }
    public bool IsFinal { get; set; }
    public bool Enabled { get; set; } = true;
}

public class Action
{
    public string Id { get; init; } = default!;
    public bool Enabled { get; set; } = true;
    public List<string> FromStates { get; set; } = new();
    public string ToState { get; set; } = default!;
}

public class WorkflowDefinition
{
    public string Id { get; init; } = default!;
    public List<State> States { get; set; } = new();
    public List<Action> Actions { get; set; } = new();
}

public class WorkflowInstance
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string DefinitionId { get; set; } = default!;
    public string CurrentStateId { get; set; } = default!;
    public List<string> History { get; set; } = new();
}
