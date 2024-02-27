using System.ComponentModel.DataAnnotations;
using webbanao.Models.Blog;

namespace webbanao.Areas.Blog.Models
{
    public class CreatePostModel : Post
    {
        [Display(Name = "Chuyên mục")]
        public int[] CategoryIDs { get; set; }
    }
}
