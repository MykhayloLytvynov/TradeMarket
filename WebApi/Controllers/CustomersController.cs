using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        readonly ICustomerService customerService;
        public CustomersController(ICustomerService customerService) 
        {
            this.customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerModel>>> Get()
        {
            return Ok(await customerService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerModel>> GetById(int id)
        {
            var model = await customerService.GetByIdAsync(id);
            if (model is null)
                return NotFound();
            return Ok(model);
        }
        
        [HttpGet("products/{id}")]
        public async Task<ActionResult<CustomerModel>> GetByProductId(int id)
        {
            return Ok(await customerService.GetCustomersByProductIdAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerModel>> Add([FromBody] CustomerModel value)
        {
            
            var validator = new CustomerServiceValidator();

            var validationResult = validator.Validate(value);

            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join("\n", validationResult.Errors));
            }
            await customerService.AddAsync(value);
            return Ok(value);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromBody] CustomerModel value)
        {
            var validator = new CustomerServiceValidator();

            var validationResult = validator.Validate(value);

            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join("\n", validationResult.Errors));
            }
            await customerService.UpdateAsync(value);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await customerService.DeleteAsync(id);
            return Ok();
        }
    }
}
