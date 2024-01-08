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
    public class ReceiptDetailRepository : IReceiptDetailRepository
    {
        readonly TradeMarketDbContext dbContext;
        public ReceiptDetailRepository(TradeMarketDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task AddAsync(ReceiptDetail entity)
        {
            await dbContext.ReceiptsDetails.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        public void Delete(ReceiptDetail entity)
        {
            dbContext.ReceiptsDetails.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var receiptsDetail = await dbContext.ReceiptsDetails.Where(rd => rd.ReceiptId == id || rd.ProductId == id)
            .FirstOrDefaultAsync();

            if (receiptsDetail != null)
            {
                dbContext.ReceiptsDetails.Remove(receiptsDetail);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ReceiptDetail>> GetAllAsync()
        {
            return await dbContext.ReceiptsDetails.ToListAsync();
        }

        public async Task<IEnumerable<ReceiptDetail>> GetAllWithDetailsAsync()
        {
            var receiptsDetailsWithDetails = await dbContext.ReceiptsDetails
            .Include(c => c.Receipt)
            .Include(c => c.Product)
            .Include(c => c.Product.Category)
            .ToListAsync();

            return receiptsDetailsWithDetails;
        }

        public async Task<ReceiptDetail> GetByIdAsync(int id)
        {
            var peceiptDetail = await dbContext.ReceiptsDetails.Where(rd => rd.Id == id)
            .FirstOrDefaultAsync();
            return peceiptDetail;
        }

        public void Update(ReceiptDetail entity)
        {
            dbContext.Attach(entity);

            dbContext.Entry(entity).State = EntityState.Modified;

            dbContext.SaveChanges();
        }
    }
}
