using Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Business.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
        readonly IReceiptService receiptService;

        public ReceiptsController(IReceiptService receiptService)
        {
            this.receiptService = receiptService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceiptModel>>> Get()
        {
            return Ok(await receiptService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReceiptModel>> GetById(int id)
        {
            return Ok(await receiptService.GetByIdAsync(id));
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<IEnumerable<ReceiptDetailModel>>> GetAllDetails(int id)
        {
            return Ok(await receiptService.GetReceiptDetailsAsync(id));
        }

        [HttpGet("{id}/sum")]
        public async Task<ActionResult<decimal>> GetReceiptSum(int id)
        {
            decimal sum = await receiptService.ToPayAsync(id);
            return Ok(sum);
        }

        [HttpGet("period")]
        public async Task<ActionResult<IEnumerable<ReceiptModel>>> ReceiptsByPeriod(DateTime startDate, DateTime endDate)
        {
            var receiptsByPeriod = await receiptService.GetReceiptsByPeriodAsync(startDate, endDate);
            return Ok(receiptsByPeriod);
        }

        [HttpPost]
        public async Task<ActionResult<ReceiptModel>> Add([FromBody] ReceiptModel value)
        {
            await receiptService.AddAsync(value);
            return Ok(value);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromBody] ReceiptModel value)
        {
            await receiptService.UpdateAsync(value);
            return Ok();
        }

        [HttpPut("{id}/products/add/{productId}/{quantity}")]
        public async Task<ActionResult> AddProductToReceipt(int id, int productId, int quantity, [FromBody] ReceiptDetailModel receiptDetail)
        {
            await receiptService.AddProductAsync(productId,id, quantity, receiptDetail);
            return Ok();
        }

        [HttpPut("{id}/products/remove/{productId}/{quantity}")]
        public async Task<ActionResult> RemoveProductFromReceipt(int id, int productId, int quantity)
        {
            await receiptService.RemoveProductAsync(productId, id, quantity);
            return Ok();
        }

        [HttpPut("{id}/checkout")]
        public async Task<ActionResult> CheckoutReceipt(int id)
        {
            await receiptService.CheckOutAsync(id);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await receiptService.DeleteAsync(id);
            return Ok();
        }
    }
}
