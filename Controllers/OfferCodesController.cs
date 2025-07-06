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
    public class OfferCodesController : Controller
    {
        private readonly RaqmiyaContext _context;

        public OfferCodesController(RaqmiyaContext context)
        {
            _context = context;
        }

        // GET: OfferCodes
        public async Task<IActionResult> Index()
        {
            var raqmiyaContext = _context.OfferCodes.Include(o => o.Product);
            return View(await raqmiyaContext.ToListAsync());
        }

        // GET: OfferCodes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offerCode = await _context.OfferCodes
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offerCode == null)
            {
                return NotFound();
            }

            return View(offerCode);
        }

        // GET: OfferCodes/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Currency");
            return View();
        }

        // POST: OfferCodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Code,DiscountType,DiscountValue,UsageLimit,TimesUsed,StartsAt,ExpiresAt")] OfferCode offerCode)
        {
            if (ModelState.IsValid)
            {
                _context.Add(offerCode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Currency", offerCode.ProductId);
            return View(offerCode);
        }

        // GET: OfferCodes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offerCode = await _context.OfferCodes.FindAsync(id);
            if (offerCode == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Currency", offerCode.ProductId);
            return View(offerCode);
        }

        // POST: OfferCodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Code,DiscountType,DiscountValue,UsageLimit,TimesUsed,StartsAt,ExpiresAt")] OfferCode offerCode)
        {
            if (id != offerCode.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(offerCode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfferCodeExists(offerCode.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Currency", offerCode.ProductId);
            return View(offerCode);
        }

        // GET: OfferCodes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offerCode = await _context.OfferCodes
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offerCode == null)
            {
                return NotFound();
            }

            return View(offerCode);
        }

        // POST: OfferCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var offerCode = await _context.OfferCodes.FindAsync(id);
            if (offerCode != null)
            {
                _context.OfferCodes.Remove(offerCode);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OfferCodeExists(int id)
        {
            return _context.OfferCodes.Any(e => e.Id == id);
        }
    }
}
