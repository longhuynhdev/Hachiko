using Hachiko.Models;

namespace Hachiko.DataAccess.Repository.IRepository;

public interface IShoppingCartRepository : IRepository<ShoppingCart>
{
    public void Update(ShoppingCart entity);
}