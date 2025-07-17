using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using Models;
using Stores;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<WorkflowStore>();
var app = builder.Build();

var store = app.Services.GetRequiredService<WorkflowStore>();

// Create workflow
app.MapPost("/workflows", (WorkflowDefinition def) =>
{
    if (store.Workflows.ContainsKey(def.Id))
        return Results.BadRequest("Workflow ID exists");

    if (def.States.Count(s => s.IsInitial) != 1)
        return Results.BadRequest("Must have exactly one initial state");

    var stateIds = def.States.Select(s => s.Id).ToHashSet();

    foreach (var a in def.Actions)
        if (!stateIds.Contains(a.ToState) || a.FromStates.Any(f => !stateIds.Contains(f)))
            return Results.BadRequest($"Invalid states in action {a.Id}");

    store.Workflows[def.Id] = def;
    store.SaveWorkflows();
    return Results.Ok("Workflow created");
});

app.MapGet("/workflows", () =>
{
    return Results.Ok(store.Workflows.Values);
});

// Get workflow
app.MapGet("/workflows/{id}", (string id) =>
    store.Workflows.TryGetValue(id, out var def) ? Results.Ok(def) : Results.NotFound());

// Start instance
app.MapPost("/workflows/{id}/instances", (string id) =>
{
    if (!store.Workflows.TryGetValue(id, out var def))
        return Results.BadRequest("No such workflow");

    var initial = def.States.FirstOrDefault(s => s.IsInitial && s.Enabled);
    if (initial == null)
        return Results.BadRequest("No enabled initial state");

    var instance = new WorkflowInstance
    {
        DefinitionId = id,
        CurrentStateId = initial.Id
    };

    store.Instances[instance.Id] = instance;
    store.SaveInstances();
    return Results.Ok(instance);
});

// Get instance
app.MapGet("/instances/{id}", (string id) =>
    store.Instances.TryGetValue(id, out var i) ? Results.Ok(i) : Results.NotFound());

app.MapGet("/instances", () =>
{
    return Results.Ok(store.Instances.Values);
});

// Execute action
app.MapPost("/instances/{id}/actions/{actionId}", (string id, string actionId) =>
{
    if (!store.Instances.TryGetValue(id, out var inst))
        return Results.BadRequest("Instance not found");

    var def = store.Workflows[inst.DefinitionId];
    var act = def.Actions.FirstOrDefault(a => a.Id == actionId);

    if (act == null || !act.Enabled)
        return Results.BadRequest("Invalid or disabled action");

    if (!act.FromStates.Contains(inst.CurrentStateId))
        return Results.BadRequest("Action not valid from current state");

    if (def.States.First(s => s.Id == inst.CurrentStateId).IsFinal)
        return Results.BadRequest("Already in final state");

    inst.CurrentStateId = act.ToState;
    inst.History.Add(actionId);
    store.SaveInstances();
    return Results.Ok("Action executed");
});

app.Run();
