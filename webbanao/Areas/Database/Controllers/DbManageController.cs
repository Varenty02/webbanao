using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webbanao.Models;

namespace webbanao.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("/database-manage/[action]")]
    //cái route này sẽ ghi đè route mặc định của startup
    public class DbManageController : Controller
    {
        private readonly AppDBContext _dbContext;
        public DbManageController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
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
    }
}
