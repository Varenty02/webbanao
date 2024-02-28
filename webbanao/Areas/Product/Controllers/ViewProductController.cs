using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using webbanao.Models.Product;
using webbanao.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using webbanao.Areas.Product.Models;
using webbanao.Areas.Product.Services;
using webbanao.Models.Order;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace webbanao.Areas.Product.Controllers
{
    [Area("Product")]
    public class ViewProductController : Controller
    {
        private readonly ILogger<ViewProductController> _logger;
        private readonly AppDBContext _context;
        private readonly CartService _cartService;
        private readonly UserManager<AppUser> _userManager;

        public ViewProductController(ILogger<ViewProductController> logger, AppDBContext context, CartService cartService,UserManager<AppUser> userManager)
        {
            _logger = logger;
            _context = context;
            _cartService = cartService;
            _userManager = userManager;
        }

        // /post/
        // /post/{categoryslug?}
        [Route("/product/{categoryslug?}")]
        public IActionResult Index(string categoryslug, [FromQuery(Name = "p")] int currentPage, int pagesize)
        {
            var categories = GetCategories();

            ViewBag.categories = categories;
            ViewBag.categoryslug = categoryslug;

            CategoryProduct category = null;

            if (!string.IsNullOrEmpty(categoryslug))
            {
                category = _context.CategoryProducts.Where(c => c.Slug == categoryslug)
                                    .FirstOrDefault();

                if (category == null)
                {
                    return NotFound("Không thấy category");
                }
            }

            var products = _context.Products
                                .Include(p => p.Author)
                                .Include(p => p.Photos)
                                .Include(p => p.ProductCategoryProducts)
                                .ThenInclude(p => p.Category)
                                .AsQueryable();

            products = products.OrderByDescending(p => p.DateUpdated);

            if (category != null)
            {


                products = products.Where(p => p.ProductCategoryProducts.Any(pc => pc.CategoryID == category.Id));


            }

            int totalProducts = products.Count();
            if (pagesize <= 0) pagesize = 6;
            int countPages = (int)Math.Ceiling((double)totalProducts / pagesize);

            if (currentPage > countPages) currentPage = countPages;
            if (currentPage < 1) currentPage = 1;

            var pagingModel = new PagingModel()
            {
                countpages = countPages,
                currentpage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index", new
                {
                    p = pageNumber,
                    pagesizse = pagesize
                })
            };

            var productsInPage = products.Skip((currentPage - 1) * pagesize)
                             .Take(pagesize);


            ViewBag.pagingModel = pagingModel;
            ViewBag.totalPosts = totalProducts;



            ViewBag.category = category;
            return View(productsInPage.ToList());
        }

        [Route("/product/{productslug}.html")]
        public IActionResult Detail(string productslug)
        {
            var categories = GetCategories();
            ViewBag.categories = categories;

            var product = _context.Products.Where(p => p.Slug == productslug)
                               .Include(p => p.Author)
                               .Include(p => p.Photos)
                               .Include(p => p.ProductCategoryProducts)
                               .ThenInclude(pc => pc.Category)
                               .FirstOrDefault();

            if (product == null)
            {
                return NotFound("Không thấy sản phẩm");
            }


            return View(product);
        }

        private List<CategoryProduct> GetCategories()
        {
            var categories = _context.CategoryProducts
                            .Include(c => c.CategoryChildren)
                            .AsEnumerable()
                            .Where(c => c.ParentCategory == null)
                            .ToList();
            return categories;
        }
        [Route("addcart/{productid:int}", Name = "addcart")]
        public IActionResult AddToCart([FromRoute] int productid)
        {

            var product = _context.Products
                .Where(p => p.ProductID == productid)
                .FirstOrDefault();
            if (product == null)
                return NotFound("Không có sản phẩm");

            // Xử lý đưa vào Cart ...
            var cart = _cartService.GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductID == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity++;
            }
            else
            {
                //  Thêm mới
                cart.Add(new CartItem() { quantity = 1, product = product });
            }
            
            // Lưu cart vào Session
            _cartService.SaveCartSession(cart);
            // Chuyển đến trang hiện thị Cart
            return RedirectToAction("Index");
        }
        // Hiện thị giỏ hàng
        [Route("/cart", Name = "cart")]
        public IActionResult Cart()
        {
            var carts = _cartService.GetCartItems();
            foreach (var cartitem in carts)
            {
                cartitem.product=_context.Products.Include(p=>p.Photos).Where(p=>p.ProductID==cartitem.product.ProductID).FirstOrDefault();
            }
            
            return View(carts.ToList());
        }

        /// xóa item trong cart
        [Route("/removecart/{productid:int}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute] int productid)
        {
            var cart = _cartService.GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductID == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cart.Remove(cartitem);
            }

            _cartService.SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }

        /// Cập nhật
        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = _cartService.GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductID == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity = quantity;
            }
            _cartService.SaveCartSession(cart);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
        }
        public class CheckoutForm
        {
            [Required]
            [Display(Name="Tên")]
            public string Name { get; set; }
            [Required]
            [Display(Name = "Mô tả")]
            public string Description { get; set; }
            [Required]
            [Display(Name = "Địa chỉ")]
            public string Address { get; set; }
            [Required]
            [Phone]
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; }
        }
        [Route("/checkout")]
        [HttpPost]
        public async Task<IActionResult> Checkout([Bind("Name,Description,Address,PhoneNumber")]CheckoutForm checkoutForm)
        {
            
            var user = await _userManager.GetUserAsync(this.User);
            var carts = _cartService.GetCartItems();
            var total = 0;
            
            foreach(var c in carts)
            {
                total += c.quantity * (int)c.product.Price;
            }
            if(carts?.Count>0&&ModelState.IsValid)
            {
                var newOrder = new OrderModel()
                {
                    UserId = user.Id,
                    Total=total,
                    Name=checkoutForm.Name,
                    Description=checkoutForm.Description,
                    Address=checkoutForm.Address,
                    PhoneNumber=checkoutForm.PhoneNumber,
                };
                await _context.OrderModels.AddAsync(newOrder);
                foreach (var c in carts)
                {
                    _context.OrderItems.Add(new OrderItem()
                    {
                        ProductId=c.product.ProductID,
                        Order=newOrder,
                        Quantity=c.quantity,
                    });
                }


            }
            else
            {
                return View(nameof(Cart), carts);
            }
            // ....
            _cartService.ClearCart();
            await _context.SaveChangesAsync();
            return Redirect(nameof(Index));

        }

    }
}
