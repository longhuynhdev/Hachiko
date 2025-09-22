1. Create new model

```cs
  public class Order {
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    // ... other properties
}
```

2. Create Repository Interface

```cs
public interface IOrderRepository : IRepository<Order> {
    void Update(Order obj);
    IEnumerable<Order> GetOrdersByUser(string userId);
    // Add any Order-specific queries here
  }

```

3. Create Repository Implement

```cs
public class OrderRepository : Repository<Order>, IOrderRepository {
    private ApplicationDbContext _db;

    public OrderRepository(ApplicationDbContext db) : base(db)
    {
    _db = db;
    }

    public void Update(Order obj)
    {
    _db.Orders.Update(obj);
    }

    public IEnumerable<Order> GetOrdersByUser(string userId)
    {
    return _db.Orders.Where(o => o.UserId == userId).ToList();
    }
}
```

4. Update Unit of Work Interface

```cs
public interface IUnitOfWork
{
    ICategoryRepository Category { get; }
    IProductRepository Product { get; }
    IOrderRepository Order { get; }  // Add this
    void Save();
}
```

5. Update Unit of Work Implementation
```cs
public class UnitOfWork : IUnitOfWork {
    private ApplicationDbContext _db;
    public ICategoryRepository Category { get; private set; }
    public IProductRepository Product { get; private set; }
    public IOrderRepository Order { get; private set; }  // Add this

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        Category = new CategoryRepository(_db);
        Product = new ProductRepository(_db);
        Order = new OrderRepository(_db);  // Add this
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}
```
  6. Add DbSet to DbContext
```cs
  public DbSet<Order> Orders { get; set; }
```