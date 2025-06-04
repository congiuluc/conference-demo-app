using ConferenceApp.Shared.Models;
using FluentAssertions;
using Xunit;

namespace ConferenceApp.Shared.Tests.Models;

public class SpeakerTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var speaker = new Speaker();

        // Assert
        speaker.PartitionKey.Should().Be("Speaker");
        speaker.ConferenceIds.Should().NotBeNull().And.BeEmpty();
        speaker.SocialMedia.Should().BeNull();
    }

    [Fact]
    public void Title_ShouldGetAndSetJobTitle()
    {
        // Arrange
        var speaker = new Speaker();
        var title = "Senior Software Engineer";

        // Act
        speaker.Title = title;

        // Assert
        speaker.JobTitle.Should().Be(title);
        speaker.Title.Should().Be(title);
    }

    [Fact]
    public void TwitterHandle_WhenSet_ShouldCreateSocialMediaDictionary()
    {
        // Arrange
        var speaker = new Speaker();
        var twitterHandle = "@johndoe";

        // Act
        speaker.TwitterHandle = twitterHandle;

        // Assert
        speaker.SocialMedia.Should().NotBeNull();
        speaker.SocialMedia.Should().ContainKey("Twitter");
        speaker.SocialMedia["Twitter"].Should().Be(twitterHandle);
        speaker.TwitterHandle.Should().Be(twitterHandle);
    }

    [Fact]
    public void TwitterHandle_WhenSetToNull_ShouldRemoveFromSocialMedia()
    {
        // Arrange
        var speaker = new Speaker();
        speaker.TwitterHandle = "@johndoe";

        // Act
        speaker.TwitterHandle = null;

        // Assert
        speaker.SocialMedia.Should().NotContainKey("Twitter");
        speaker.TwitterHandle.Should().BeNull();
    }

    [Fact]
    public void TwitterHandle_WhenSetToEmpty_ShouldRemoveFromSocialMedia()
    {
        // Arrange
        var speaker = new Speaker();
        speaker.TwitterHandle = "@johndoe";

        // Act
        speaker.TwitterHandle = "";

        // Assert
        speaker.SocialMedia.Should().NotContainKey("Twitter");
        speaker.TwitterHandle.Should().BeNull();
    }

    [Fact]
    public void LinkedInProfile_WhenSet_ShouldCreateSocialMediaDictionary()
    {
        // Arrange
        var speaker = new Speaker();
        var linkedInProfile = "https://linkedin.com/in/johndoe";

        // Act
        speaker.LinkedInProfile = linkedInProfile;

        // Assert
        speaker.SocialMedia.Should().NotBeNull();
        speaker.SocialMedia.Should().ContainKey("LinkedIn");
        speaker.SocialMedia["LinkedIn"].Should().Be(linkedInProfile);
        speaker.LinkedInProfile.Should().Be(linkedInProfile);
    }

    [Fact]
    public void LinkedInProfile_WhenSetToNull_ShouldRemoveFromSocialMedia()
    {
        // Arrange
        var speaker = new Speaker();
        speaker.LinkedInProfile = "https://linkedin.com/in/johndoe";

        // Act
        speaker.LinkedInProfile = null;

        // Assert
        speaker.SocialMedia.Should().NotContainKey("LinkedIn");
        speaker.LinkedInProfile.Should().BeNull();
    }

    [Fact]
    public void LinkedInProfile_WhenSetToEmpty_ShouldRemoveFromSocialMedia()
    {
        // Arrange
        var speaker = new Speaker();
        speaker.LinkedInProfile = "https://linkedin.com/in/johndoe";

        // Act
        speaker.LinkedInProfile = "";

        // Assert
        speaker.SocialMedia.Should().NotContainKey("LinkedIn");
        speaker.LinkedInProfile.Should().BeNull();
    }

    [Fact]
    public void SocialMedia_CanStoreMultiplePlatforms()
    {
        // Arrange
        var speaker = new Speaker();

        // Act
        speaker.TwitterHandle = "@johndoe";
        speaker.LinkedInProfile = "https://linkedin.com/in/johndoe";

        // Assert
        speaker.SocialMedia.Should().HaveCount(2);
        speaker.SocialMedia.Should().ContainKeys("Twitter", "LinkedIn");
    }

    [Fact]
    public void AllProperties_ShouldBeSettable()
    {
        // Arrange
        var speaker = new Speaker();
        var name = "John Doe";
        var bio = "Experienced software engineer";
        var company = "Tech Corp";
        var jobTitle = "Senior Developer";
        var photoUrl = "https://example.com/photo.jpg";
        var email = "john.doe@example.com";
        var website = "https://johndoe.dev";
        var phone = "+1-555-0123";

        // Act
        speaker.Name = name;
        speaker.Bio = bio;
        speaker.Company = company;
        speaker.JobTitle = jobTitle;
        speaker.PhotoUrl = photoUrl;
        speaker.Email = email;
        speaker.Website = website;
        speaker.Phone = phone;
        speaker.ConferenceIds.Add("conf1");
        speaker.ConferenceIds.Add("conf2");

        // Assert
        speaker.Name.Should().Be(name);
        speaker.Bio.Should().Be(bio);
        speaker.Company.Should().Be(company);
        speaker.JobTitle.Should().Be(jobTitle);
        speaker.PhotoUrl.Should().Be(photoUrl);
        speaker.Email.Should().Be(email);
        speaker.Website.Should().Be(website);
        speaker.Phone.Should().Be(phone);
        speaker.ConferenceIds.Should().Contain("conf1");
        speaker.ConferenceIds.Should().Contain("conf2");
    }

    [Fact]
    public void TwitterHandle_WhenSocialMediaIsNull_ShouldReturnNull()
    {
        // Arrange
        var speaker = new Speaker();

        // Act & Assert
        speaker.TwitterHandle.Should().BeNull();
    }

    [Fact]
    public void LinkedInProfile_WhenSocialMediaIsNull_ShouldReturnNull()
    {
        // Arrange
        var speaker = new Speaker();

        // Act & Assert
        speaker.LinkedInProfile.Should().BeNull();
    }
}