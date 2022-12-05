using CustomIdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Web.Administration;
using System;
using System.Diagnostics;
using System.Linq;

namespace CustomIdentityApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _context;
        public IConfiguration _AppConfiguration;

        public HomeController(ApplicationContext context, ILogger<HomeController> logger, IConfiguration AppConfiguration)
        {
            _context = context;
            _logger = logger;
            _AppConfiguration = AppConfiguration;
        }

        public IActionResult Index()
        {         
            return View(_context.News.Include(i => i.User).ToList());
        }


        [Authorize(Roles = "Администратор")]
        public IActionResult Privacy()
        {
            //получить бж
            IConfigurationSection connStrings = _AppConfiguration.GetSection("ConnectionStrings");
            string defaultConnection = connStrings.GetSection("DefaultConnection").Value;

            ViewBag.ConnectionString = defaultConnection;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
