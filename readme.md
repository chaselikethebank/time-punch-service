A RESTful API for employee time tracking built with C# .NET Core, Entity Framework Core, and SQLite.

## Setup

```bash
dotnet build
dotnet run
```

API Documentation: `http://localhost:5132/swagger`

## Endpoints

### Employee Management

**GET /api/employees**

```
Returns: List of all employees
```

**POST /api/employees**

```json
{
  "firstName": "John",
  "lastName": "Doe",
  "employeeNumber": "EMP001",
  "email": "john@company.com"
}
```

### Time Punch Operations

**GET /api/employees/{employeeId}/timepunches**

```
Returns: List of time punches for employee
```

**POST /api/employees/{employeeId}/timepunches**

```json
{
  "punchType": 1,
  "notes": "Clocking in for the day"
}
```

**PUT /api/employees/{employeeId}/timepunches/{id}**

```json
{
  "punchType": 2,
  "notes": "Updated notes",
  "timestamp": "2025-08-28T12:00:00"
}
```

**DELETE /api/employees/{employeeId}/timepunches/{id}**

```
Deletes specified time punch
```

## Punch Types

- `1` - Clock In
- `2` - Clock Out
- `3` - Lunch
- `4` - Transfer

## Business Rules

- First punch must be Clock In (PunchType 1)
- Valid sequences:
    - In → Out, Lunch, Transfer
    - Out → In
    - Lunch → In
    - Transfer → Out

## Security Features

- Employee validation (404 if employee doesn't exist)
- Punch sequence validation (400 for invalid sequences)
- Employee isolation (can only access own punches)

## Error Responses

- `404` - "Employee not found"
- `404` - "Time punch not found"
- `400` - "Invalid punch sequence"

## Tech Stack

- .NET Core 8.0
- Entity Framework Core
- SQLite Database
- Swagger/OpenAPI Documentation