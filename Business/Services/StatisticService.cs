using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public StatisticService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<ProductModel>> GetCustomersMostPopularProductsAsync(int productCount, int customerId)
        {
            var allCustomerReceipts = await unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();
            var customerReceipts = allCustomerReceipts.Where(r => r.CustomerId == customerId).ToList();

            var allReceiptDetails = customerReceipts.SelectMany(r => r.ReceiptDetails);

            var productCounts = allReceiptDetails
                .GroupBy(rd => rd.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    PurchaseCount = g.Sum(rd => rd.Quantity)
                })
                .OrderByDescending(g => g.PurchaseCount)
                .Take(productCount);

            var popularProductIds = productCounts
            .OrderByDescending(pc => pc.PurchaseCount)
            .Select(pc => pc.ProductId)
            .Distinct()
            .ToList();

            var allPopularProducts = customerReceipts
            .SelectMany(receipt => receipt.ReceiptDetails)
            .Where(detail => popularProductIds.Contains(detail.ProductId))
            .Select(detail => detail.Product)
            .Distinct()
            .ToList();

            var popularProducts = allPopularProducts
            .Where(product => popularProductIds.Contains(product.Id))
            .OrderBy(product => popularProductIds.IndexOf(product.Id))
            .ToList();

            var productModels = mapper.Map<IEnumerable<ProductModel>>(popularProducts);

            return productModels;
        }

        public async Task<decimal> GetIncomeOfCategoryInPeriod(int categoryId, DateTime startDate, DateTime endDate)
        {
            var receipts = await unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();
            receipts = receipts.Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate);

            decimal totalIncome = receipts
                .SelectMany(r => r.ReceiptDetails) // Flatten the ReceiptDetails
                .Where(rd => rd.Product.Category.Id == categoryId) // Filter by categoryId
                .Sum(rd => (rd.DiscountUnitPrice * rd.Quantity)); // Calculate the income for each ReceiptDetail and sum

            return totalIncome;
        }

        public async Task<IEnumerable<ProductModel>> GetMostPopularProductsAsync(int productCount)
        {
            var allReceiptDetails = await unitOfWork.ReceiptDetailRepository.GetAllWithDetailsAsync();

            var productCounts = allReceiptDetails
                .GroupBy(rd => rd.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    PurchaseCount = g.Sum(rd => rd.Quantity)
                })
                .OrderByDescending(g => g.PurchaseCount)
                .Take(productCount);

            var popularProductIds = productCounts
            .OrderByDescending(pc => pc.PurchaseCount)
            .Select(pc => pc.ProductId)
            .Distinct()
            .ToList();

            var allPopularProducts = allReceiptDetails
            .Where(detail => popularProductIds.Contains(detail.ProductId))
            .Select(detail => detail.Product)
            .Distinct()
            .ToList();

            var popularProducts = allPopularProducts
            .Where(product => popularProductIds.Contains(product.Id))
            .OrderBy(product => popularProductIds.IndexOf(product.Id))
            .ToList();
            var productModels = mapper.Map<IEnumerable<ProductModel>>(popularProducts);

            return productModels;
        }

        public async Task<IEnumerable<CustomerActivityModel>> GetMostValuableCustomersAsync(int customerCount, DateTime startDate, DateTime endDate)
        {
            var receipts = await unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();

            receipts = receipts.Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate);

            if (unitOfWork.PersonRepository != null) 
            {
                await unitOfWork.PersonRepository.GetAllAsync();
            }
            var customerSpending = receipts
                .GroupBy(r => r.CustomerId)
                .Select(g => new CustomerActivityModel
                {
                    CustomerId = g.Key,
                    CustomerName = g.First().Customer.Person.Name + " " + g.First().Customer.Person.Surname,
                    ReceiptSum = g.Sum(r => r.ReceiptDetails.Sum(rd => rd.DiscountUnitPrice * rd.Quantity))
                })
                .OrderByDescending(c => c.ReceiptSum)
                .Take(customerCount)
                .ToList();

            return customerSpending;
        }
    }
}
