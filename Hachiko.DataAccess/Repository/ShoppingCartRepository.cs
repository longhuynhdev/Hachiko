using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.DataAcess.Data;
using Hachiko.Models;

namespace Hachiko.DataAccess.Repository;

public class ShoppingCartRepository : Repository<ShoppingCart>,IShoppingCartRepository
{
    private ApplicationDbContext _db;

    public ShoppingCartRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(ShoppingCart entity)
    {
        _db.Update(entity);
    }
}