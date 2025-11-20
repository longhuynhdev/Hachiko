## Adding New Entities

Follow these steps to add a new entity to the project:

### 1. Create Model Class
Create your entity class in `Hachiko.Models`:

```csharp
public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    // ... other properties
}
```

### 2. Create Repository Interface
Create repository interface in `Hachiko.DataAccess/Repository/IRepository`:

```csharp
public interface IOrderRepository : IRepository<Order>
{
    void Update(Order obj);
    IEnumerable<Order> GetOrdersByUser(string userId);
    // Add entity-specific queries here
}
```

### 3. Implement Repository
Create repository implementation in `Hachiko.DataAccess/Repository`:

```csharp
public class OrderRepository : Repository<Order>, IOrderRepository
{
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

### 4. Update Unit of Work Interface
Add the repository property to `IUnitOfWork`:

```csharp
public interface IUnitOfWork
{
    ICategoryRepository Category { get; }
    IProductRepository Product { get; }
    ICompanyRepository Company { get; }
    IShoppingCartRepository ShoppingCart { get; }
    IApplicationUserRepository ApplicationUser { get; }
    IOrderHeaderRepository OrderHeader { get; }
    IOrderDetailRepository OrderDetail { get; }
    IAddressRepository Address { get; }
    IOrderRepository Order { get; }  // Add new repository here
    void Save();
}
```

### 5. Update Unit of Work Implementation
Add repository initialization in `UnitOfWork` class:

```csharp
public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext _db;

    public ICategoryRepository Category { get; private set; }
    public IProductRepository Product { get; private set; }
    public ICompanyRepository Company { get; private set; }
    public IShoppingCartRepository ShoppingCart { get; private set; }
    public IApplicationUserRepository ApplicationUser { get; private set; }
    public IOrderHeaderRepository OrderHeader { get; private set; }
    public IOrderDetailRepository OrderDetail { get; private set; }
    public IAddressRepository Address { get; private set; }
    public IOrderRepository Order { get; private set; }  // Add this

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        Category = new CategoryRepository(_db);
        Product = new ProductRepository(_db);
        Company = new CompanyRepository(_db);
        ShoppingCart = new ShoppingCartRepository(_db);
        ApplicationUser = new ApplicationUserRepository(_db);
        OrderHeader = new OrderHeaderRepository(_db);
        OrderDetail = new OrderDetailRepository(_db);
        Address = new AddressRepository(_db);
        Order = new OrderRepository(_db);  // Initialize here
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}
```

### 6. Add DbSet to ApplicationDbContext
Add the DbSet property to `ApplicationDbContext`:

```csharp
public DbSet<Order> Orders { get; set; }
```

### 7. Create and Apply Migration
```bash
cd Hachiko
dotnet ef migrations add AddOrderEntity
dotnet ef database update
```