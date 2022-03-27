using Microsoft.EntityFrameworkCore;
using ShoppingCart.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.API.Services
{
    public class AsyncShoppingCartService : IAsyncShoppingCartService
    {
        private readonly AppDbContext _context;

        public AsyncShoppingCartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShoppingItem>> GetAllItemsAsync() => await _context.ShoppingItem.AsNoTracking().ToListAsync();

        public async Task<ShoppingItem> GetByIdAsync(Guid id) => await _context.ShoppingItem.FirstOrDefaultAsync(s => s.Id == id);

        public async Task<ShoppingItem> AddAsync(ShoppingItem newItem)
        {
            newItem.Id = Guid.NewGuid();
            _context.ShoppingItem.Add(newItem);
            await _context.SaveChangesAsync();
            return newItem;
        }

        public async Task<ShoppingItem> UpdateAsync(ShoppingItem updateItem)
        {
            _context.Entry(updateItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return updateItem;
        }

        public async Task RemoveAsync(ShoppingItem existingItem)
        {
            _context.Remove(existingItem);
            await _context.SaveChangesAsync();
        }
    }
}
