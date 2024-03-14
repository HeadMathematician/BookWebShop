using BookWebShop.Data;
using BookWebShop.DataAccess.Repository;
using BookWebShop.DataAccess.Repository.IRepository;
using BookWebShop.Models.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace BookWebShop.Controllers
{

    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _context;
        //private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        //public CategoryController(ICategoryRepository categoryRepository, ApplicationDbContext context)
        //{
        //    _categoryRepository = categoryRepository;
        //    _context = context;
        //}

        public CategoryController(IUnitOfWork unitOfWork)
        {
           _unitOfWork = unitOfWork;
        }

        //public IActionResult Index()
        //{
        //    List<Category> categoryList = _categoryRepository.GetAll().ToList();
        //    return View(categoryList);
        //}
        public IActionResult Index()
        {
            List<Category> categoryList = _unitOfWork.Category.GetAll().ToList();
            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult Delete(int categoryId)
        {
            var category = _unitOfWork.Category.Get(c => c.Id == categoryId);
            _unitOfWork.Category.Delete(category);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int categoryId)
        {
            if(categoryId == 0)
            {
                return NotFound();
            }

            Category category = _unitOfWork.Category.Get(x => x.Id == categoryId);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {

            Category toUpdate = _unitOfWork.Category.Get(x => x.Id == category.Id);

            toUpdate.Name = category.Name;
            toUpdate.DisplayOrder = category.DisplayOrder;
            _unitOfWork.Save();

            TempData["success"] = "Category edited succesfully";
            return RedirectToAction("Index");
        }
    }
}
