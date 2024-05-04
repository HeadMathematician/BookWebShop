using BookWebShop.DataAccess.Repository.IRepository;
using BookWebShop.Models.Models;
using BookWebShop.Models.ViewModels;
using BookWebShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace BookWebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Role.Role_Admin)]

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

        public IActionResult Upsert(int? productId)
        {
            IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });


            if (productId == null || productId == 0)
            {
                ProductViewModel createProduct = new ProductViewModel()
                {
                    Product = new Product(),
                    CategoryList = categoryList,
                };
                createProduct.Product.PublishDate = DateTime.Now;
                return View(createProduct);
            }
            else
            {
                Product product = _unitOfWork.Product.Get(x => x.Id == productId);
                ProductViewModel updateProduct = new ProductViewModel()
                {
                    CategoryList = categoryList,
                    Product = product,
                };
                return View(updateProduct);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel productViewModel, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                byte[] imageBytes = null;

                if (file != null)
                {
                    using (var stream = file.OpenReadStream())
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                }

                if (productViewModel.Product.Id == 0)
                {
                    if (!imageBytes.IsNullOrEmpty())
                    {
                        productViewModel.Product.Image = Convert.ToBase64String(imageBytes);
                    }
                    _unitOfWork.Product.Add(productViewModel.Product);
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    if (!imageBytes.IsNullOrEmpty())
                    {
                        productViewModel.Product.Image = Convert.ToBase64String(imageBytes);
                    }

                    _unitOfWork.Product.Update(productViewModel.Product);
                    TempData["success"] = "Product updated successfully";
                }

                _unitOfWork.Save();
                return RedirectToAction("Index", "Product");

            }
            else
            {
                productViewModel.CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
                return View(productViewModel);
            }
        }
        

        //public IActionResult Delete(int productId)
        //{
        //    if (productId == 0)
        //    {
        //        return NotFound();
        //    }

        //    Product product = _unitOfWork.Product.Get(x => x.Id == productId);


        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(product);
        //}

        //[HttpPost]
        //public IActionResult Delete(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Delete(product);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product edited successfully";
        //        return Redirect("index");
        //    }
        //    return View();
        //}



        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = productList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product = _unitOfWork.Product.Get(x => x.Id == id);

            if(product == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }


            _unitOfWork.Product.Delete(product);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
            
        }

        #endregion

    }
}
