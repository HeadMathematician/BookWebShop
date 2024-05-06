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

            if(claimsIdentity != null && claimsIdentity.IsAuthenticated)
            {
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();

                var allCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId);

                double total = 0;

                foreach (var cart in allCarts)
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
            else
            {
                return Redirect("/Identity/Account/Register");
            }
        }

        public IActionResult Summary()
        {
            return View("Summary");
        }

        public IActionResult IncreaseCount(int itemId)
        {
            var cart = _unitOfWork.ShoppingCart.Get(x => x.Id == itemId);

            cart.Count += 1;

            _unitOfWork.ShoppingCart.Update(cart);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DecreaseCount(int itemId)
        {
            var cart = _unitOfWork.ShoppingCart.Get(x => x.Id ==itemId);    

            cart.Count -= 1;

            if(cart.Count > 0)
            {
                _unitOfWork.ShoppingCart.Update(cart);
                _unitOfWork.Save();
            }

            else
            {
                _unitOfWork.ShoppingCart.Delete(cart);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteCart(int itemId)
        {
            var cart = _unitOfWork.ShoppingCart.Get(x => x.Id == itemId);

            _unitOfWork.ShoppingCart.Delete(cart);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public PartialViewResult CartCount(string userId)
        {
            var cartCount = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId).Count();
            return PartialView("_CartCount", cartCount);
        }
    }
}
