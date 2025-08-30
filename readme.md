This doc serves as the requirements -> API design and schema 
## Resources needed
- Employees -> these will be the people punching in 
- Punch Types -> for in, out, lunch and transfer 
- Time Punches -> the actual stamp for when someone punches time, will probably need a timestamp, note, type and emp id
## Relationships 
- Employee has many TimePunch
- TimePunch belongs to Employee
- TimePunch has one PunchType

## Models

### Employee Model
- `Id` (int)  
- FirstName (string)  
- LastName (string)  
- EmployeeNumber (int)
- `Email` (string)  
- TimePunches (generic, of type T, list, TimePunches object)

### TimePunch
- `Id` (int)  
- `EmployeeId` (int) FK -> Employee
- `Timestamp` (DateTime) 
- `PunchType` (enum) 
- `Notes` (string) — optional note for the punch
-  Employee (nav to related employee)

### PunchType (enum)
Enumerates possible punch types:
- `In`
- `Out`
- `Lunch`
- `Transfer`

## CRUD -> REST APIs
### Employees

- **GET /api/employees**  
  Returns a list of all employees.  
  **Response:** 200 OK, JSON array of employee objects  

- **POST /api/employees**  
  Creates a new employee.  
  **Request Body:** JSON `{ "name": string, "email": string, ... }`  
  **Response:** 201 Created, JSON object of created employee  
  **Validation:** 400 Bad Request if required fields are missing  

- **DELETE /api/employees/{id}**  
  Deletes an employee by ID.  
  **Response:** 204 No Content  
  **Validation:** 404 Not Found if employee does not exist  

- **GET /api/employees/{id}/status**  
  Returns the current status of an employee based on last punch (spot check as I am testing validation / business logic for fraud / accidental punching).  
  **Response:** 200 OK, JSON { "employeeId": int, "currentState": string, "lastPunchType": PunchType?, "lastPunchTime": DateTime? }  
  **Validation:** 404 Not Found if employee does not exist  

## TimePunches

- **GET /api/employees/{employeeId}/timepunches**  
  Retrieves all punches for a specific employee.  
  **Response:** 200 OK, JSON array of time punches  
  **Validation:** 404 Not Found if employee does not exist  

- **POST /api/employees/{employeeId}/timepunches**  
  Creates a punch for a specific employee.  
  **Request Body:** JSON { "punchType": int, "notes": string }
  **Response:** 201 Created, JSON object of created punch  
  **Validation:** 404 Not Found if employee does not exist  
  **Validation:** 400 Bad Request if punch sequence is invalid  

- **PUT /api/employees/{employeeId}/timepunches/{id}**  
  Updates a specific punch for an employee.  
  **Request Body:** JSON { "punchType": int, "notes": string, "timestamp": DateTime }
  **Response:** 200 OK, JSON object of updated punch  
  **Validation:** 404 Not Found if employee or punch does not exist  

- **DELETE /api/employees/{employeeId}/timepunches/{id}**  
  Deletes a specific punch for an employee.  
  **Response:** 204 No Content  
  **Validation:** 404 Not Found if employee or punch does not exist  
