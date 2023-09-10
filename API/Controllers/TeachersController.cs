using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class TeachersController : BaseController
    {
        private readonly DataContext _context;

        public TeachersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppTeacher>>> GetUsers()
        {
            var teachers = await _context.Teachers.ToListAsync();
            return teachers;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppTeacher>> GetUser(int id)
        {
            return await _context.Teachers.FindAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> PostTeacher(AppTeacher newTeacher)
        {
            if (ModelState.IsValid)
            {
                // Check if a teacher with the same NationalID or TeacherNumber already exists
                if (await TeacherExists(newTeacher.NationalID, newTeacher.TeacherNumber))
                {
                    return Conflict("A teacher with the same National ID or Teacher Number already exists.");
                }

                // Add the new teacher to the context
                _context.Teachers.Add(newTeacher);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    // Handle database-related exceptions here
                    return StatusCode(500, "An error occurred while saving the teacher to the database.");
                }

                return CreatedAtAction("GetUser", new { id = newTeacher.Id }, newTeacher);
            }

            return BadRequest(ModelState);
        }

        private async Task<bool> TeacherExists(string nationalID, string teacherNumber)
        {
            return await _context.Teachers.AnyAsync(t => t.NationalID == nationalID || t.TeacherNumber == teacherNumber);
        }
    }
}