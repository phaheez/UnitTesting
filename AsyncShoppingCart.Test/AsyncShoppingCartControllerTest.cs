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
            _mockService.Setup(service => service.GetAllItemsAsync()).ReturnsAsync(ShoppingCartMockData.GetCartItems());

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
            _mockService.Setup(service => service.GetAllItemsAsync()).ReturnsAsync(ShoppingCartMockData.GetEmptyCartItems());

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
            _mockService.Setup(service => service.GetAllItemsAsync()).ReturnsAsync(ShoppingCartMockData.GetCartItems());

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
            var testGuid = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200");
            var singleItem = ShoppingCartMockData.SingleCartItem();
            _mockService.Setup(service => service.GetByIdAsync(testGuid)).ReturnsAsync(singleItem);

            // Act
            var okObjectResult = await _controller.GetAsync(testGuid);
            var result = okObjectResult as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(result);
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingGuidPassed_ReturnRightItem()
        {
            // Arrange
            var testGuid = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200");
            var singleItem = ShoppingCartMockData.SingleCartItem();
            _mockService.Setup(service => service.GetByIdAsync(testGuid)).ReturnsAsync(singleItem);

            // Act
            var okResult = await _controller.GetAsync(testGuid) as OkObjectResult;
            var result = okResult.Value as ShoppingItem;

            // Assert
            Assert.Equal(testGuid, result.Id);
            Assert.Equal(singleItem, result);
            result.Should().BeEquivalentTo(singleItem);
        }

        #endregion

        #region Add Item

        [Fact]
        public async Task PostAsync_InvalidObjectPassed_Return400BadRequestStatus()
        {
            // Arrange
            var nameMissingItem = ShoppingCartMockData.MissingNameItem();
            _mockService.Setup(service => service.AddAsync(nameMissingItem));
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
            _mockService.Setup(service => service.AddAsync(It.IsAny<ShoppingItem>())).ReturnsAsync(newItem);

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
            _mockService.Setup(service => service.AddAsync(It.IsAny<ShoppingItem>())).ReturnsAsync(newItem);

            // Act
            var result = await _controller.PostAsync(newItem) as OkObjectResult;

            // Assert
            Assert.Equal(newItem, result.Value);
            result.Value.Should().BeEquivalentTo(newItem);

            _mockService.Verify(_ => _.AddAsync(newItem), Times.Exactly(1));
        }

        #endregion

        #region Update Item

        [Fact]
        public async Task Update_ValidObjectPassed_Return200OkStatus()
        {
            // Arrange
            var testItem = ShoppingCartMockData.SingleCartItem();
            _mockService.Setup(service => service.GetByIdAsync(testItem.Id)).ReturnsAsync(testItem).Verifiable();
            _mockService.Setup(service => service.UpdateAsync(testItem)).ReturnsAsync(testItem).Verifiable();

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
            _mockService.Setup(service => service.GetByIdAsync(testItem.Id)).ReturnsAsync(testItem);
            _mockService.Setup(service => service.UpdateAsync(testItem)).ReturnsAsync(testItem);

            // Act
            var result = await _controller.PutAsync(testItem) as OkObjectResult;
            var item = result.Value as ShoppingItem;

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
            var existingGuid = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200");
            var existingItem = ShoppingCartMockData.SingleCartItem();
            _mockService.Setup(service => service.GetByIdAsync(existingGuid)).ReturnsAsync(existingItem).Verifiable();
            _mockService.Setup(service => service.RemoveAsync(existingItem)).Verifiable();

            // Act
            var noContentResponse = await _controller.RemoveAsync(existingGuid);
            var result = noContentResponse as NoContentResult;

            // Assert
            Assert.IsType<NoContentResult>(noContentResponse);
            result.StatusCode.Should().Be(204);
            _mockService.Verify();
        }

        #endregion
    }
}
