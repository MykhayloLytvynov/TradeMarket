using Data.Data;
using Data.Repositories;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Data
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly TradeMarketDbContext dbContext;
        CustomerRepository customerRepository;
        PersonRepository personRepository;
        ProductRepository productRepository;
        ProductCategoryRepository productCategoryRepository;
        ReceiptRepository receiptRepository;
        ReceiptDetailRepository receiptDetailRepository;

        public UnitOfWork(TradeMarketDbContext dbContext) 
        {
            this.dbContext = dbContext;
        }

        public ICustomerRepository CustomerRepository => customerRepository = new CustomerRepository(dbContext);

        public IPersonRepository PersonRepository => personRepository = new PersonRepository(dbContext);

        public IProductRepository ProductRepository => productRepository = new ProductRepository(dbContext);

        public IProductCategoryRepository ProductCategoryRepository => productCategoryRepository = new ProductCategoryRepository(dbContext);

        public IReceiptRepository ReceiptRepository => receiptRepository = new ReceiptRepository(dbContext);

        public IReceiptDetailRepository ReceiptDetailRepository => receiptDetailRepository = new ReceiptDetailRepository(dbContext);

        public async Task SaveAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
