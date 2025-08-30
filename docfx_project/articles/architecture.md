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
```csharp

```


**Dependency Injection Configuration**
```csharp
// Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
```

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
    
    public string Description { get; set; }
    
    [Required]
    public string Author { get; set; }
    
    [Display(Name = "List Price")]
    [Range(1, 1000)]
    public double ListPrice { get; set; }
    
    [Display(Name = "Price for 1-50")]
    [Range(1, 1000)]
    public double Price { get; set; }
    
    public int CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    [ValidateNever]
    public Category Category { get; set; }
    
    public string ImageUrl { get; set; }
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
    IShoppingCartRepository ShoppingCart { get; }
    IApplicationUserRepository ApplicationUser { get; }
    void Save();
}

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext _db;
    
    public ICategoryRepository Category { get; private set; }
    public IProductRepository Product { get; private set; }
    public IApplicationUserRepository ApplicationUser { get; private set; }

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        Category = new CategoryRepository(_db);
        Product = new ProductRepository(_db);
        ApplicationUser = new ApplicationUserRepository(_db);
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
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

```