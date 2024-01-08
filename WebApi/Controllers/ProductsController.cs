using Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Business.Services;
using System.Linq;
using Business.Validation;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class ProductsController : ControllerBase
    {
        readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetById(int id)
        {
            var model = await productService.GetByIdAsync(id);
            if (model is null)
                return NotFound();
            return Ok(model);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetAllCategories()
        {
            return Ok(await productService.GetAllProductCategoriesAsync());
        }

        [HttpPost]
        public async Task<ActionResult<ProductModel>> Add([FromBody] ProductModel value)
        {
            var validator = new ProductServiceValidator();

            var validationResult = validator.Validate(value);

            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join("\n", validationResult.Errors));
            }
            await productService.AddAsync(value);
            return Ok(value);
        }

        [HttpPost("categories")]
        public async Task<ActionResult<ProductCategoryModel>> AddCategory([FromBody] ProductCategoryModel categoryModel)
        {
            await productService.AddCategoryAsync(categoryModel);
            return Ok(categoryModel);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await productService.DeleteAsync(id);
            return Ok();
        }


        [HttpDelete("categories/{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            await productService.RemoveCategoryAsync(id);
            return Ok();
        }

        [HttpGet]
        [ActionName("GetByFilter")]
        public async Task<ActionResult<ProductModel>> GetByFilter([FromQuery(Name = "categoryId")] int categoryId, [FromQuery(Name = "minPrice")] int minPrice, [FromQuery(Name = "maxPrice")] int maxPrice)
        {
            var filterSearch = new FilterSearchModel
            {
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            var filteredProducts = await productService.GetByFilterAsync(filterSearch);

            return Ok(filteredProducts);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromBody] ProductModel value)
        {
            var validator = new ProductServiceValidator();

            var validationResult = validator.Validate(value);

            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join("\n", validationResult.Errors));
            }
            await productService.UpdateAsync(value);
            return Ok();
        }

        [HttpPut("categories/{id}")]
        public async Task<ActionResult> UpdateCategory([FromBody] ProductCategoryModel updatedCategory)
        {
            await productService.UpdateCategoryAsync(updatedCategory);
            return Ok();
        }
    }
}
