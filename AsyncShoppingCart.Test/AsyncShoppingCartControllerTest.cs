using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShoppingCart.API.Controllers;
using ShoppingCart.API.Models;
using ShoppingCart.API.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AsyncShoppingCart.Test
{
    public class AsyncShoppingCartControllerTest
    {
        private readonly AsyncShoppingCartController _controller;
        private readonly Mock<IAsyncShoppingCartService> _mockService;

        public AsyncShoppingCartControllerTest()
        {
            _mockService = new Mock<IAsyncShoppingCartService>();
            _controller = new AsyncShoppingCartController(_mockService.Object);
        }

        #region Get All Items

        [Fact]
        public async Task GetAllAsync_ShouldReturn200OkStatus()
        {
            // Arrange
            var cartItems = ShoppingCartMockData.GetCartItems();
            _mockService.Setup(service => service.GetAllItemsAsync()).ReturnsAsync(cartItems);

            // Act
            var result = await _controller.GetAllAsync() as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(result);
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetAllAsync_WhenItemIsEmpty_ShouldReturn204NoContentStatus()
        {
            // Arrange
            var cartItems = ShoppingCartMockData.GetEmptyCartItems();
            _mockService.Setup(service => service.GetAllItemsAsync()).ReturnsAsync(cartItems);

            // Act
            var result = await _controller.GetAllAsync() as NoContentResult;

            // Assert
            Assert.IsType<NoContentResult>(result);
            result.StatusCode.Should().Be(204);
            _mockService.Verify(service => service.GetAllItemsAsync(), Times.Exactly(1));
        }

        [Fact]
        public async Task GetAllAsync_ReturnShoppingCartItems()
        {
            // Arrange
            var cartItems = ShoppingCartMockData.GetCartItems();
            _mockService.Setup(service => service.GetAllItemsAsync()).ReturnsAsync(cartItems);

            // Act
            var okResult = await _controller.GetAllAsync() as OkObjectResult;
            var result = okResult.Value as List<ShoppingItem>;

            // Assert
            Assert.IsType<List<ShoppingItem>>(okResult.Value);
            Assert.Equal(ShoppingCartMockData.GetCartItems().Count, result.Count);
            result.Should().BeOfType<List<ShoppingItem>>();
            result.Should().HaveCount(ShoppingCartMockData.GetCartItems().Count);
        }

        #endregion

        #region Get Item By Id

        [Fact]
        public async Task GetByIdAsync_UnknownGuidPassed_Return404NotFoundStatus()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var result = await _controller.GetAsync(id) as NotFoundResult;

            // Assert
            Assert.IsType<NotFoundResult>(result);
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingGuidPassed_Return200OkStatus()
        {
            // Arrange
            var singleItem = ShoppingCartMockData.SingleCartItem();
            _mockService.Setup(service => service.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(singleItem);

            // Act
            var okObjectResult = await _controller.GetAsync(singleItem.Id);
            var result = okObjectResult as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(result);
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingGuidPassed_ReturnRightItem()
        {
            // Arrange
            var singleItem = ShoppingCartMockData.SingleCartItem();
            _mockService.Setup(service => service.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(singleItem);

            // Act
            var okResult = await _controller.GetAsync(singleItem.Id) as OkObjectResult;
            var result = okResult.Value as ShoppingItem;

            // Assert
            Assert.Same(singleItem, result);
            Assert.Equal(singleItem.Id, result.Id);
            Assert.Equal(singleItem.Name, result.Name);
            Assert.Equal(singleItem.Manufacturer, result.Manufacturer);
            Assert.Equal(singleItem.Price, result.Price);
            result.Should().BeEquivalentTo(singleItem);
        }

        #endregion

        #region Add Item

        [Fact]
        public async Task PostAsync_InvalidObjectPassed_Return400BadRequestStatus()
        {
            // Arrange
            var nameMissingItem = ShoppingCartMockData.MissingNameItem();
            _mockService.Setup(service => service.AddAsync(It.IsAny<ShoppingItem>()));
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.PostAsync(nameMissingItem) as BadRequestObjectResult;

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task PostAsync_ValidObjectPassed_Return200OkStatus()
        {
            // Arrange
            var newItem = ShoppingCartMockData.NewCartItem();
            _mockService.Setup(service => service.AddAsync(It.IsAny<ShoppingItem>())).ReturnsAsync(newItem).Verifiable();

            // Act
            var result = await _controller.PostAsync(newItem) as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(result);
            result.StatusCode.Should().Be(200);

            _mockService.Verify(_ => _.AddAsync(newItem), Times.Exactly(1));
        }

        [Fact]
        public async Task PostAsync_ValidObjectPassed_ReturnCreatedItem()
        {
            // Arrange
            var newItem = ShoppingCartMockData.NewCartItem();
            _mockService.Setup(service => service.AddAsync(It.IsAny<ShoppingItem>())).ReturnsAsync(newItem).Verifiable();

            // Act
            var okObjectResult = await _controller.PostAsync(newItem)as OkObjectResult;
            var result = okObjectResult.Value as ShoppingItem;

            // Assert
            Assert.Same(newItem, result);
            result.Should().BeEquivalentTo(newItem);

            _mockService.Verify(_ => _.AddAsync(newItem), Times.Exactly(1));
        }

        #endregion

        #region Update Item

        [Fact]
        public async Task Update_ValidObjectPassed_Return200OkStatus()
        {
            // Arrange
            var testItem = ShoppingCartMockData.SingleCartItem();
            _mockService.Setup(service => service.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(testItem).Verifiable();
            _mockService.Setup(service => service.UpdateAsync(It.IsAny<ShoppingItem>())).ReturnsAsync(testItem).Verifiable();

            // Act
            var okObjectResult = await _controller.PutAsync(testItem);
            var result = okObjectResult as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(okObjectResult);
            result.StatusCode.Should().Be(200);
            _mockService.Verify();
        }

        [Fact]
        public async Task Update_ValidObjectPassed_ReturnedRightItem()
        {
            // Arrange
            var testItem = ShoppingCartMockData.SingleCartItem();
            _mockService.Setup(service => service.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(testItem);
            _mockService.Setup(service => service.UpdateAsync(It.IsAny<ShoppingItem>())).ReturnsAsync(testItem);

            // Act
            var result = await _controller.PutAsync(testItem) as OkObjectResult;
            var item = result.Value as ShoppingItem;

            // Assert
            Assert.IsType<ShoppingItem>(item);
            Assert.Same(testItem, item);
            Assert.Equal(testItem.Id, item.Id);
            Assert.Equal(testItem.Name, item.Name);
            Assert.Equal(testItem.Manufacturer, item.Manufacturer);
            Assert.Equal(testItem.Price, item.Price);
        }

        #endregion

        #region Delete Item

        [Fact]
        public async Task RemoveAsync_NotExistingGuidPassed_Return404NotFoundStatus()
        {
            // Arrange
            var notExistingGuid = Guid.NewGuid();

            // Act
            var notFoundResult = await _controller.RemoveAsync(notExistingGuid);
            var result = notFoundResult as NotFoundResult;
            
            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task RemoveAsync_ExistingGuidPassed_Return204NoContentStatus()
        {
            // Arrange
            var existingItem = ShoppingCartMockData.SingleCartItem();
            _mockService.Setup(service => service.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(existingItem).Verifiable();
            _mockService.Setup(service => service.RemoveAsync(It.IsAny<ShoppingItem>())).Verifiable();

            // Act
            var noContentResponse = await _controller.RemoveAsync(existingItem.Id);
            var result = noContentResponse as NoContentResult;

            // Assert
            Assert.IsType<NoContentResult>(noContentResponse);
            result.StatusCode.Should().Be(204);
            _mockService.Verify();
        }

        #endregion
    }
}
