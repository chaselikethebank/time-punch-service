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

        [HttpGet]
        public async Task<ActionResult<List<TimePunch>>> GetTimePunches(int employeeId)
        {
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
            var timePunch = await _context.TimePunches.FirstOrDefaultAsync(tp =>
                tp.Id == id && tp.EmployeeId == employeeId
            );

            if (timePunch == null)
                return NotFound();

            timePunch.PunchType = updatedTimePunch.PunchType;
            timePunch.Notes = updatedTimePunch.Notes;
            timePunch.Timestamp = updatedTimePunch.Timestamp;

            await _context.SaveChangesAsync();
            return Ok(timePunch);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTimePunch(int employeeId, int id)
        {
            var timePunch = await _context.TimePunches.FirstOrDefaultAsync(tp =>
                tp.Id == id && tp.EmployeeId == employeeId
            );

            if (timePunch == null)
                return NotFound();

            _context.TimePunches.Remove(timePunch);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
