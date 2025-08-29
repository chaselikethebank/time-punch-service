using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimePunchService.Data;
using TimePunchService.Models;

namespace TimePunchService.Controllers
{
    [ApiController]
    [Route("api/employees/{employeeId}/timepunches")]
    public class TimePunchController : ControllerBase
    {
        private readonly TimePunchDbContext _context;

        public TimePunchController(TimePunchDbContext context)
        {
            _context = context;
        }

        [HttpGet("~/api/employees")]
        public async Task<ActionResult<List<Employee>>> GetAllEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            return Ok(employees);
        }

        [HttpPost("~/api/employees")]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateEmployee), new { id = employee.Id }, employee);
        }

        [HttpDelete("~/api/employees/{id}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound("Employee not found");

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<List<TimePunch>>> GetTimePunches(int employeeId)
        {
            if (!await EmployeeExists(employeeId))
            {
                return NotFound("Employee not found");
            }

            var timePunches = await _context
                .TimePunches.Where(tp => tp.EmployeeId == employeeId)
                .ToListAsync();

            return Ok(timePunches);
        }

        [HttpPost]
        public async Task<ActionResult<TimePunch>> CreateTimePunch(
            int employeeId,
            TimePunch timePunch
        )
        {
            if (!await EmployeeExists(employeeId))
            {
                return NotFound("Employee not found");
            }

            if (!await IsValidPunchSequence(employeeId, timePunch.PunchType))
            {
                return BadRequest("Invalid punch sequence");
            }

            timePunch.EmployeeId = employeeId;
            timePunch.Timestamp = DateTime.Now;

            _context.TimePunches.Add(timePunch);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTimePunches),
                new { employeeId = employeeId },
                timePunch
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TimePunch>> UpdateTimePunch(
            int employeeId,
            int id,
            TimePunch updatedTimePunch
        )
        {
            if (!await EmployeeExists(employeeId))
            {
                return NotFound("Employee not found");
            }

            var timePunch = await _context.TimePunches.FirstOrDefaultAsync(tp =>
                tp.Id == id && tp.EmployeeId == employeeId
            );

            if (timePunch == null)
                return NotFound("Time punch not found");

            timePunch.PunchType = updatedTimePunch.PunchType;
            timePunch.Notes = updatedTimePunch.Notes;
            timePunch.Timestamp = updatedTimePunch.Timestamp;

            await _context.SaveChangesAsync();
            return Ok(timePunch);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTimePunch(int employeeId, int id)
        {
            if (!await EmployeeExists(employeeId))
            {
                return NotFound("Employee not found");
            }

            var timePunch = await _context.TimePunches.FirstOrDefaultAsync(tp =>
                tp.Id == id && tp.EmployeeId == employeeId
            );

            if (timePunch == null)
                return NotFound("Time punch not found");

            _context.TimePunches.Remove(timePunch);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> EmployeeExists(int employeeId)
        {
            return await _context.Employees.AnyAsync(e => e.Id == employeeId);
        }

        private async Task<bool> IsValidPunchSequence(int employeeId, PunchType newPunchType)
        {
            var lastPunch = await _context
                .TimePunches.Where(tp => tp.EmployeeId == employeeId)
                .OrderByDescending(tp => tp.Timestamp)
                .FirstOrDefaultAsync();

            if (lastPunch == null)
                return newPunchType == PunchType.In;

            return newPunchType switch
            {
                PunchType.In => lastPunch.PunchType == PunchType.Out
                    || lastPunch.PunchType == PunchType.Lunch,
                PunchType.Out => lastPunch.PunchType == PunchType.In
                    || lastPunch.PunchType == PunchType.Transfer,
                PunchType.Lunch => lastPunch.PunchType == PunchType.In,
                PunchType.Transfer => lastPunch.PunchType == PunchType.In,
                _ => false,
            };
        }

        [HttpGet("~/api/employees/{id}/status")]
        public async Task<ActionResult<object>> GetEmployeeStatus(int id)
        {
            if (!await EmployeeExists(id))
            {
                return NotFound("Employee not found");
            }

            var lastPunch = await _context
                .TimePunches.Where(tp => tp.EmployeeId == id)
                .OrderByDescending(tp => tp.Timestamp)
                .FirstOrDefaultAsync();

            if (lastPunch == null)
            {
                return Ok(
                    new
                    {
                        employeeId = id,
                        currentState = "Not Started",
                        lastPunchType = (PunchType?)null,
                        lastPunchTime = (DateTime?)null,
                    }
                );
            }

            var currentState = lastPunch.PunchType switch
            {
                PunchType.In => "Currently punched in",
                PunchType.Out => "Currently punched out",
                PunchType.Lunch => "Currently on Lunch",
                PunchType.Transfer => "Currently in Transfer",
                _ => "Unknown",
            };

            return Ok(
                new
                {
                    employeeId = id,
                    currentState = currentState,
                    lastPunchType = lastPunch.PunchType,
                    lastPunchTime = lastPunch.Timestamp,
                }
            );
        }
    }
}
