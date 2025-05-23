﻿using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.DataAcess.Data;
using Hachiko.Models;

namespace Hachiko.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
     

        public void Update(Category obj)
        {
            _db.Categories.Update(obj);
        }
    }
}
