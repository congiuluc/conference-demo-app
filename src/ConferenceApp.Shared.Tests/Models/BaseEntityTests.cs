using ConferenceApp.Shared.Models;
using FluentAssertions;
using Xunit;

namespace ConferenceApp.Shared.Tests.Models;

public class TestEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}

public class BaseEntityTests
{
    [Fact]
    public void Constructor_ShouldInitializeIdWithGuid()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.Id.Should().NotBeNullOrEmpty();
        Guid.TryParse(entity.Id, out _).Should().BeTrue();
    }

    [Fact]
    public void Constructor_ShouldInitializeCreatedAtWithCurrentTime()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var entity = new TestEntity();

        // Assert
        var afterCreation = DateTime.UtcNow.AddSeconds(1);
        entity.CreatedAt.Should().BeAfter(beforeCreation);
        entity.CreatedAt.Should().BeBefore(afterCreation);
    }

    [Fact]
    public void Constructor_ShouldLeaveUpdatedAtAsNull()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void CreatedDate_ShouldReturnCreatedAt()
    {
        // Arrange
        var entity = new TestEntity();
        var expectedDate = DateTime.UtcNow;
        entity.CreatedAt = expectedDate;

        // Act & Assert
        entity.CreatedDate.Should().Be(expectedDate);
    }

    [Fact]
    public void CreatedDate_WhenSet_ShouldUpdateCreatedAt()
    {
        // Arrange
        var entity = new TestEntity();
        var newDate = DateTime.UtcNow.AddDays(-1);

        // Act
        entity.CreatedDate = newDate;

        // Assert
        entity.CreatedAt.Should().Be(newDate);
    }

    [Fact]
    public void ModifiedDate_ShouldReturnUpdatedAt()
    {
        // Arrange
        var entity = new TestEntity();
        var expectedDate = DateTime.UtcNow;
        entity.UpdatedAt = expectedDate;

        // Act & Assert
        entity.ModifiedDate.Should().Be(expectedDate);
    }

    [Fact]
    public void ModifiedDate_WhenSet_ShouldUpdateUpdatedAt()
    {
        // Arrange
        var entity = new TestEntity();
        var newDate = DateTime.UtcNow.AddDays(-1);

        // Act
        entity.ModifiedDate = newDate;

        // Assert
        entity.UpdatedAt.Should().Be(newDate);
    }

    [Fact]
    public void ModifiedDate_WhenSetToNull_ShouldUpdateUpdatedAtToNull()
    {
        // Arrange
        var entity = new TestEntity();
        entity.UpdatedAt = DateTime.UtcNow;

        // Act
        entity.ModifiedDate = null;

        // Assert
        entity.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void PartitionKey_ShouldBeSettable()
    {
        // Arrange
        var entity = new TestEntity();
        var partitionKey = "TestPartition";

        // Act
        entity.PartitionKey = partitionKey;

        // Assert
        entity.PartitionKey.Should().Be(partitionKey);
    }
}