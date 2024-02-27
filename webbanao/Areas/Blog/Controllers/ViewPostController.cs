using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using webbanao.Models;
using webbanao.Models.Blog;

namespace webbanao.Areas.Blog.Controllers
{
    [Area("Blog")]
    public class ViewPostController : Controller
    {
        private readonly ILogger<ViewPostController> logger;
        private readonly AppDBContext _context ;

        public ViewPostController(ILogger<ViewPostController> logger, AppDBContext context)
        {
            this.logger = logger;
            _context = context;
        }
        [Route("/post/{slug?}")]
        public IActionResult Index(string slug,int page)
        {
            ViewBag.categories = _context.Categories.ToList();
            ViewBag.categorySlug= slug;
            Category category = null;
            var posts = _context.Posts.Include(p => p.Author).Include(p => p.PostCategories).ThenInclude(pc => pc.Category).AsQueryable();
            if (!string.IsNullOrEmpty(slug))
            {
                category=_context.Categories.Include(c=>c.ChildrenCategory).Where(c=>c.Slug==slug).FirstOrDefault();
                var list = new List<Category>();
                
                if (category == null)
                {
                    return NotFound("Không có danh mục như vậy");
                }
                else
                {
                    
                    AllChildCategories(category, list);
                }
                list.Add(category);
                var postIDs=new List<int>();
                foreach(var item in list)
                {
                    postIDs.AddRange(_context.PostCategories.Where(pc => pc.CategoryId == item.Id).Select(pc=>pc.PostId).ToArray());

                }
                var listAllPost=new List<Post>(); 
                foreach (var postID in postIDs)
                {
                    listAllPost.Add(posts.Where(p=>p.PostId== postID).FirstOrDefault());

                }
                return View(listAllPost);
                
            }
            
            posts.OrderByDescending(p => p.DateUpdated);
            return View(posts.ToList());
        }
        [Route("/post/{postslug}.html")]
        public IActionResult Detail(string postslug)
        {
            return Content(postslug);

        }
        public List<Category> AllChildCategories(Category source, List<Category> des)
        {
            if (source.ChildrenCategory?.Count > 0)
            {
                foreach (var c in source.ChildrenCategory)
                {
                    AllChildCategories(c, des);
                }
            }
            return null;
        }
    }
}
