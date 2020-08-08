using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanDay.scheduling.API.Core.Models;
using PlanDay.scheduling.API.DataAccess;

namespace PlanDay.scheduling.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly SchedulingContext _context;

        public EmployeesController(SchedulingContext context)
        {
            _context = context;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return BadRequest();
            }

            if (_context.Employees.Any(e => e.Email.ToLower().Equals(employee.Email.ToLower())))
            {
                return ValidationProblem($"Email is unique, a employee with email: {employee.Email} already exists.");
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                Console.WriteLine(e);

                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
            }

            return NoContent();
        }

        // POST: api/Employees
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            if (string.IsNullOrEmpty(employee.FirstName) || string.IsNullOrEmpty(employee.LastName) ||
                string.IsNullOrEmpty(employee.Email))
            {
                return ValidationProblem("First name, last name and email cannot be empty/null");
            }

            if (!EmailAddresValidate(employee.Email))
            {
                return ValidationProblem("Email didn't pass validation. Use a proper email e.g test@testDomain.com");
            }

            if (_context.Employees.Any(e => e.Email.ToLower().Equals(employee.Email.ToLower())))
            {
                return ValidationProblem($"Email is unique, a employee with email: {employee.Email} already exists.");
            }


            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployee", new {id = employee.EmployeeId}, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Employee>> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var existingShifts = await _context.Shifts.Where(s => s.EmployeeId == id).ToListAsync();
            if(existingShifts.Count > 0)
                _context.Shifts.RemoveRange(existingShifts);

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok(employee);
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }

        private bool EmailAddresValidate(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}