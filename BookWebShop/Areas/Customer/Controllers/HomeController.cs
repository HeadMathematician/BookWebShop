using BookWebShop.DataAccess.Repository.IRepository;
using BookWebShop.Models.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookWebShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(productList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Details(int productId)
        {
            Product product = _unitOfWork.Product.Get(x => x.Id == productId);
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            if (claimsIdentity != null && claimsIdentity.IsAuthenticated)
            {
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    Product = product,
                    ApplicationUserId = userId,
                };

                string categoryName = _unitOfWork.Category.Get(x => x.Id == product.CategoryId).Name;

                ViewBag.CategoryName = categoryName;

                return View(shoppingCart);
            }
            else
            {
                return Redirect("/Identity/Account/Register");
            }
        }

        [HttpPost]
        public IActionResult Details(int? productId, int count)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCart dbCart = _unitOfWork.ShoppingCart.Get(x => x.ProductId == productId);

            if(dbCart != null)
            {                           
                ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(x => x.ProductId == productId && x.ApplicationUserId == userId);
                shoppingCart.Count = count;
                _unitOfWork.ShoppingCart.Update(shoppingCart);
                _unitOfWork.Save();
            }
            else
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    Count = count,
                    ProductId = (int)productId,
                    ApplicationUserId = userId,
                };

                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
            }
           
            return Redirect("Index");
        }       
    }
}
