using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webbanao.Models.Blog
{
    public class PostCategory
    {
        public int CategoryId { get; set; }
        public int PostId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }
    }
}
