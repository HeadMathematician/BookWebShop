using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWebShop.Models.Models
{
    public class ProductCreateViewModel
    {
        public Product Product { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
