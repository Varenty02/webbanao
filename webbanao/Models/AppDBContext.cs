using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webbanao.Models.Blog;
using webbanao.Models.Order;
using webbanao.Models.Product;
namespace webbanao.Models
{
    //webbanao.Models.AppDBContext
    public class AppDBContext : IdentityDbContext<AppUser>
    {
       
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
            //..
            // this.Roles
            // IdentityRole<string>
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Slug);
            });
            modelBuilder.Entity<PostCategory>(entity =>
            {
                entity.HasKey(c => new { c.PostId, c.CategoryId });
            });
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasIndex(p => p.Slug).IsUnique();
            });
            modelBuilder.Entity<ProductCategoryProduct>(entity =>
            {
            entity.HasKey(pc => new { pc.ProductID, pc.CategoryID });
            });
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(o => new { o.OrderId, o.ProductId });
            });
        }
        public DbSet<ContactModel> Contacts { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<ProductModel> Products { get; set; }   
        public DbSet<ProductCategoryProduct> ProductCategoryProducts {  get; set; }
        public DbSet<CategoryProduct> CategoryProducts { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderModel> OrderModels { get; set; }
    }
}
