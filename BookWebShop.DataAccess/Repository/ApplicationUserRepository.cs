using BookWebShop.Data;
using BookWebShop.DataAccess.Repository.IRepository;
using BookWebShop.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWebShop.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(ApplicationUser applicationUser)
        {
            _context.Update(applicationUser);
        }
    }
}
