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
    public class AddedFilesController : Controller
    {
        private readonly RaqmiyaContext _context;

        public AddedFilesController(RaqmiyaContext context)
        {
            _context = context;
        }

        // GET: AddedFiles
        public async Task<IActionResult> Index()
        {
            var raqmiyaContext = _context.Files.Include(a => a.Product);
            return View(await raqmiyaContext.ToListAsync());
        }

        // GET: AddedFiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addedFile = await _context.Files
                .Include(a => a.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (addedFile == null)
            {
                return NotFound();
            }

            return View(addedFile);
        }

        // GET: AddedFiles/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Currency");
            return View();
        }

        // POST: AddedFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Name,FileUrl,Size,ContentType")] AddedFile addedFile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(addedFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Currency", addedFile.ProductId);
            return View(addedFile);
        }

        // GET: AddedFiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addedFile = await _context.Files.FindAsync(id);
            if (addedFile == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Currency", addedFile.ProductId);
            return View(addedFile);
        }

        // POST: AddedFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Name,FileUrl,Size,ContentType")] AddedFile addedFile)
        {
            if (id != addedFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(addedFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddedFileExists(addedFile.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Currency", addedFile.ProductId);
            return View(addedFile);
        }

        // GET: AddedFiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addedFile = await _context.Files
                .Include(a => a.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (addedFile == null)
            {
                return NotFound();
            }

            return View(addedFile);
        }

        // POST: AddedFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var addedFile = await _context.Files.FindAsync(id);
            if (addedFile != null)
            {
                _context.Files.Remove(addedFile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AddedFileExists(int id)
        {
            return _context.Files.Any(e => e.Id == id);
        }
    }
}
