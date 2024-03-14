using BookWebShop.DataAccess.Repository.IRepository;
using BookWebShop.Models.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace BookWebShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll().ToList();

            return View(products);
        }

        public List<Category> GetAllCategories()
        {
            var categories =  _unitOfWork.Category.GetAll().ToList();
            return categories;
        }

        public IActionResult Create()
        {
            var productCreateViewModel = new ProductCreateViewModel
            {
                Product = new Product(),
                Categories = GetAllCategories()
            };

            return View(productCreateViewModel);
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            
                _unitOfWork.Product.Add(product);
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            
        }
    }
}
