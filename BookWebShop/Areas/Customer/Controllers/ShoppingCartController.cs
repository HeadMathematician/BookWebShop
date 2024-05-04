using BookWebShop.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using BookWebShop.Models.ViewModels;
using BookWebShop.Models.Models;
using System.Security.Claims;
using BookWebShop.Data;
using Microsoft.CodeAnalysis;

namespace BookWebShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private ApplicationDbContext _context { get; set; }
        public ShoppingCartController(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();

            var allCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId);

            double total = 0;

            foreach(var cart in allCarts)
            {
                shoppingCarts.Add(cart);
                var productPrice = _unitOfWork.Product.Get(x => x.Id == cart.ProductId).Price;
                total += Convert.ToDouble(cart.Count) * productPrice;
            }

            ShoppingCartViewModel shoppingCartViewModel = new ShoppingCartViewModel()
            {
                ShoppingCartList = allCarts,
                OrderTotal = total,
            };

            return View(shoppingCartViewModel);
        }
    }
}
