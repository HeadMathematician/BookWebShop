﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWebShop.Models.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string? Description { get; set; }
        public DateTime PublishDate { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public Category Category { get; set; }
    }
}
