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
    public class CustomerRepository : ICustomerRepository
    {
        readonly TradeMarketDbContext dbContext;
        public CustomerRepository(TradeMarketDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(Customer entity)
        {
            await dbContext.Customers.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        public void Delete(Customer entity)
        {
            dbContext.Customers.Remove(entity);
            dbContext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var customer = await dbContext.Customers.FindAsync(id);

            if (customer != null)
            {
                dbContext.Customers.Remove(customer);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await dbContext.Customers.ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetAllWithDetailsAsync()
        {
            var customersWithDetails = await dbContext.Customers
            .Include(c => c.Person)
            .Include(c => c.Receipts)
                .ThenInclude(r => r.ReceiptDetails)
            .ToListAsync();

            return customersWithDetails;
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            var customer = await dbContext.Customers.FindAsync(id);
            return customer;
        }

        public async Task<Customer> GetByIdWithDetailsAsync(int id)
        {
            var customerWithDetails = await dbContext.Customers
           .Where(c => c.Id == id)
           .Include(c => c.Person)
           .Include(c => c.Receipts)
               .ThenInclude(r => r.ReceiptDetails)
           .FirstOrDefaultAsync();

            return customerWithDetails;
        }

        public async Task<IEnumerable<Customer>> GetCustomersByProductIdAsync(int productId)
        {
            var customers = await dbContext.Customers
           .Include(c => c.Receipts)
           .ThenInclude(r => r.ReceiptDetails)
           .Where(c => c.Receipts.Any(r => r.ReceiptDetails.Any(rd => rd.ProductId == productId)))
           .ToListAsync();

            return customers;
        }

        public void Update(Customer entity)
        {
            if (entity.Person != null)
            {
                var attachedPerson = dbContext.Entry(entity).Reference(c => c.Person).TargetEntry.Entity;
                attachedPerson.Id = entity.Id;
                dbContext.Entry(attachedPerson).State = EntityState.Modified;
            }
            dbContext.Attach(entity);

            dbContext.Entry(entity).State = EntityState.Modified;

            dbContext.SaveChanges();
        }
    }
}
