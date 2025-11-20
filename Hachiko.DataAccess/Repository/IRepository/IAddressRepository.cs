using Hachiko.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hachiko.DataAccess.Repository.IRepository
{
    public interface IAddressRepository : IRepository<Address>
    {
        void Update(Address entity);
        IEnumerable<Address> GetAddressesByUserId(string userId);
        IEnumerable<Address> GetAddressesByCompanyId(int companyId);
        Address? GetDefaultAddressByUserId(string userId);
        Address? GetDefaultAddressByCompanyId(int companyId);
        bool CanAddMoreAddresses(string? userId = null, int? companyId = null);
        void SetAsDefault(int addressId, string? userId = null, int? companyId = null);
    }
}
