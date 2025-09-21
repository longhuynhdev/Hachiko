using Hachiko.Models;

namespace Hachiko.DataAccess.Repository.IRepository;

public interface ICompanyRepository : IRepository<Company>
{
    public void Update(Company entity);
}