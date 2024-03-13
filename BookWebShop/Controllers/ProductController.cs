using Microsoft.AspNetCore.Mvc;

namespace BookWebShop.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
