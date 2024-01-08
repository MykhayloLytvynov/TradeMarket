using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Data;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task AddAsync(CustomerModel model)
        {
            var validator = new CustomerServiceValidator();

            if (model == null)
            {
                throw new MarketException(nameof(model));
            }
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                throw new MarketException(string.Join("\n", validationResult.Errors));
            }

            var customerEntity = mapper.Map<Customer>(model);
            await unitOfWork.CustomerRepository.AddAsync(customerEntity);
            await unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            await unitOfWork.CustomerRepository.DeleteByIdAsync(modelId);
            await unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<CustomerModel>> GetAllAsync()
        {
            var customers = await unitOfWork.CustomerRepository.GetAllWithDetailsAsync();

            var customerModels = mapper.Map<IEnumerable<CustomerModel>>(customers);

            return customerModels;
        }

        public async Task<CustomerModel> GetByIdAsync(int id)
        {
            var customer = await unitOfWork.CustomerRepository.GetByIdWithDetailsAsync(id);

            var customerModels = mapper.Map<CustomerModel>(customer);

            return customerModels;
        }

        public async Task<IEnumerable<CustomerModel>> GetCustomersByProductIdAsync(int productId)
        {
            var allCustomers = await unitOfWork.CustomerRepository.GetAllWithDetailsAsync();

            var customers = allCustomers
                .Where(c => c.Receipts.Any(r => r.ReceiptDetails.Any(rd => rd.ProductId == productId)));

            var customerModels = mapper.Map<IEnumerable<CustomerModel>>(customers);

            return customerModels;
        }

        public async Task UpdateAsync(CustomerModel model)
        {
            var validator = new CustomerServiceValidator();

            if (model == null)
            {
                throw new MarketException(nameof(model));
            }
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                throw new MarketException(string.Join("\n", validationResult.Errors));
            }
            var customerEntity = mapper.Map<Customer>(model);
            unitOfWork.CustomerRepository.Update(customerEntity);
            await unitOfWork.SaveAsync();
        }
    }
}
