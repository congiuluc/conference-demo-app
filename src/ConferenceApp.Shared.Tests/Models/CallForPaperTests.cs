using ConferenceApp.Shared.Models;
using FluentAssertions;
using Xunit;

namespace ConferenceApp.Shared.Tests.Models;

public class CallForPaperTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var callForPaper = new CallForPaper();

        // Assert
        callForPaper.PartitionKey.Should().Be("CallForPaper");
        callForPaper.Topics.Should().NotBeNull().And.BeEmpty();
        callForPaper.SessionTypes.Should().NotBeNull().And.BeEmpty();
        callForPaper.IsOpen.Should().BeTrue();
    }

    [Fact]
    public void AllProperties_ShouldBeSettable()
    {
        // Arrange
        var callForPaper = new CallForPaper();
        var conferenceId = "conf123";
        var title = "Call for Papers - Tech Conference 2024";
        var description = "We are looking for innovative speakers";
        var startDate = new DateTime(2024, 1, 1);
        var deadline = new DateTime(2024, 3, 31);
        var evaluationCriteria = "Quality, Relevance, Innovation";
        var contactEmail = "cfp@techconf2024.com";
        var infoUrl = "https://techconf2024.com/cfp";

        // Act
        callForPaper.ConferenceId = conferenceId;
        callForPaper.Title = title;
        callForPaper.Description = description;
        callForPaper.StartDate = startDate;
        callForPaper.Deadline = deadline;
        callForPaper.EvaluationCriteria = evaluationCriteria;
        callForPaper.ContactEmail = contactEmail;
        callForPaper.InfoUrl = infoUrl;
        callForPaper.IsOpen = false;
        
        // Add topics and session types
        callForPaper.Topics.Add("Artificial Intelligence");
        callForPaper.Topics.Add("Cloud Computing");
        callForPaper.Topics.Add("DevOps");
        
        callForPaper.SessionTypes.Add("Talk");
        callForPaper.SessionTypes.Add("Workshop");
        callForPaper.SessionTypes.Add("Panel Discussion");

        // Assert
        callForPaper.ConferenceId.Should().Be(conferenceId);
        callForPaper.Title.Should().Be(title);
        callForPaper.Description.Should().Be(description);
        callForPaper.StartDate.Should().Be(startDate);
        callForPaper.Deadline.Should().Be(deadline);
        callForPaper.EvaluationCriteria.Should().Be(evaluationCriteria);
        callForPaper.ContactEmail.Should().Be(contactEmail);
        callForPaper.InfoUrl.Should().Be(infoUrl);
        callForPaper.IsOpen.Should().BeFalse();
        
        callForPaper.Topics.Should().Contain("Artificial Intelligence");
        callForPaper.Topics.Should().Contain("Cloud Computing");
        callForPaper.Topics.Should().Contain("DevOps");
        
        callForPaper.SessionTypes.Should().Contain("Talk");
        callForPaper.SessionTypes.Should().Contain("Workshop");
        callForPaper.SessionTypes.Should().Contain("Panel Discussion");
    }

    [Fact]
    public void Topics_ShouldSupportMultipleValues()
    {
        // Arrange
        var callForPaper = new CallForPaper();

        // Act
        callForPaper.Topics.Add("Machine Learning");
        callForPaper.Topics.Add("Blockchain");
        callForPaper.Topics.Add("Cybersecurity");
        callForPaper.Topics.Add("Mobile Development");

        // Assert
        callForPaper.Topics.Should().HaveCount(4);
        callForPaper.Topics.Should().Contain(new[] 
        { 
            "Machine Learning", 
            "Blockchain", 
            "Cybersecurity", 
            "Mobile Development" 
        });
    }

    [Fact]
    public void SessionTypes_ShouldSupportMultipleValues()
    {
        // Arrange
        var callForPaper = new CallForPaper();

        // Act
        callForPaper.SessionTypes.Add("Keynote");
        callForPaper.SessionTypes.Add("Lightning Talk");
        callForPaper.SessionTypes.Add("Tutorial");

        // Assert
        callForPaper.SessionTypes.Should().HaveCount(3);
        callForPaper.SessionTypes.Should().Contain(new[] 
        { 
            "Keynote", 
            "Lightning Talk", 
            "Tutorial" 
        });
    }

    [Fact]
    public void IsOpen_ShouldBeToggleable()
    {
        // Arrange
        var callForPaper = new CallForPaper();

        // Act & Assert (initial state)
        callForPaper.IsOpen.Should().BeTrue();

        // Act
        callForPaper.IsOpen = false;

        // Assert
        callForPaper.IsOpen.Should().BeFalse();

        // Act
        callForPaper.IsOpen = true;

        // Assert
        callForPaper.IsOpen.Should().BeTrue();
    }
}