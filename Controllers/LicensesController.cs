using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ITI_Raqmiya_MVC.Data;
using ITI_Raqmiya_MVC.Models;

namespace ITI_Raqmiya_MVC.Controllers
{
    public class LicensesController : Controller
    {
        private readonly RaqmiyaContext _context;

        public LicensesController(RaqmiyaContext context)
        {
            _context = context;
        }

        // GET: Licenses
        public async Task<IActionResult> Index()
        {
            var raqmiyaContext = _context.Licenses.Include(l => l.Buyer).Include(l => l.Order).Include(l => l.Product);
            return View(await raqmiyaContext.ToListAsync());
        }

        // GET: Licenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var license = await _context.Licenses
                .Include(l => l.Buyer)
                .Include(l => l.Order)
                .Include(l => l.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (license == null)
            {
                return NotFound();
            }

            return View(license);
        }

        // GET: Licenses/Create
        public IActionResult Create()
        {
            ViewData["BuyerId"] = new SelectList(_context.Users, "Id", "Email");
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Currency");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Currency");
            return View();
        }

        // POST: Licenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderId,ProductId,BuyerId,LicenseKey,AccessGrantedAt,ExpiresAt,Status")] License license)
        {
            if (ModelState.IsValid)
            {
                _context.Add(license);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BuyerId"] = new SelectList(_context.Users, "Id", "Email", license.BuyerId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Currency", license.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Currency", license.ProductId);
            return View(license);
        }

        // GET: Licenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var license = await _context.Licenses.FindAsync(id);
            if (license == null)
            {
                return NotFound();
            }
            ViewData["BuyerId"] = new SelectList(_context.Users, "Id", "Email", license.BuyerId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Currency", license.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Currency", license.ProductId);
            return View(license);
        }

        // POST: Licenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,ProductId,BuyerId,LicenseKey,AccessGrantedAt,ExpiresAt,Status")] License license)
        {
            if (id != license.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(license);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LicenseExists(license.Id))
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
            ViewData["BuyerId"] = new SelectList(_context.Users, "Id", "Email", license.BuyerId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Currency", license.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Currency", license.ProductId);
            return View(license);
        }

        // GET: Licenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var license = await _context.Licenses
                .Include(l => l.Buyer)
                .Include(l => l.Order)
                .Include(l => l.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (license == null)
            {
                return NotFound();
            }

            return View(license);
        }

        // POST: Licenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var license = await _context.Licenses.FindAsync(id);
            if (license != null)
            {
                _context.Licenses.Remove(license);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LicenseExists(int id)
        {
            return _context.Licenses.Any(e => e.Id == id);
        }
    }
}
