using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webbanao.Models.Blog;
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
        }
        public DbSet<ContactModel> Contacts { get; set; }

        public DbSet<Category> Categories { get; set; }
    }
}
