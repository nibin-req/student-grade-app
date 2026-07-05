using Microsoft.AspNetCore.Mvc;
using Crud_App.Data;
using Crud_App.Models;
using Microsoft.EntityFrameworkCore;

namespace Crud_App.Controllers
{
    public class SubjectController : Controller
    {
        private readonly AppDbContext _context;

        public SubjectController(AppDbContext context)
        {
            _context = context;
        }

        // LIST
        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subjects.ToListAsync();
            return View(subjects);
        }

        // CREATE - GET
        public IActionResult Create()
        {
            return View();
        }

        // CREATE - POST
        [HttpPost]
        public async Task<IActionResult> Create(Subject subject)
        {
            // ❌ Case 1: Empty or whitespace
            if (string.IsNullOrWhiteSpace(subject.SubjectName))
            {
                ModelState.AddModelError("", "Please enter subject name.");
                return View(subject);
            }

            // ❌ Case 2: Duplicate (case-insensitive)
            var exists = await _context.Subjects.AnyAsync(s =>
                s.SubjectName.Trim().ToLower() == subject.SubjectName.Trim().ToLower()
            );

            if (exists)
            {
                ModelState.AddModelError("", "Subject already exists.");
                return View(subject);
            }

            // ✅ SAVE
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        // EDIT - GET
        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            return View(subject);
        }

        // EDIT - POST
        [HttpPost]
        public async Task<IActionResult> Edit(Subject subject)
        {
            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // DELETE
        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}