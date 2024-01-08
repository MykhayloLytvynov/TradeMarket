using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        readonly TradeMarketDbContext dbContext;
        public ProductCategoryRepository(TradeMarketDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(ProductCategory entity)
        {
            await dbContext.ProductCategories.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        public void Delete(ProductCategory entity)
        {
            dbContext.ProductCategories.Remove(entity);
            dbContext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var productCategory = await dbContext.ProductCategories.FindAsync(id);

            if (productCategory != null)
            {
                dbContext.ProductCategories.Remove(productCategory);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProductCategory>> GetAllAsync()
        {
            return await dbContext.ProductCategories.ToListAsync();
        }

        public async Task<ProductCategory> GetByIdAsync(int id)
        {
            var productCategory = await dbContext.ProductCategories.FindAsync(id);
            return productCategory;
        }

        public void Update(ProductCategory entity)
        {
            dbContext.Attach(entity);

            dbContext.Entry(entity).State = EntityState.Modified;

            dbContext.SaveChanges();
        }
    }
}
