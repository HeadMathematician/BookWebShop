using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using BookWebShop.Models.Models.CustomErrors;
using System.ComponentModel.DataAnnotations;

namespace BookWebShop.Models.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        [DisplayName("Publish Date")]
        public DateTime PublishDate { get; set; }
        public double Price { get; set; }
        [ValidateNever]
        [DisplayName("")]
        public string? Image { get; set; }
        public Category? Category { get; set; }
        [DisplayName("Category")]
        public int CategoryId { get; set; }
    }
}
