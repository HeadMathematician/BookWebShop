﻿using BookWebShop.Models.Models;

namespace BookWebShop.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);
    }
}
