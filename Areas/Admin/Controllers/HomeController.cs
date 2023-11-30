using Microsoft.AspNetCore.Mvc;
using QLSanBong.Models;
using QLSanBong.Data;
using System.Diagnostics;

namespace QLSanBong.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            if (HttpContext.Session.GetString("user") == null)
            {
                return Redirect("~/Login/Index");
            }
            ViewBag.Configuration = configuration;
            return View();
        }
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("~/Login/Index");
        }
    }
}