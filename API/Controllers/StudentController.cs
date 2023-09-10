using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class StudentController : BaseController
    {
        private readonly DataContext _context;

        public StudentController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppStudent>>> GetUsers()
        {
            var students = await _context.Students.ToListAsync();
            return students;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppStudent>> GetUser(int id)
        {
            return await _context.Students.FindAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> PostTeacher(AppStudent newStudent)
        {
            if (ModelState.IsValid)
            {
                // Check if a student with the same NationalID or TeacherNumber already exists
                if (await StudentExists(newStudent.NationalID, newStudent.StudentNumber))
                {
                    return Conflict("A student with the same National ID or Student Number already exists.");
                }

                // Add the new student to the context
                _context.Students.Add(newStudent);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    // Handle database-related exceptions here
                    return StatusCode(500, "An error occurred while saving the student to the database.");
                }

                return CreatedAtAction("GetUser", new { id = newStudent.Id }, newStudent);
            }

            return BadRequest(ModelState);
        }

        private async Task<bool> StudentExists(string nationalID, string studentNumber)
        {
            return await _context.Students.AnyAsync(s => s.NationalID == nationalID || s.StudentNumber == studentNumber);
        }
    }
}