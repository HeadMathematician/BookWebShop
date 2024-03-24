using BookWebShop.Data;
using BookWebShop.DataAccess.Repository.IRepository;
using BookWebShop.Models.Models;

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
