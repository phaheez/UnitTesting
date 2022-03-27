using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.API.Models;
using ShoppingCart.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.API.Controllers
{
    [Route("api/shopping-cart")]
    [ApiController]
    public class AsyncShoppingCartController : ControllerBase
    {
        private readonly IAsyncShoppingCartService _service;

        public AsyncShoppingCartController(IAsyncShoppingCartService service) => _service = service;

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var items = await _service.GetAllItemsAsync();
            if (items.Count() == 0)
            {
                return NoContent();
            }
            return Ok(items);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetAsync([FromRoute] Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item is null) return NotFound();
            return Ok(item);
        }

        [HttpPost("add")]
        public async Task<IActionResult> PostAsync([FromBody] ShoppingItem item)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newItem = await _service.AddAsync(item);
            return Ok(newItem);
        }

        [HttpPut("update")]
        public async Task<IActionResult> PutAsync([FromBody] ShoppingItem item)
        {
            var existingItem = await _service.GetByIdAsync(item.Id);
            if (existingItem is null) return NotFound();
            var updatedItem = await _service.UpdateAsync(item);
            return Ok(updatedItem);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> RemoveAsync(Guid id)
        {
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem is null) return NotFound();
            await _service.RemoveAsync(existingItem);
            return NoContent();
        }
    }
}
