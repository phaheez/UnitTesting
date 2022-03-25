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
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _service;

        public ShoppingCartController(IShoppingCartService service) => _service = service;

        [HttpGet]
        public IActionResult Get()
        {
            var items = _service.GetAllItems();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public IActionResult Get([FromRoute] Guid id)
        {
            var item = _service.GetById(id);
            if (item is null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ShoppingItem item)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newItem = _service.Add(item);
            return CreatedAtAction("Get", new { id = newItem.Id }, newItem);
        }

        [HttpPut]
        public IActionResult Put([FromBody] ShoppingItem item)
        {
            var existingItem = _service.GetById(item.Id);
            if (existingItem is null) return NotFound();
            var updatedItem = _service.Update(item);
            return Ok(updatedItem);
        }

        [HttpDelete("{id}")]
        public IActionResult Remove(Guid id)
        {
            var existingItem = _service.GetById(id);
            if (existingItem is null) return NotFound();
            _service.Remove(id);
            return NoContent();
        }
    }
}
