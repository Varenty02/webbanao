using System.ComponentModel.DataAnnotations;
using webbanao.Models.Product;

namespace webbanao.Areas.Product.Models
{
    public class CreateProductModel : ProductModel
    {
        [Display(Name = "Chuyên mục")]
        public int[] CategoryIDs { get; set; }
    }
}
