using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.DataAcess.Data;
using Hachiko.Models;

namespace Hachiko.DataAccess.Repository;

public class CompanyRepository : Repository<Company>, ICompanyRepository {
    
    private ApplicationDbContext _dbContext;

    public CompanyRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    
    public void Update(Company entity)
    {
        _dbContext.Companies.Update(entity);
    }
}