using ShoppingCart.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.API.Services
{
    public interface IAsyncShoppingCartService
    {
        Task<IEnumerable<ShoppingItem>> GetAllItemsAsync();
        Task<ShoppingItem> GetByIdAsync(Guid id);
        Task<ShoppingItem> AddAsync(ShoppingItem newItem);
        Task<ShoppingItem> UpdateAsync(ShoppingItem updateItem);
        Task RemoveAsync(ShoppingItem existingItem);
    }
}
