using CustomIdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomIdentityApp.Controllers
{
    [Authorize(Roles = "Администратор, Модератор")]
    public class IndicatorsController : Controller
    {
        private readonly ApplicationContext _context;

        public IndicatorsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Indicators
        public async Task<IActionResult> Index()
        {
            return View(await _context.Indicator.Include(i => i.Department).ToListAsync());
        }

        // GET: Indicators/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var indicator = await _context.Indicator.FirstOrDefaultAsync(m => m.Id == id);
            if (indicator == null)
            {
                return NotFound();
            }

            return View(indicator);
        }

        // GET: Indicators/Create
        public IActionResult Create()
        {
            Department department = GetDepartmentByUser();

            ViewBag.s = department;

            if (department == null)
                ViewBag.Departments = new SelectList(_context.Department.ToList(), "Id", "Name");
            else
                ViewBag.Departments = new SelectList(new List<Department>() { department }, "Id", "Name");


            return View();
        }


        public Department GetDepartmentByUser()
        {
            //получаем текущего юзера
            var user = _context.Users.Where(w => w.Email == User.Identity.Name).Include(i => i.Department).FirstOrDefault();

            Department departament = user.Department;

            return departament;
        }

        // POST: Indicators/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,DepartmentId")] Indicator indicator)
        {
            if (ModelState.IsValid)
            {
                _context.Add(indicator);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(indicator);
        }

        // GET: Indicators/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Department department = GetDepartmentByUser();

            ViewBag.s = department;

            if (department == null)
                ViewBag.Departments = new SelectList(_context.Department.ToList(), "Id", "Name");
            else
                ViewBag.Departments = new SelectList(new List<Department>() { department }, "Id", "Name");

            if (id == null)
            {
                return NotFound();
            }

            var indicator = await _context.Indicator.FindAsync(id);

            if (indicator == null)
            {
                return NotFound();
            }

            return View(indicator);
        }

        // POST: Indicators/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DepartmentId")] Indicator indicator)
        {
            if (id != indicator.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(indicator);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IndicatorExists(indicator.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(indicator);
        }

        // GET: Indicators/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var indicator = await _context.Indicator
                .FirstOrDefaultAsync(m => m.Id == id);
            if (indicator == null)
            {
                return NotFound();
            }

            return View(indicator);
        }

        // POST: Indicators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var indicator = await _context.Indicator.FindAsync(id);
            _context.Indicator.Remove(indicator);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IndicatorExists(int id)
        {
            return _context.Indicator.Any(e => e.Id == id);
        }
    }
}
