using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using webbanao.Areas.Blog.Models;
using webbanao.Data;
using webbanao.Models;
using webbanao.Models.Blog;
using webbanao.Utilities;

namespace webbanao.Areas.Blog.Controllers
{
    [Area("Blog")]
    [Route("admin/blog/post/[action]/{id?}")]
    [Authorize(Roles =RoleName.Administrator+","+RoleName.Editor)]
    public class PostsController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<AppUser> _userManager;

        public PostsController(AppDBContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Blog/Posts
        public async Task<IActionResult> Index([FromQuery(Name ="size")] int pagesize, [FromQuery(Name = "page")] int currentPage)
        {
            var appDBContext = _context.Posts.Include(p => p.Author).OrderByDescending(p=>p.DateCreated);
            int totalPosts=appDBContext.Count();
            if (pagesize <= 0) pagesize = 10;
            int countPage = (int)Math.Ceiling((double)totalPosts / pagesize);
            
            if(currentPage>countPage) currentPage= countPage;
            if(currentPage<1)
                currentPage= 1;
            var paging = new PagingModel()
            {
                currentpage=currentPage,
                countpages=countPage,
                generateUrl = (pageNumber) =>Url.Action("Index",new {
                            size=pagesize,
                            p= pageNumber
                        })
                
            };
            ViewBag.paging = paging;
            ViewBag.totalPosts = totalPosts;
            var postInPage=appDBContext.Skip((paging.currentpage-1)* pagesize).Take(pagesize).Include(p=>p.PostCategories).ThenInclude(pc=>pc.Category).ToList();
            return View(postInPage);
        }

        // GET: Blog/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Blog/Posts/Create
        public async Task<IActionResult> Create()
        {
            var categories=await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");
            

            return View();
        }

        // POST: Blog/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Description,Slug,Content,Published,CategoryIDs")] CreatePostModel post)
        {
            string content = Request.Form["inputContent"];
            post.Content=content;
            var slugExist= await _context.Posts.AnyAsync(p => p.Slug==post.Slug);
            if (slugExist)
            {
                ModelState.AddModelError(string.Empty, "Slug đã tồn tại");
                return View(post);
            }
            if (post.Slug == null)
            {
                post.Slug = AppUtilities.GenerateSlug(post.Title);
            }
            if (ModelState.IsValid)
            {
                post.DateCreated = post.DateUpdated = DateTime.Now;
                post.AuthorId=_userManager.GetUserId(this.User);
                
                _context.Add(post);
                if (post.CategoryIDs != null)
                {
                    foreach(var cateId in post.CategoryIDs)
                    {
                        _context.PostCategories.Add(new PostCategory
                        {
                            CategoryId = cateId,
                            Post=post,
                        });
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", post.AuthorId);
            return View(post);
        }

        // GET: Blog/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p=>p.PostCategories).FirstOrDefaultAsync();
            if (post == null)
            {
                return NotFound();
            }
            var postEdit = new CreatePostModel()
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                Description = post.Description,
                Slug = post.Slug,
                Published = post.Published,
                CategoryIDs = post.PostCategories.Select(pc => pc.CategoryId).ToArray(),

            };

            var categories=await _context.Categories.ToListAsync();

            ViewData["Categories"] = new SelectList(categories, "Id", "Title");
            return View(postEdit);
        }

        // POST: Blog/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Description,Slug,Content,Published,CategoryIDs")] CreatePostModel post)
        {
            var categories = await _context.Categories.ToListAsync();
            if (post.Slug == null)
            {
                post.Slug = AppUtilities.GenerateSlug(post.Title);
            }
            if (await _context.Posts.AnyAsync(p => p.Slug == post.Slug))
            {
                ModelState.AddModelError(string.Empty, "Slug này đã tồn tại");
                

                ViewData["Categories"] = new SelectList(categories, "Id", "Title");
                return View(post);
            }
            
            if (ModelState.IsValid&&post.PostId!=id)
            {
                
               
                try
                {
                    var postUpdate=await _context.Posts.Include(p=>p.PostCategories).FirstOrDefaultAsync(p=>p.PostId==id);
                    postUpdate.Title = post.Title;
                    postUpdate.Description = post.Description;
                    postUpdate.Content = post.Content;
                    postUpdate.Published = post.Published;
                    postUpdate.Slug = post.Slug;
                    postUpdate.DateUpdated = DateTime.Now;

                    if (post.CategoryIDs == null) post.CategoryIDs = new int[] { };
                    var oldCateID=postUpdate.PostCategories.Select(p=>p.CategoryId).ToArray();
                    var newCateID = post.CategoryIDs;
                    var removeCate = from postCate in postUpdate.PostCategories
                                     where (!newCateID.Contains(postCate.CategoryId))
                                     select postCate;
                    _context.PostCategories.RemoveRange(removeCate);
                    var addCates=from CateID in newCateID
                                where !oldCateID.Contains(CateID)
                                select CateID;
                    foreach(var addCate in addCates)
                    {
                        _context.PostCategories.Add(new PostCategory() { CategoryId=addCate,PostId=post.PostId});
                    }
                    _context.Update(postUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
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
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", post.AuthorId);
            

            ViewData["Categories"] = new SelectList(categories, "Id", "Title");
            return View(post);
        }

        // GET: Blog/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Blog/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
