using Microsoft.AspNetCore.Mvc;
using Moq;
using ShoppingCart.API.Controllers;
using ShoppingCart.API.Models;
using ShoppingCart.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ShoppingCart.Test
{
    public class ShoppingCartControllerTest
    {
        private readonly ShoppingCartController _controller;
        private readonly Mock<IShoppingCartService> _mockService;

        public ShoppingCartControllerTest()
        {
            _mockService = new Mock<IShoppingCartService>();
            //_mockService = new Mock<IShoppingCartService>() { DefaultValue = DefaultValue.Mock };
            _controller = new ShoppingCartController(_mockService.Object);
        }

        #region Get All Items

        [Fact]
        public void Get_WhenCalled_ReturnsOkResult()
        {
            // Act
            var okResult = _controller.Get();

            // Assert
            Assert.IsType<OkObjectResult>(okResult);
        }

        [Fact]
        public void Get_WhenCalled_ReturnsAllItems()
        {
            // Arrange
            _mockService.Setup(service => service.GetAllItems())
                .Returns(new List<ShoppingItem> { new ShoppingItem(), new ShoppingItem(), new ShoppingItem() });

            // Act
            var okResult = _controller.Get() as OkObjectResult;
            
            // Assert
            var items = Assert.IsType<List<ShoppingItem>>(okResult.Value);
            Assert.Equal(3, items.Count);
        }

        #endregion

        #region Get Item By Id

        [Fact]
        public void GetById_UnknownGuidPassed_ReturnsNotFoundResult()
        {
            // Act
            var notFoundResult = _controller.Get(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
        }

        [Fact]
        public void GetById_ExistingGuidPassed_ReturnsOkResult()
        {
            // Arrange
            var testGuid = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200");
            _mockService.Setup(service => service.GetById(It.IsAny<Guid>()))
                .Returns(new ShoppingItem());

            // Act
            var okResult = _controller.Get(testGuid) as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(okResult);
        }

        [Fact]
        public void GetById_ExistingGuidPassed_ReturnsRightItem()
        {
            // Arrange
            var testGuid = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200");
            _mockService.Setup(service => service.GetById(It.IsAny<Guid>()))
                .Returns(new ShoppingItem { Id = testGuid });

            // Act
            var okResult = _controller.Get(testGuid) as OkObjectResult;
            var item = okResult.Value as ShoppingItem;

            // Assert
            Assert.IsType<ShoppingItem>(item);
            Assert.Equal(testGuid, item.Id);
        }

        #endregion

        #region Add Item

        [Fact]
        public void Add_InvalidObjectPassed_ReturnsBadRequest()
        {
            // Arrange
            var nameMissingItem = new ShoppingItem
            {
                Manufacturer = "Guinness",
                Price = 12.00M
            };
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = _controller.Post(nameMissingItem);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Add_ValidObjectPassed_ReturnsCreatedResponse()
        {
            // Arrange
            var testItem = new ShoppingItem
            {
                Name = "Guinness Original 6 Pack",
                Manufacturer = "Guinness",
                Price = 12.00M
            };
            _mockService.Setup(service => service.Add(It.IsAny<ShoppingItem>()))
                .Returns(testItem);

            // Act
            var result = _controller.Post(testItem);

            // Assert
            var createdResponse = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Null(createdResponse.ControllerName);
            Assert.Equal("Get", createdResponse.ActionName);
        }

        [Fact]
        public void Add_ValidObjectPassed_ReturnedResponseHasCreatedItem()
        {
            // Arrange
            var testItem = new ShoppingItem()
            {
                Name = "Guinness Original 6 Pack",
                Manufacturer = "Guinness",
                Price = 12.00M
            };
            _mockService.Setup(service => service.Add(It.IsAny<ShoppingItem>()))
                .Returns(testItem);

            // Act
            var createdResponse = _controller.Post(testItem) as CreatedAtActionResult;
            var item = createdResponse.Value as ShoppingItem;

            // Assert
            Assert.IsType<ShoppingItem>(item);
            Assert.Equal(testItem.Name, item.Name);
            Assert.Equal(testItem.Manufacturer, item.Manufacturer);
            Assert.Equal(testItem.Price, item.Price);
        }

        #endregion

        #region Update Item

        [Fact]
        public void Update_ValidObjectPassed_ReturnsOkResult()
        {
            // Arrange
            var testItem = new ShoppingItem
            {
                Id = new Guid("33704c4a-5b87-464c-bfb6-51971b4d18ad"),
                Name = "Frozen Pizza",
                Manufacturer = "Uncle Mickey",
                Price = 12.00M
            };

            _mockService.Setup(service => service.GetById(testItem.Id)).Returns(new ShoppingItem()
            {
                Id = new Guid("33704c4a-5b87-464c-bfb6-51971b4d18ad"),
                Name = "Frozen Pizza",
                Manufacturer = "Uncle Mickey",
                Price = 12.00M
            });

            _mockService.Setup(service => service.Update(It.IsAny<ShoppingItem>()))
                .Returns(testItem);

            // Act
            var result = _controller.Put(testItem);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Update_ValidObjectPassed_ReturnedRightItem()
        {
            // Arrange
            var testItem = new ShoppingItem
            {
                Id = new Guid("33704c4a-5b87-464c-bfb6-51971b4d18ad"),
                Name = "Frozen Pizza",
                Manufacturer = "Uncle Mickey",
                Price = 12.00M
            };

            _mockService.Setup(service => service.GetById(testItem.Id)).Returns(new ShoppingItem()
            {
                Id = new Guid("33704c4a-5b87-464c-bfb6-51971b4d18ad"),
                Name = "Frozen Pizza",
                Manufacturer = "Uncle Mickey",
                Price = 12.00M
            });

            _mockService.Setup(service => service.Update(It.IsAny<ShoppingItem>()))
                .Returns(testItem);

            // Act
            var result = _controller.Put(testItem);
            var okResult = result as OkObjectResult;
            var item = okResult.Value as ShoppingItem;

            // Assert
            Assert.IsType<ShoppingItem>(item);
            Assert.Equal(testItem.Id, item.Id);
            Assert.Equal(testItem.Name, item.Name);
            Assert.Equal(testItem.Manufacturer, item.Manufacturer);
            Assert.Equal(testItem.Price, item.Price);
        }

        #endregion

        #region Delete Item

        [Fact]
        public void Remove_NotExistingGuidPassed_ReturnsNotFoundResponse()
        {
            // Arrange
            var notExistingGuid = Guid.NewGuid();

            // Act
            var badResponse = _controller.Remove(notExistingGuid);

            // Assert
            Assert.IsType<NotFoundResult>(badResponse);
        }

        [Fact]
        public void Remove_ExistingGuidPassed_ReturnsNoContentResult()
        {
            // Arrange
            var existingGuid = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200");
            
            _mockService.Setup(service => service.GetById(existingGuid)).Returns(new ShoppingItem()
            {
                Id = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"),
                Name = "Orange Juice",
                Manufacturer = "Orange Tree",
                Price = 5.00M
            });

            _mockService.Setup(service => service.Remove(It.IsAny<Guid>()));

            // Act
            var noContentResponse = _controller.Remove(existingGuid);

            // Assert
            Assert.IsType<NoContentResult>(noContentResponse);
        }

        //[Fact]
        //public void Remove_ExistingGuidPassed_RemovesOneItem()
        //{
        //    // Arrange
        //    var existingGuid = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200");

        //    // Act
        //    var okResponse = _controller.Remove(existingGuid);

        //    // Assert
        //    Assert.Equal(2, _mockService.GetAllItems().Count());
        //}

        #endregion
    }
}
