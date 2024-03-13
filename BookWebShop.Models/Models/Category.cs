using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookWebShop.Models.Models
{
    public class Category
    {
        public int Id { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
