using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Data.Entities;
using Data.Interfaces;
using FluentValidation;

namespace Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task AddAsync(ProductModel model)
        {
            var validator = new ProductServiceValidator();

            if (model == null)
            {
                throw new MarketException(nameof(model));
            }
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                throw new MarketException(string.Join("\n", validationResult.Errors));
            }
            var productEntity = mapper.Map<Product>(model);
            await unitOfWork.ProductRepository.AddAsync(productEntity);
            await unitOfWork.SaveAsync();
        }

        public async Task AddCategoryAsync(ProductCategoryModel categoryModel)
        {
            var validator = new ProductCategoryValidator();

            if (categoryModel == null)
            {
                throw new MarketException(nameof(categoryModel));
            }
            var validationResult = validator.Validate(categoryModel);

            if (!validationResult.IsValid)
            {
                throw new MarketException(string.Join("\n", validationResult.Errors));
            }
            var categoryEntity = mapper.Map<ProductCategory>(categoryModel);
            await unitOfWork.ProductCategoryRepository.AddAsync(categoryEntity);
            await unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            await unitOfWork.ProductRepository.DeleteByIdAsync(modelId);
            await unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<ProductModel>> GetAllAsync()
        {
            var products = await unitOfWork.ProductRepository.GetAllWithDetailsAsync();

            var productModels = mapper.Map<IEnumerable<ProductModel>>(products);

            return productModels;
        }

        public async Task<IEnumerable<ProductCategoryModel>> GetAllProductCategoriesAsync()
        {
            var productCategories = await unitOfWork.ProductCategoryRepository.GetAllAsync();

            var productCategoryModels = mapper.Map<IEnumerable<ProductCategoryModel>>(productCategories);

            return productCategoryModels;
        }

        public async Task<IEnumerable<ProductModel>> GetByFilterAsync(FilterSearchModel filterSearch)
        {
            var query = await unitOfWork.ProductRepository.GetAllWithDetailsAsync();

            if (filterSearch.CategoryId.HasValue && filterSearch.CategoryId != 0)
            {
                query =  query.Where(product => product.ProductCategoryId == filterSearch.CategoryId);
            }

            if (filterSearch.MinPrice.HasValue && filterSearch.MinPrice != 0)
            {
                query = query.Where(product => product.Price >= filterSearch.MinPrice.Value);
            }

            if (filterSearch.MaxPrice.HasValue && filterSearch.MaxPrice != 0)
            {
                query = query.Where(product => product.Price <= filterSearch.MaxPrice.Value);
            }

            var productModels = mapper.Map<IEnumerable<ProductModel>>(query);

            return productModels;
        }

        public async Task<ProductModel> GetByIdAsync(int id)
        {
            var product = await unitOfWork.ProductRepository.GetByIdWithDetailsAsync(id);

            var productModel = mapper.Map<ProductModel>(product);

            return productModel;
        }

        public async Task RemoveCategoryAsync(int categoryId)
        {
            await unitOfWork.ProductCategoryRepository.DeleteByIdAsync(categoryId);
            await unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(ProductModel model)
        {
            var validator = new ProductServiceValidator();

            if (model == null)
            {
                throw new MarketException(nameof(model));
            }
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                throw new MarketException(string.Join("\n", validationResult.Errors));
            }

            var productEntity = mapper.Map<Product>(model);
            unitOfWork.ProductRepository.Update(productEntity);
            await unitOfWork.SaveAsync();
        }

        public async Task UpdateCategoryAsync(ProductCategoryModel categoryModel)
        {
            var validator = new ProductCategoryValidator();

            if (categoryModel == null)
            {
                throw new MarketException(nameof(categoryModel));
            }
            var validationResult = validator.Validate(categoryModel);

            if (!validationResult.IsValid)
            {
                throw new MarketException(string.Join("\n", validationResult.Errors));
            }
            var productCategoryEntity = mapper.Map<ProductCategory>(categoryModel);
            unitOfWork.ProductCategoryRepository.Update(productCategoryEntity);
            await unitOfWork.SaveAsync();
        }
    }
}
