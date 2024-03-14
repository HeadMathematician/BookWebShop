using BookWebShop.DataAccess.Repository.IRepository;
using BookWebShop.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BookWebShop.Data;

namespace BookWebShop.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Category category)
        {
            _context.Update(category);
        }

    }
}
