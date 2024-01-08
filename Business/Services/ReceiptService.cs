using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data;
using AutoMapper;
using Data.Interfaces;
using System.Linq;
using Business.Validation;
using System.ComponentModel.DataAnnotations;

namespace Business.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ReceiptService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task AddAsync(ReceiptModel model)
        {
            var receiptEntity = mapper.Map<Receipt>(model);
            await unitOfWork.ReceiptRepository.AddAsync(receiptEntity);
            await unitOfWork.SaveAsync();
        }

        public async Task AddProductAsync(int productId, int receiptId, int quantity)
        {
            var receipt = await unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);
            if (receipt == null)
            {
                throw new MarketException(string.Join("\n", "receipt cannot be null."));
            }

            if (receipt.ReceiptDetails != null)
            {
                var existingDetail = receipt.ReceiptDetails.FirstOrDefault(rd => rd.ProductId == productId);
                if (existingDetail != null)
                {
                    existingDetail.Quantity += quantity;
                }
                else 
                {
                    var createdDetail = new ReceiptDetail
                    {
                        ReceiptId = receiptId,
                        ProductId = productId,
                        Quantity = quantity
                    };

                    await unitOfWork.ReceiptDetailRepository.AddAsync(createdDetail);
                }
            }
            else
            {
                var product = await unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    throw new MarketException(string.Join("\n", "product cannot be null."));
                }

                var customer = receipt.Customer;
                var customerDiscount = customer.DiscountValue;
                var discount = product.Price - (product.Price / 100 * customerDiscount);
                
                var newDetail = new ReceiptDetail
                {
                    ReceiptId = receiptId,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    DiscountUnitPrice = discount
                };

                await unitOfWork.ReceiptDetailRepository.AddAsync(newDetail);
            }

            await unitOfWork.SaveAsync();
        }

        public async Task AddProductAsync(int productId, int receiptId, int quantity, ReceiptDetailModel receiptDetail)
        {
            var receipt = await unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);
           
            if (receipt == null)
            {
                throw new MarketException(string.Join("\n", "receipt cannot be null."));
            }

            if (receipt.ReceiptDetails != null)
            {
                var existingDetail = receipt.ReceiptDetails.FirstOrDefault(rd => rd.ProductId == productId);
                if (existingDetail != null)
                {
                    existingDetail.Quantity += quantity;
                }
                else
                {
                    var product = await unitOfWork.ProductRepository.GetByIdAsync(productId);
                    var receiptDetailEntity = mapper.Map<ReceiptDetail>(receiptDetail);
                    receiptDetailEntity.Product = product;
                    await unitOfWork.ReceiptDetailRepository.AddAsync(receiptDetailEntity);
                }
            }
            else
            {
                var product = await unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    throw new MarketException(string.Join("\n", "product cannot be null."));
                }

                var customer = receipt.Customer;
                var customerDiscount = customer.DiscountValue;
                var discount = product.Price - (product.Price / 100 * customerDiscount);

                var newDetail = new ReceiptDetail
                {
                    ReceiptId = receiptId,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    DiscountUnitPrice = discount
                };

                await unitOfWork.ReceiptDetailRepository.AddAsync(newDetail);
            }

            await unitOfWork.SaveAsync();
        }

        public async Task CheckOutAsync(int receiptId)
        {
            var receipt = await unitOfWork.ReceiptRepository.GetByIdAsync(receiptId);

            if (receipt == null)
            {
                throw new InvalidOperationException("Receipt not found");
            }

            if (receipt.IsCheckedOut)
            {
                throw new InvalidOperationException("Receipt is already checked out");
            }

            receipt.IsCheckedOut = true;

            await unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            var receipt = await unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(modelId);
            
            if (receipt != null)
            {
                var collection = receipt.ReceiptDetails;
                foreach (var detail in collection)
                {
                    unitOfWork.ReceiptDetailRepository.Delete(detail);
                }

                await unitOfWork.ReceiptRepository.DeleteByIdAsync(modelId);
                await unitOfWork.SaveAsync();
            }
        }

        public async Task<IEnumerable<ReceiptModel>> GetAllAsync()
        {
            var receipts = await unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();

            var receiptModels = mapper.Map<IEnumerable<ReceiptModel>>(receipts);

            return receiptModels;
        }

        public async Task<ReceiptModel> GetByIdAsync(int id)
        {
            var receipt = await unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(id);

            var receiptModels = mapper.Map<ReceiptModel>(receipt);

            return receiptModels;
        }

        public async Task<IEnumerable<ReceiptDetailModel>> GetReceiptDetailsAsync(int receiptId)
        {
            var receipt = await unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);
            var receiptsDetails = receipt.ReceiptDetails;

            var receiptsDetailsModels = mapper.Map<IEnumerable<ReceiptDetailModel>>(receiptsDetails);

            return receiptsDetailsModels;
        }

        public async Task<IEnumerable<ReceiptModel>> GetReceiptsByPeriodAsync(DateTime startDate, DateTime endDate)
        {
               var allReceiptDetails = await unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();
            var receiptDetails = allReceiptDetails.Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate);

            var receiptModels = mapper.Map<IEnumerable<ReceiptModel>>(receiptDetails);

            return receiptModels;
        }

        public async Task RemoveProductAsync(int productId, int receiptId, int quantity)
        {
            var receipt = await unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

            if (receipt == null)
            {
                throw new InvalidOperationException("Receipt not found");
            }

            var existingDetail = receipt.ReceiptDetails.FirstOrDefault(rd => rd.ProductId == productId);

            if (existingDetail != null)
            {
                if (existingDetail.Quantity >= quantity)
                {
                    existingDetail.Quantity -= quantity;

                    if (existingDetail.Quantity == 0)
                    {
                        unitOfWork.ReceiptDetailRepository.Delete(existingDetail);
                    }

                    await unitOfWork.SaveAsync();
                }
                else
                {
                    throw new InvalidOperationException("Quantity to remove exceeds the available quantity");
                }
            }
            else
            {
                throw new InvalidOperationException("Product not found in receipt details");
            }
        }

        public async Task<decimal> ToPayAsync(int receiptId)
        {
            var receipt = await unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);
            var allReceiptDetails = receipt.ReceiptDetails.ToList();

            decimal totalAmount = allReceiptDetails.Sum(rd => rd.Quantity * rd.DiscountUnitPrice);

            return totalAmount;
        }

        public async Task UpdateAsync(ReceiptModel model)
        {
            var receiptEntity = mapper.Map<Receipt>(model);
            unitOfWork.ReceiptRepository.Update(receiptEntity);
            await unitOfWork.SaveAsync();
        }
    }
}
