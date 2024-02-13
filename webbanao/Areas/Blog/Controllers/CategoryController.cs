using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webbanao.Data;
using webbanao.Models;
using webbanao.Models.Blog;

namespace webbanao.Areas.Blog.Controllers
{
    [Area("Blog")]
    [Route("admin/blog/category/[action]/{id?}")]
    [Authorize(Roles =RoleName.Administrator)]
    public class CategoryController : Controller
    {
        private readonly AppDBContext _context;

        public CategoryController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Blog/Category
        public async Task<IActionResult> Index()
        {
            var appDBContext = _context.Categories.Include(c => c.ParentCategory).Include(c=>c.ChildrenCategory);
            var categories =( await appDBContext.ToListAsync()).Where(c => c.ParentCategory == null).ToList();
            return View(categories);
        }

        // GET: Blog/Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        //// GET: Blog/Category/Create
        public async Task<IActionResult> Create()
        {
            var qr = (from c in _context.Categories select c)
                .Include(c => c.ParentCategory)
                .Include(c => c.ChildrenCategory);
            var categories = (await qr.ToListAsync()).Where(c => c.ParentCategoryId == null).ToList();
            categories.Insert(0, new Category()
            {
                Id = -1,
                Title = "Không có danh mục cha"
            });
            var items = new List<Category>();
            CreateSelectedItem(categories, items, 0);
            ViewData["ParentCategoryId"] = new SelectList(items, "Id", "Title");
            return View();
        }

        private void CreateSelectedItem(List<Category> source, List<Category> des, int level)
        {
            string prefix = string.Concat(Enumerable.Repeat("----", level));
            foreach (var category in source)
            {
                des.Add(new Category()
                {
                    Id = category.Id,
                    Title = prefix + " " + category.Title
                });
                if (category.ChildrenCategory?.Count > 0)
                {
                    CreateSelectedItem(category.ChildrenCategory.ToList(), des, level + 1);
                }
            }
        }

        // POST: Blog/Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,ParentCategoryId")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.ParentCategoryId == -1) category.ParentCategoryId = null;
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var qr = (from c in _context.Categories select c)
                .Include(c => c.ParentCategory)
                .Include(c => c.ChildrenCategory);
            var categories = (await qr.ToListAsync()).Where(c => c.ParentCategoryId == null).ToList();
            categories.Insert(0, new Category()
            {
                Id = -1,
                Title = "Không có danh mục cha"
            });
            var items = new List<Category>();
            CreateSelectedItem(categories, items, 0);
            ViewData["ParentCategoryId"] = new SelectList(items, "Id", "Slug");
            return View(category);
        }

        // GET: Blog/Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Slug", category.ParentCategoryId);
            return View(category);
        }

        // POST: Blog/Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Slug,ParentCategoryId")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Slug", category.ParentCategoryId);
            return View(category);
        }

        // GET: Blog/Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Blog/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var category = await _context.Categories.FindAsync(id);
            //_context.Categories.Remove(category);
            //await _context.SaveChangesAsync();
            var cate = await _context.Categories.Include(c => c.ChildrenCategory).FirstOrDefaultAsync(c=>c.Id==id);
            if(cate == null)
            {
                return NotFound();
            }
            if (cate.ChildrenCategory?.Count > 0)
            {
                foreach (var childCate in cate.ChildrenCategory)
                {
                    childCate.ParentCategoryId = cate.ParentCategoryId;
                }
            }
            _context.Categories.Remove(cate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
