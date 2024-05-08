using BookWebShop.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using BookWebShop.Models.ViewModels;
using BookWebShop.Models.Models;
using System.Security.Claims;
using BookWebShop.Data;
using Microsoft.CodeAnalysis;
using Stripe.Checkout;
using BookWebShop.Utility;
using Stripe;

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

                ApplicationUser user = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);

                List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();

				var allCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId);

				double total = 0;

				foreach (var cart in allCarts)
				{
					shoppingCarts.Add(cart);
					var productPrice = _unitOfWork.Product.Get(x => x.Id == cart.ProductId).Price;
					total += Convert.ToDouble(cart.Count) * productPrice;
				}

				DateTime orderDate = DateTime.Now;

				OrderHeader orderHeader = new OrderHeader
				{
					Name = user.Name,
					PhoneNumber = user.PhoneNumber,
					StreetAddress = user.StreetAddress,
					City = user.City,
					State = user.State,
					PostalCode = user.PostalCode,
					OrderTotal = total,
					OrderDate = orderDate,
				};

				ShoppingCartViewModel shoppingCartViewModel = new ShoppingCartViewModel()
                {
                    ShoppingCartList = allCarts,
                    OrderHeader = orderHeader,
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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var shoppingCart = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId);

			List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();

			ApplicationUser user = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);
			DateTime orderDate = DateTime.Now;

			var allCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId);

            double total = 0;

			foreach (var cart in allCarts)
			{
				shoppingCarts.Add(cart);
				var productPrice = _unitOfWork.Product.Get(x => x.Id == cart.ProductId).Price;
				total += Convert.ToDouble(cart.Count) * productPrice;
			}

			OrderHeader orderHeader = new OrderHeader
			{
				Name = user.Name,
				PhoneNumber = user.PhoneNumber,
				StreetAddress = user.StreetAddress,
				City = user.City,
				State = user.State,
				PostalCode = user.PostalCode,
				OrderTotal = total,
				OrderDate = orderDate,
			};

			ShoppingCartViewModel shoppingCartViewModel = new ShoppingCartViewModel()
			{
				ShoppingCartList = allCarts,
				OrderHeader = orderHeader,
			};
			
			return View(shoppingCartViewModel);
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

		public IActionResult OrderConfirmation(int id)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == id, includeProperties: "ApplicationUser");

			if (orderHeader.PaymentStatus != PaymentStatus.Delayed)
			{
				var sessionService = new SessionService();
				Session session = sessionService.Get(orderHeader.SessionId);

				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
					_unitOfWork.OrderHeader.UpdateStatus(id, OrderStatus.Approved, PaymentStatus.Approved);
					_unitOfWork.Save();
				}
			}

			List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

			_unitOfWork.ShoppingCart.DeleteRange(shoppingCarts);
			_unitOfWork.Save();

			return View(id);
		}

		[HttpPost]
		[ActionName("Summary")]
		public IActionResult SummaryPOST(OrderHeader orderHeader)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();

			var allCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId);

			foreach (var cart in allCarts)
			{
				shoppingCarts.Add(cart);
			}

			orderHeader.OrderDate = DateTime.Now;
			orderHeader.ApplicationUserId = userId;

			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);

			foreach (var cart in allCarts)
			{
				BookWebShop.Models.Models.Product product = _unitOfWork.Product.Get(x => x.Id == cart.ProductId);
				orderHeader.OrderTotal += (product.Price * cart.Count);
			}

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				orderHeader.PaymentStatus = PaymentStatus.Pending;
				orderHeader.OrderStatus = OrderStatus.Pending;
			}
			else
			{
				orderHeader.PaymentStatus = PaymentStatus.Delayed;
				orderHeader.OrderStatus = OrderStatus.Approved;
			}

			_unitOfWork.OrderHeader.Add(orderHeader);
			_unitOfWork.Save();

			foreach (var cart in allCarts)
			{
				BookWebShop.Models.Models.Product product = _unitOfWork.Product.Get(x => x.Id == cart.ProductId);
				OrderDetails orderDetails = new()
				{				
					ProductId = cart.ProductId,
					OrderHeaderId = orderHeader.Id,
					Price = product.Price * cart.Count,
					Count = cart.Count,
				};

				_unitOfWork.OrderDetails.Add(orderDetails);
				_unitOfWork.Save();
			}

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				var domain = "https://localhost:7287/";

				var options = new SessionCreateOptions
				{
					//SuccessUrl = "https://example.com/success",
					SuccessUrl = domain + $"customer/shoppingcart/OrderConfirmation?id={orderHeader.Id}",
					CancelUrl = domain + "customer/cart/index",
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
				};

				foreach (var item in allCarts)
				{
					BookWebShop.Models.Models.Product product = _unitOfWork.Product.Get(x => x.Id == item.ProductId);
					var sessionLineItem = new SessionLineItemOptions()
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(product.Price * item.Count * 100), 
							Currency = "eur",
							ProductData = new SessionLineItemPriceDataProductDataOptions()
							{
								Name = item.Product.Name,
							}
						},
						Quantity = item.Count
					};
					options.LineItems.Add(sessionLineItem);
				}

				var service = new SessionService();
				Session session = service.Create(options);

                var nest2o = orderHeader;


                _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeader.Id, session.Id, session.PaymentIntentId);
				_unitOfWork.Save();

				Response.Headers.Add("Location", session.Url);

				return new StatusCodeResult(303);
			}

			return RedirectToAction(nameof(OrderConfirmation), new { id = 15 });
		}
	}
}
