using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Crud_App.Data;
using Crud_App.Models;

namespace Crud_App.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        // LIST
        public async Task<IActionResult> Index(string searchString, string filter)
        {
            var studentsQuery = _context.Students
                .Include(s => s.Subject)
                .AsQueryable();

            // SEARCH
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim().ToLower();

                if (searchString.Length > 1)
                {
                    studentsQuery = studentsQuery.Where(s =>
                        s.StudentName.ToLower().Contains(searchString)
                    );
                }
            }

            // FILTER (PASS / FAIL)
            if (!string.IsNullOrEmpty(filter))
            {
                if (filter == "PASS")
                {
                    studentsQuery = studentsQuery.Where(s => s.Grade >= 75);
                }
                else if (filter == "FAIL")
                {
                    studentsQuery = studentsQuery.Where(s => s.Grade < 75);
                }
            }

            ViewBag.SearchString = searchString;
            ViewBag.Filter = filter;

            var students = await studentsQuery.ToListAsync();

            return View(students);
        }

        // CREATE - GET
        public async Task<IActionResult> Create()
        {
            var subjects = await _context.Subjects.ToListAsync();

            if (subjects == null || subjects.Count == 0)
            {
                TempData["Error"] = "Please add a subject first before creating a student.";
                return RedirectToAction("Index", "Subject");
            }

            ViewBag.Subjects = subjects;
            return View();
        }

        // CREATE - POST
        [HttpPost]
        public async Task<IActionResult> Create(Student student)
        {
            var exists = await _context.Students.AnyAsync(s =>
    s.StudentName.Trim().ToLower() == student.StudentName.Trim().ToLower() &&
    s.SubjectId == student.SubjectId
);

            if (exists)
            {
                ModelState.AddModelError("", "This student already exists for the selected subject.");
                ViewBag.Subjects = await _context.Subjects.ToListAsync();
                return View(student);
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // EDIT - GET
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Subjects = await _context.Subjects.ToListAsync();
            var student = await _context.Students.FindAsync(id);
            return View(student);
        }

        // EDIT - POST
        [HttpPost]
        public async Task<IActionResult> Edit(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // DELETE
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}