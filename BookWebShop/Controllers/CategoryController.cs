using BookWebShop.Data;
using BookWebShop.Models.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace BookWebShop.Controllers
{

    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Category> categoryList = _context.Categories.ToList();
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
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");

            }
            return View();
        }

        public ActionResult Delete(int categoryId)
        {
            var toDelete = _context.Categories.Where(x => x.Id == categoryId).FirstOrDefault();

            if (toDelete == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(toDelete);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult Edit(int categoryId)
        {
            if(categoryId == 0)
            {
                return NotFound();
            }
            Category category = _context.Categories.Find(categoryId);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            
            Category toUpdate = _context.Categories.Where(x => x.Id == category.Id).FirstOrDefault();

            toUpdate.Name = category.Name;
            toUpdate.DisplayOrder = category.DisplayOrder;

            _context.SaveChanges();
            TempData["success"] = "Category edited succesfully";
            return RedirectToAction("Index");
        }
    }
}
