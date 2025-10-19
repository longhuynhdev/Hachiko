using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.DataAcess.Data;
using Hachiko.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hachiko.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _dbContext;
        public OrderHeaderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public void Update(OrderHeader entity)
        {
            _dbContext.OrderHeaders.Update(entity);
        }
    }
}
