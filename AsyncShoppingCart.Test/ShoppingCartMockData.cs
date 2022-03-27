using ShoppingCart.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncShoppingCart.Test
{
    public class ShoppingCartMockData
    {
        public static List<ShoppingItem> GetCartItems()
        {
            return new List<ShoppingItem>
            {
                new ShoppingItem()
                {
                    Id = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"),
                    Name = "Orange Juice",
                    Manufacturer = "Orange Tree",
                    Price = 5.00M
                },
                new ShoppingItem()
                {
                    Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"),
                    Name = "Diary Milk",
                    Manufacturer = "Cow",
                    Price = 4.00M
                },
                new ShoppingItem()
                {
                    Id = new Guid("33704c4a-5b87-464c-bfb6-51971b4d18ad"),
                    Name = "Frozen Pizza",
                    Manufacturer = "Uncle Mickey",
                    Price = 12.00M
                }
            };
        }

        public static List<ShoppingItem> GetEmptyCartItems()
        {
            return new List<ShoppingItem>();
        }

        public static ShoppingItem SingleCartItem()
        {
            return new ShoppingItem()
            {
                Id = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"),
                Name = "Orange Juice",
                Manufacturer = "Orange Tree",
                Price = 5.00M
            };
        }

        public static ShoppingItem MissingNameItem()
        {
            return new ShoppingItem()
            {
                Manufacturer = "Guinness",
                Price = 19.00M
            };
        }

        public static ShoppingItem NewCartItem()
        {
            return new ShoppingItem()
            {
                Id = Guid.NewGuid(),
                Name = "Banana Juice",
                Manufacturer = "Banana Tree",
                Price = 9.00M
            };
        }
    }
}
