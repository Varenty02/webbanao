using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webbanao.Data;
using webbanao.Models;
using webbanao.Models.Blog;
using webbanao.Models.Product;

namespace webbanao.Areas.Product.Controllers
{
    [Area("Product")]
    [Route("admin/categoryproduct/category/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator)]
    public class CategoryProductsController : Controller
    {
        private readonly AppDBContext _context;

        public CategoryProductsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Product/CategoryProducts
        public async Task<IActionResult> Index()
        {
            var appDBContext = _context.CategoryProducts.Include(c => c.ParentCategory).Include(c=>c.CategoryChildren);
            var categories=appDBContext.Where(c=>c.ParentCategory==null).ToList();
            
            return View(categories);
        }
        // GET: Product/CategoryProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryProduct = await _context.CategoryProducts
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryProduct == null)
            {
                return NotFound();
            }

            return View(categoryProduct);
        }
        private void CreateSelectItems(List<CategoryProduct> source, List<CategoryProduct> des, int level)
        {
            string prefix = string.Concat(Enumerable.Repeat("----", level));
            foreach (var category in source)
            {
                // category.Title = prefix + " " + category.Title;
                des.Add(new CategoryProduct()
                {
                    Id = category.Id,
                    Title = prefix + " " + category.Title
                });
                if (category.CategoryChildren?.Count > 0)
                {
                    CreateSelectItems(category.CategoryChildren.ToList(), des, level + 1);
                }
            }
        }
        // GET: Product/CategoryProducts/Create
        public async Task<IActionResult> Create()
        {
            var qr = (from c in _context.CategoryProducts select c).Include(c => c.ParentCategory).Include(c => c.CategoryChildren);
            var categories=qr.Where(c => c.ParentCategory==null).ToList();
            categories.Insert(0, new CategoryProduct()
            {
                Id = -1,
                Title = "Không có danh mục cha",
            });
            var items=new List<CategoryProduct>();
            CreateSelectItems(categories,items,0);
            ViewData["ParentCategoryId"] = new SelectList(items, "Id", "Title");
            return View();
        }

        // POST: Product/CategoryProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,ParentCategoryId")] CategoryProduct categoryProduct)
        {
            if (ModelState.IsValid)
            {
                if (categoryProduct.ParentCategoryId == -1) categoryProduct.ParentCategoryId = null;
                _context.Add(categoryProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var qr = (from c in _context.CategoryProducts select c).Include(c => c.ParentCategory).Include(c => c.CategoryChildren);
            var categories = qr.Where(c => c.ParentCategory == null).ToList();
            categories.Insert(0, new CategoryProduct()
            {
                Id = -1,
                Title = "Không có danh mục cha",
            });
            var items = new List<CategoryProduct>();
            CreateSelectItems(categories, items, 0);
            ViewData["ParentCategoryId"] = new SelectList(items, "Id", "Title");
            return View(categoryProduct);
        }

        // GET: Product/CategoryProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryProduct = await _context.CategoryProducts.FindAsync(id);
            if (categoryProduct == null)
            {
                return NotFound();
            }
            var qr = (from c in _context.CategoryProducts select c).Include(c => c.ParentCategory).Include(c => c.CategoryChildren);
            var categories = qr.Where(c => c.ParentCategory == null).ToList();
            categories.Insert(0, new CategoryProduct()
            {
                Id = -1,
                Title = "Không có danh mục cha",
            });
            var items = new List<CategoryProduct>();
            CreateSelectItems(categories, items, 0);
            ViewData["ParentCategoryId"] = new SelectList(items, "Id", "Title", categoryProduct.ParentCategoryId);
            return View(categoryProduct);
        }

        // POST: Product/CategoryProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Slug,ParentCategoryId")] CategoryProduct categoryProduct)
        {
            if (id != categoryProduct.Id)
            {
                return NotFound();
            }
            bool canUpdate = true;
            if (categoryProduct.ParentCategoryId ==categoryProduct.Id)
            {
                ModelState.AddModelError(string.Empty, "Danh mục cha không hợp lệ");
                return View(categoryProduct);
            }
            if(canUpdate&&categoryProduct.ParentCategoryId!=null)
            {
                var childCates = _context.CategoryProducts.Include(c => c.CategoryChildren).ToList();
                Func<List<CategoryProduct>, bool> checkCateId = null;
                checkCateId = (childCates) =>
                {
                    foreach (var c in childCates)
                    {
                        if (c.Id == categoryProduct.ParentCategoryId)
                        {
                            canUpdate = false;
                            ModelState.AddModelError(string.Empty, "Danh mục cha không hợp lệ");
                            return false;
                        }
                        if(c.CategoryChildren!=null)
                            return checkCateId(c.CategoryChildren.ToList());
                    }
                    return true;
                };
            }
            if (ModelState.IsValid&&canUpdate)
            {
                try
                   
                {
                    if(categoryProduct.ParentCategoryId==-1)
                        categoryProduct.ParentCategoryId= null;
                    var dtc = _context.CategoryProducts.FirstOrDefault(c => c.Id == id);
                    _context.Entry(dtc).State = EntityState.Detached;
                    _context.Update(categoryProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryProductExists(categoryProduct.Id))
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
            var qr = (from c in _context.CategoryProducts select c).Include(c => c.ParentCategory).Include(c => c.CategoryChildren);
            var categories = qr.Where(c => c.ParentCategory == null).ToList();
            categories.Insert(0, new CategoryProduct()
            {
                Id = -1,
                Title = "Không có danh mục cha",
            });
            var items = new List<CategoryProduct>();
            CreateSelectItems(categories, items, 0);
            ViewData["ParentCategoryId"] = new SelectList(items, "Id", "Title", categoryProduct.ParentCategoryId);
            return View(categoryProduct);
        }

        // GET: Product/CategoryProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryProduct = await _context.CategoryProducts
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryProduct == null)
            {
                return NotFound();
            }

            return View(categoryProduct);
        }

        // POST: Product/CategoryProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.CategoryProducts
                           .Include(c => c.CategoryChildren)
                           .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            foreach (var cCategory in category.CategoryChildren)
            {
                cCategory.ParentCategoryId = category.ParentCategoryId;
            }


            _context.CategoryProducts.Remove(category);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        private bool CategoryProductExists(int id)
        {
            return _context.CategoryProducts.Any(e => e.Id == id);
        }
    }
}
