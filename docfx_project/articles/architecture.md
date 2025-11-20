# Architecture Overview

## System Architecture

Hachiko follows a **Clean Architecture** pattern with clear separation of concerns across multiple layers. This approach ensures maintainability, testability, and scalability of the application.

## Solution Structure

```
Hachiko Solution/
├── Hachiko/                    # 🌐 Web Application Layer
│   ├── Controllers/            # MVC Controllers
│   ├── Views/                  # Razor Views
│   ├── Areas/                  # Feature Areas (Identity)
│   ├── Migrations/            # EF Core migrations
│   ├── wwwroot/               # Static assets
│   └── Program.cs             # Application entry point
│
├── Hachiko.Models/            # 📊 Domain Models Layer
│   ├── Models/                # Entity models
│   ├── ViewModels/            # Presentation models
│   └── DTOs/                  # Data transfer objects
│
├── Hachiko.DataAccess/        # 🗄️ Data Access Layer
│   ├── Data/                  # DbContext
│   ├── Repository/            # Repository pattern
│   └── IRepository/           # Repository interfaces
│
└── Hachiko.Utility/           # 🔧 Utility Layer
```

## Layer Responsibilities

### 1. Web Application Layer (`Hachiko`)

The presentation layer handling user interactions and HTTP requests/responses.

#### Key Components:

**Controllers**



**Dependency Injection Configuration**
```csharp
// Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Hachiko")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
```

**Authentication & Payment Integration**
- **ASP.NET Core Identity**: Custom `ApplicationUser` extending `IdentityUser`
- **Facebook Authentication**: External login provider
- **Google Authentication**: External login provider with account picker
- **Stripe Payment Processing**: Integrated for order payments
- **Session Management**: For shopping cart and checkout flow

### 2. Domain Models Layer (`Hachiko.Models`)

Contains all business entities, view models, and data transfer objects.

#### Entity Models
```csharp
public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }
    public string? Description { get; set; }
    public string ISBN { get; set; }

    [Required]
    public string Author { get; set; }

    [Display(Name = "Original Price")]
    [Range(0, int.MaxValue)]
    public double OriginalPrice { get; set; }

    [Display(Name = "Price")]
    [Range(0, int.MaxValue)]
    public double Price { get; set; }

    public int CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    [ValidateNever]
    public Category? Category { get; set; }

    [ValidateNever]
    public string? ImageUrl { get; set; }
}
```

#### View Models
```csharp
public class ProductViewModel
{
    public Product Product { get; set; }
    public IEnumerable<SelectListItem> CategoryList { get; set; }
}

```

### 3. Data Access Layer (`Hachiko.DataAccess`)

Implements the Repository and Unit of Work patterns for data access.

#### DbContext Configuration
```csharp
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Seed initial data
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
            new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
            new Category { Id = 3, Name = "History", DisplayOrder = 3 }
        );
    }
}
```

#### Repository Pattern Implementation
```csharp
public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
    T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
    void Add(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entity);
}

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _db;
    internal DbSet<T> dbSet;

    public Repository(ApplicationDbContext db)
    {
        _db = db;
        this.dbSet = _db.Set<T>();
    }

    public void Add(T entity)
    {
        dbSet.Add(entity);
    }

    public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        query = query.Where(filter);
        
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }
        
        return query.FirstOrDefault();
    }
}
```

#### Unit of Work Pattern
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
    void Save();
}

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
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}
```

### 4. Utility Layer (`Hachiko.Utility`)

Contains helper classes, constants, and extension methods used across the application.

```csharp

```

## Design Patterns Used

### 1. Repository Pattern
- **Purpose**: Abstracts data access logic
- **Benefits**: Improved testability, loose coupling
- **Implementation**: Generic repository with specific repositories for complex queries

### 2. Unit of Work Pattern
- **Purpose**: Manages transactions and ensures consistency
- **Benefits**: Coordinates changes across multiple repositories
- **Implementation**: Single point of save for all repositories

### 3. Dependency Injection
- **Purpose**: Promotes loose coupling and testability
- **Benefits**: Easy mocking, configuration flexibility
- **Implementation**: Built-in ASP.NET Core DI container

### 4. Model-View-Controller (MVC)
- **Purpose**: Separates concerns in presentation layer
- **Benefits**: Clear separation, maintainable code
- **Implementation**: Standard ASP.NET Core MVC pattern



## Configuration Management

### Environment-Specific Settings

**Development (appsettings.Development.json)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Hachiko;User Id=sa;Password=SQLServer123@;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Authentication & Authorization
```csharp
// Identity configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 6;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// External authentication providers (configured via User Secrets)
builder.Services.AddAuthentication()
    .AddFacebook(options => { /* Facebook OAuth */ })
    .AddGoogle(options => { /* Google OAuth */ });
```

### Payment Integration
The application uses **Stripe** for payment processing, configured through:
```csharp
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
```