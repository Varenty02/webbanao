using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using webbanao.Models.Blog;

namespace webbanao.Views.Shared.Component
{
    [ViewComponent]
    public class CategoryNav:ViewComponent
    {
        public class CategoryNavData
        {
            public List<Category> Categories { get; set; }
            public string CategorySlug { get; set; }    
        }
        public IViewComponentResult Invoke(CategoryNavData data)
        {
            return View(data);
        }
    }
}
