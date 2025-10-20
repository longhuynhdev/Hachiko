using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.DataAcess.Data;
using Hachiko.Models;

namespace Hachiko.DataAccess.Repository;

public class ShoppingCartRepository : Repository<ShoppingCart>,IShoppingCartRepository
{
    private ApplicationDbContext _dbContext;

    public ShoppingCartRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void Update(ShoppingCart entity)
    {
        _dbContext.ShoppingCarts.Update(entity);
    }
}