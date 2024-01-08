using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ReceiptRepository : IReceiptRepository
    {
        readonly TradeMarketDbContext dbContext;
        public ReceiptRepository(TradeMarketDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(Receipt entity)
        {
            await dbContext.Receipts.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        public void Delete(Receipt entity)
        {
            dbContext.Receipts.Remove(entity);
            dbContext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var receipts = await dbContext.Receipts.FindAsync(id);

            if (receipts != null)
            {
                dbContext.Receipts.Remove(receipts);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Receipt>> GetAllAsync()
        {
            return await dbContext.Receipts.ToListAsync();
        }

        public async Task<IEnumerable<Receipt>> GetAllWithDetailsAsync()
        {
            var receiptsDetailsWithDetails = await dbContext.Receipts
             .Include(c => c.Customer)
             .Include(c => c.ReceiptDetails)
             .ThenInclude(r => r.Product)
             .ThenInclude(r => r.Category)
             .ToListAsync();

            return receiptsDetailsWithDetails;
        }

        public async Task<Receipt> GetByIdAsync(int id)
        {
            var receipt = await dbContext.Receipts.FindAsync(id);
            return receipt;
        }

        public async Task<Receipt> GetByIdWithDetailsAsync(int id)
        {
            var receiptsDetailsWithDetails = await dbContext.Receipts
                .Where(c => c.Id == id)
             .Include(c => c.Customer)
             .Include(c => c.ReceiptDetails)
             .ThenInclude(r => r.Product)
             .ThenInclude(r => r.Category)
              .FirstOrDefaultAsync();

            return receiptsDetailsWithDetails;
        }

        public void Update(Receipt entity)
        {
            dbContext.Attach(entity);

            dbContext.Entry(entity).State = EntityState.Modified;

            dbContext.SaveChanges();
        }
    }
}
