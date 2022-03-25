using ShoppingCart.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.API.Services
{
    public interface IShoppingCartService
    {
        IEnumerable<ShoppingItem> GetAllItems();
        ShoppingItem Add(ShoppingItem newItem);
        ShoppingItem Update(ShoppingItem existingItem);
        ShoppingItem GetById(Guid id);
        void Remove(Guid id);
    }
}
