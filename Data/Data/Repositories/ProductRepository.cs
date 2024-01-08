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
    public class ProductRepository : IProductRepository
    {
        readonly TradeMarketDbContext dbContext;
        public ProductRepository(TradeMarketDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(Product entity)
        {
            var category = await dbContext.ProductCategories.FindAsync(entity.ProductCategoryId);

            if (category == null)
            {
                category = new ProductCategory { 

                };
                await dbContext.ProductCategories.AddAsync(category);
            }

            entity.Category = category;


            await dbContext.Products.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        public void Delete(Product entity)
        {
            dbContext.Products.Remove(entity);
            dbContext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var product = await dbContext.Products.FindAsync(id);

            if (product != null)
            {
                dbContext.Products.Remove(product);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await dbContext.Products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
        {
            var productsWithDetails = await dbContext.Products
            .Include(c => c.ReceiptDetails)
            .Include(c => c.Category)
            .ToListAsync();

            return productsWithDetails;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            var product= await dbContext.Products.FindAsync(id);
            return product;
        }

        public async Task<Product> GetByIdWithDetailsAsync(int id)
        {
            var productsWithDetails = await dbContext.Products
            .Where(c => c.Id == id)
            .Include(c => c.ReceiptDetails)
            .Include(c => c.Category)
            .FirstOrDefaultAsync();

            return productsWithDetails;
        }

        public void Update(Product entity)
        {
            if (entity.Category != null)
            {
                var attachedCategory = dbContext.Entry(entity).Reference(c => c.Category).TargetEntry.Entity;
                attachedCategory.Id = entity.Id;
                dbContext.Entry(attachedCategory).State = EntityState.Modified;
            }

            dbContext.Attach(entity);

            dbContext.Entry(entity).State = EntityState.Modified;

            dbContext.SaveChanges();
        }
    }
}
