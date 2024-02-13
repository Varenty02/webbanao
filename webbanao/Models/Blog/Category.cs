using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webbanao.Models.Blog
{
    public class Category
    {
        
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage ="Phải có tên danh mục")]
        [StringLength(50,MinimumLength =3,ErrorMessage ="{0} dài {1} đến {2}")]
        [Display(Name ="Tên danh mục")]
        public string Title { get; set; }
        [Display(Name ="Nội dung danh mục")]
        [DataType(DataType.Text)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Phải tạo url")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
        [Display(Name = "Url hiện thị")]

        public string Slug { get; set; }
        //Danh mục cha
        [Display(Name ="Danh mục cha")]
        public int? ParentCategoryId { get; set; }
        [ForeignKey(nameof(ParentCategoryId))]
        [Display(Name = "Danh mục cha")]
        public Category ParentCategory { get; set; }
        //Danh mục con
        public ICollection<Category> ChildrenCategory { get; set; }

    }

}
