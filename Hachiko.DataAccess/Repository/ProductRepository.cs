using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.DataAcess.Data;
using Hachiko.Models;

namespace Hachiko.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
     

        public void Update(Product entity)
        {
            _dbContext.Products.Update(entity);
        }
    }
}
