using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ShiftsController : ControllerBase
    {
        private readonly SchedulingContext _context;

        public ShiftsController(SchedulingContext context)
        {
            _context = context;
        }

        // GET: api/Shifts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shift>>> GetShifts()
        {
            return await _context.Shifts.ToListAsync();
        }

        // GET: api/Shifts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Shift>> GetShift(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);

            if (shift == null)
            {
                return NotFound();
            }

            return shift;
        }

        // PUT: api/Shifts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShift(int id, Shift shift)
        {
            if (id != shift.ShiftId)
            {
                return BadRequest();
            }

            _context.Entry(shift).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                Console.WriteLine(e);

                if (!ShiftExists(id))
                {
                    return NotFound();
                }
            }


            return NoContent();
        }

        // POST: api/Shifts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Shift>> PostShift(Shift shift)
        {
            var sameDayShifts = await _context.Shifts
                .Where(s => s.EmployeeId == shift.EmployeeId &&
                            (s.Start.Date == shift.Start.Date || s.End.Date == shift.End.Date)).ToListAsync();

            foreach (var sameDayShift in sameDayShifts)
            {
                if (ShiftIsOverlapping(sameDayShift, shift))
                {
                    return ValidationProblem(
                        "An employee cannot work more than 1 shift at a time");
                }
            }

            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShift", new {id = shift.ShiftId}, shift);
        }

        // DELETE: api/Shifts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Shift>> DeleteShift(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift == null)
            {
                return NotFound();
            }

            _context.Shifts.Remove(shift);
            await _context.SaveChangesAsync();

            return shift;
        }

        [HttpGet("GetShiftsForEmployee/{id}")]
        public async Task<ActionResult<IEnumerable<Shift>>> GetAllShiftsForEmployee(int id)
        {
            if (!_context.Employees.Any(e => e.EmployeeId == id))
                return BadRequest($"No employee with id: {id} exists");

            return await _context.Shifts.Where(s => s.EmployeeId == id).ToListAsync();
        }

        [Route("SwapShifts")]
        [HttpPost]
        public async Task<IActionResult> SwapShifts(SwapShiftsModel model)
        {
            if (!ShiftExists(model.ShiftId1))
            {
                return BadRequest($"ShiftId: {model.ShiftId1} wasn't found in DB.");
            }

            if (!ShiftExists(model.ShiftId2))
            {
                return BadRequest($"ShiftId: {model.ShiftId2} wasn't found in DB.");
            }


            var shift1 = await _context.Shifts.FindAsync(model.ShiftId1);
            var shift2 = await _context.Shifts.FindAsync(model.ShiftId2);
            if (shift2.EmployeeId == shift1.EmployeeId)
            {
                return BadRequest("You cannot swap shifts between the same employee");
            }


            if (ShiftIsOverlapping(shift2, shift1))
            {
                return ValidationProblem(
                    "It's not allowed to swap shifts with someone who already has a conflicting shift.");
            }

            var id1 = shift1.EmployeeId;
            var id2 = shift2.EmployeeId;

            shift2.EmployeeId = id1;
            shift1.EmployeeId = id2;

            _context.Shifts.UpdateRange(new[] {shift1, shift2});

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                Console.WriteLine(e);
            }

            return Ok("Shifts was swapped successfully");
        }

        private bool ShiftIsOverlapping(Shift shift1, Shift shift2)
        {
            var overlap = shift1.Start < shift2.End && shift2.Start < shift1.End;

            return overlap;
        }

        private bool ShiftExists(int id)
        {
            return _context.Shifts.Any(e => e.ShiftId == id);
        }
    }
}