using ConferenceApp.API.Services;
using ConferenceApp.Shared.Models;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Moq;
using System.Net;
using Xunit;

namespace ConferenceApp.API.Tests.Services;

public class TestEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}

public class CosmosDbServiceTests
{
    private readonly Mock<Container> _mockContainer;
    private readonly CosmosDbService<TestEntity> _service;

    public CosmosDbServiceTests()
    {
        var mockCosmosClient = new Mock<CosmosClient>();
        _mockContainer = new Mock<Container>();
        
        mockCosmosClient
            .Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(_mockContainer.Object);

        _service = new CosmosDbService<TestEntity>(
            mockCosmosClient.Object, 
            "testDb", 
            "testContainer");
    }

    [Fact]
    public async Task GetItemAsync_WhenItemExists_ShouldReturnItem()
    {
        // Arrange
        var entity = new TestEntity { Id = "test-id", Name = "Test Entity" };
        var mockResponse = new Mock<ItemResponse<TestEntity>>();
        mockResponse.Setup(r => r.Resource).Returns(entity);

        _mockContainer
            .Setup(c => c.ReadItemAsync<TestEntity>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        // Act
        var result = await _service.GetItemAsync("test-id", "Test");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("test-id");
        result.Name.Should().Be("Test Entity");
    }

    [Fact]
    public async Task GetItemAsync_WhenItemNotFound_ShouldReturnNull()
    {
        // Arrange
        _mockContainer
            .Setup(c => c.ReadItemAsync<TestEntity>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CosmosException("Not found", HttpStatusCode.NotFound, 0, "", 0));

        // Act
        var result = await _service.GetItemAsync("nonexistent-id", "Test");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddItemAsync_ShouldCreateItem()
    {
        // Arrange
        var entity = new TestEntity { Name = "New Entity" };
        var mockResponse = new Mock<ItemResponse<TestEntity>>();
        mockResponse.Setup(r => r.Resource).Returns(entity);

        _mockContainer
            .Setup(c => c.CreateItemAsync(
                It.IsAny<TestEntity>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        // Act
        var result = await _service.AddItemAsync(entity);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Entity");
        
        _mockContainer.Verify(c => c.CreateItemAsync(
            entity,
            It.IsAny<PartitionKey>(),
            It.IsAny<ItemRequestOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateItemAsync_ShouldUpdateItem()
    {
        // Arrange
        var entity = new TestEntity { Id = "test-id", Name = "Updated Entity" };
        var mockResponse = new Mock<ItemResponse<TestEntity>>();
        mockResponse.Setup(r => r.Resource).Returns(entity);

        _mockContainer
            .Setup(c => c.ReplaceItemAsync(
                It.IsAny<TestEntity>(),
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        // Act
        var result = await _service.UpdateItemAsync("test-id", entity);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Entity");
        entity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        
        _mockContainer.Verify(c => c.ReplaceItemAsync(
            entity,
            "test-id",
            It.IsAny<PartitionKey>(),
            It.IsAny<ItemRequestOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteItemAsync_ShouldDeleteItem()
    {
        // Arrange
        var mockResponse = new Mock<ItemResponse<TestEntity>>();
        
        _mockContainer
            .Setup(c => c.DeleteItemAsync<TestEntity>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        // Act & Assert
        await _service.DeleteItemAsync("test-id", "Test");

        _mockContainer.Verify(c => c.DeleteItemAsync<TestEntity>(
            "test-id",
            It.IsAny<PartitionKey>(),
            It.IsAny<ItemRequestOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}