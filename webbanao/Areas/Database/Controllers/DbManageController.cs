using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webbanao.Data;
using webbanao.Models;

namespace webbanao.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("/database-manage/[action]")]
    //cái route này sẽ ghi đè route mặc định của startup
    public class DbManageController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbManageController(AppDBContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DeleteDb()
        {
            return View();
        }
        [TempData]
        public string  StatusMessage { get; set; }
        [TempData]
        public bool StatusAlert { get; set; } 
        [HttpPost]
        public async Task<IActionResult> DeleteDbAsync()
        {
            var success = await _dbContext.Database.EnsureDeletedAsync();
            StatusMessage = success ? "Xóa database thành công" :"Không xóa được";
            StatusAlert= success ;
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> CreateDbAsync()
        {
            await _dbContext.Database.MigrateAsync();
            StatusMessage = "Tạo database thành công";
            StatusAlert = true;
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> SeedDataAsync()
        {
            var rolenames=typeof(RoleName).GetFields().ToList();
            foreach(var r in rolenames)
            {
                var rolename=(string)r.GetRawConstantValue();
                var rfound = await _roleManager.FindByNameAsync(rolename);
                if (rfound == null) {
                    await _roleManager.CreateAsync(new IdentityRole(rolename));
                }
            }
            var useradmin = await _userManager.FindByNameAsync("admin");
            if(useradmin == null)
            {
                useradmin = new AppUser()
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(useradmin,"admin123");
                await _userManager.AddToRoleAsync(useradmin, RoleName.Administrator);
            }
            StatusMessage = "Seed database thành công";
            StatusAlert = true;
            return RedirectToAction(nameof(Index));
        }
    }
}
