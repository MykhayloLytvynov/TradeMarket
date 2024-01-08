using Business.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Business.Models;
using System;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        readonly IStatisticService statisticService;

        public StatisticsController(IStatisticService statisticService)
        {
            this.statisticService = statisticService;
        }

        [HttpGet("popularProducts")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetMostPopularProducts([FromQuery] int productCount)
        {
            var popularProducts = await statisticService.GetMostPopularProductsAsync(productCount);
            return Ok(popularProducts);
        }

        [HttpGet("customer/{id}/{productCount}")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetMostFavoriteProductsForCustomer(int id, int productCount)
        {
            var favoriteProducts = await statisticService.GetCustomersMostPopularProductsAsync(id, productCount);
            return Ok(favoriteProducts);
        }

        [HttpGet("activity/{customerCount}")]
        public async Task<ActionResult<IEnumerable<CustomerActivityModel>>> GetMostActiveCustomers(int customerCount, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var mostActiveCustomers = await statisticService.GetMostValuableCustomersAsync(customerCount, startDate, endDate);
            return Ok(mostActiveCustomers);
        }

        [HttpGet("income/{categoryId}")]
        public async Task<ActionResult<decimal>> GetIncomeOfCategoryInPeriod(int categoryId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var income = await statisticService.GetIncomeOfCategoryInPeriod(categoryId, startDate, endDate);
            return Ok(income);
        }

    }
}
