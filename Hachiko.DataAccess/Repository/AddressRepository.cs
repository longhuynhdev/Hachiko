using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.DataAcess.Data;
using Hachiko.Models;
using Microsoft.EntityFrameworkCore;

namespace Hachiko.DataAccess.Repository
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        private ApplicationDbContext _dbContext;
        private const int MAX_ADDRESSES_PER_ENTITY = 5;

        public AddressRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Address entity)
        {
            entity.UpdatedAt = DateTime.Now;
            _dbContext.Addresses.Update(entity);
        }

        public IEnumerable<Address> GetAddressesByUserId(string userId)
        {
            return _dbContext.Addresses
                .Where(a => a.ApplicationUserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedAt)
                .ToList();
        }

        public IEnumerable<Address> GetAddressesByCompanyId(int companyId)
        {
            return _dbContext.Addresses
                .Where(a => a.CompanyId == companyId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedAt)
                .ToList();
        }

        public Address? GetDefaultAddressByUserId(string userId)
        {
            return _dbContext.Addresses
                .FirstOrDefault(a => a.ApplicationUserId == userId && a.IsDefault);
        }

        public Address? GetDefaultAddressByCompanyId(int companyId)
        {
            return _dbContext.Addresses
                .FirstOrDefault(a => a.CompanyId == companyId && a.IsDefault);
        }

        public bool CanAddMoreAddresses(string? userId = null, int? companyId = null)
        {
            int count = 0;

            if (userId != null)
            {
                count = _dbContext.Addresses.Count(a => a.ApplicationUserId == userId);
            }
            else if (companyId != null)
            {
                count = _dbContext.Addresses.Count(a => a.CompanyId == companyId);
            }

            return count < MAX_ADDRESSES_PER_ENTITY;
        }

        public void SetAsDefault(int addressId, string? userId = null, int? companyId = null)
        {
            // Unset all other addresses as default
            if (userId != null)
            {
                var userAddresses = _dbContext.Addresses
                    .Where(a => a.ApplicationUserId == userId && a.Id != addressId);
                foreach (var addr in userAddresses)
                {
                    addr.IsDefault = false;
                    addr.UpdatedAt = DateTime.Now;
                }
            }
            else if (companyId != null)
            {
                var companyAddresses = _dbContext.Addresses
                    .Where(a => a.CompanyId == companyId && a.Id != addressId);
                foreach (var addr in companyAddresses)
                {
                    addr.IsDefault = false;
                    addr.UpdatedAt = DateTime.Now;
                }
            }

            // Set the specified address as default
            var address = _dbContext.Addresses.Find(addressId);
            if (address != null)
            {
                address.IsDefault = true;
                address.UpdatedAt = DateTime.Now;
            }
        }
    }
}
