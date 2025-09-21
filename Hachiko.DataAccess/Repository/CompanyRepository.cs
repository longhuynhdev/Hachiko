using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.DataAcess.Data;
using Hachiko.Models;

namespace Hachiko.DataAccess.Repository;

public class CompanyRepository : Repository<Company>, ICompanyRepository {
    
    private ApplicationDbContext _db;

    public CompanyRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
    
    public void Update(Company entity)
    {
        _db.Update(entity);
    }
}