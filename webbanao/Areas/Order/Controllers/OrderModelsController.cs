using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webbanao.Data;
using webbanao.Models;
using webbanao.Models.Order;

namespace webbanao.Areas.Order.Controllers
{
    [Area("Order")]
    [Route("admin/order/[action]/{id?}")]
    [Authorize(Roles =RoleName.Administrator)]
    public class OrderModelsController : Controller
    {
        private readonly AppDBContext _context;

        public OrderModelsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Order/OrderModels
        public async Task<IActionResult> Index()
        {
            var appDBContext = _context.OrderModels.Include(o => o.Customer);
            return View(await appDBContext.ToListAsync());
        }

        // GET: Order/OrderModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderModel = await _context.OrderModels
                .Include(o => o.Customer)
                .Include(o=>o.Items).ThenInclude(o=>o.Product).ThenInclude(p=>p.Photos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderModel == null)
            {
                return NotFound();
            }

            return View(orderModel);
        }

        

        // GET: Order/OrderModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderModel = await _context.OrderModels.Include(o=>o.Customer).Include(o=>o.Items).ThenInclude(i=>i.Product).ThenInclude(p=>p.Photos).Where(o=>o.Id==id).FirstOrDefaultAsync();
            if (orderModel == null)
            {
                return NotFound();
            }
            
            return View(orderModel);
        }

        // POST: Order/OrderModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Total,Name,Description,Address,PhoneNumber")] OrderModel orderModel)
        {
            if (id != orderModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderModelExists(orderModel.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", orderModel.UserId);
            return View(orderModel);
        }

        // GET: Order/OrderModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderModel = await _context.OrderModels
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderModel == null)
            {
                return NotFound();
            }

            return View(orderModel);
        }

        // POST: Order/OrderModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderModel = await _context.OrderModels.FindAsync(id);
            _context.OrderModels.Remove(orderModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderModelExists(int id)
        {
            return _context.OrderModels.Any(e => e.Id == id);
        }
    }
}
