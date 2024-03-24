using BookWebShop.Data;
using BookWebShop.DataAccess.Repository.IRepository;
using BookWebShop.Models.Models;

namespace BookWebShop.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            _context.Update(product);
        }
    }
}
