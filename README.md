# Workflow Engine API

A minimal workflow engine API using ASP.NET Core. Define workflows, start instances, and execute actions to move between states using IDs only.

---

## Setup Instructions

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Code editor or IDE

### Running the Application

bash
git clone https://github.com/catchakshit/Infonetica
cd Infonetica
dotnet build
dotnet run


The API runs by default at http://localhost:5000.

---

## API Endpoints

### 1. Create Workflow

*POST* /workflows

Create a new workflow definition.

*Request body example:*

json
{
	"id": "workflow1",
	"states": [
		{ "id": "start", "isInitial": true, "isFinal": false, "enabled": true },
		{ "id": "mid", "isInitial": false, "isFinal": false, "enabled": true },
		{ "id": "end", "isInitial": false, "isFinal": true, "enabled": true }
	],
	"actions": [
		{
			"id": "toMid",
			"enabled": true,
			"fromStates": ["start"],
			"toState": "mid"
		},
		{
			"id": "toEnd",
			"enabled": true,
			"fromStates": ["start", "mid"],
			"toState": "end"
		}
	]
}


*Responses:*

- 200 OK – Workflow created.
- 400 Bad Request – Invalid workflow (duplicate ID, no initial state, invalid action states).

---

### 2. Get All Workflow Definition

*GET* /workflows

Get All workflows.

*Responses:*

- 200 OK – Workflow definition JSON.
- 404 Not Found – Not found.

---

### 3. Get Workflow Definition

*GET* /workflows/{id}

Get workflow by ID.

*Responses:*

- 200 OK – Workflow definition JSON.
- 404 Not Found – Not found.

---

### 4. Start Workflow Instance

*POST* /workflows/{id}/instances

Start new instance of workflow.

*Responses:*

- 200 OK – Returns instance with new ID and initial state.
- 400 Bad Request – Workflow not found or no enabled initial state.

---

### 5. Get All Workflow Instance

*GET* /instances

Get all instances.

*Responses:*

- 200 OK – Instance details (current state and history).
- 404 Not Found – Not found.

---

### 6. Get Workflow Instance

*GET* /instances/{id}

Get instance by ID.

*Responses:*

- 200 OK – Instance details (current state and history).
- 404 Not Found – Not found.

---

### 7. Execute Action on Instance

*POST* /instances/{id}/actions/{actionId}

Execute an action on instance to move state.

*Responses:*

- 200 OK – Action executed successfully.
- 400 Bad Request – Invalid instance or action, disabled action, invalid from-state, or already in final state.

---

## Data Model Highlights

- *State:*

  - id (string)
  - isInitial (bool)
  - isFinal (bool)
  - enabled (bool)

- *Action:*

  - id (string)
  - enabled (bool)
  - fromStates (list of state IDs)
  - toState (state ID)

- *WorkflowDefinition:*

  - id (string)
  - states (list of State)
  - actions (list of Action)

- *WorkflowInstance:*
  - id (string, GUID)
  - definitionId (string)
  - currentStateId (string)
  - history (list of action IDs)

---

## Persistence

Workflow instances persist in history.json file. Definitions and instances are in-memory.
Workflows persist in workflows.json file. Definitions and instances are in-memory.

---
