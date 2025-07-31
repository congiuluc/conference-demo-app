using AngleSharp;
using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace ConferenceApp.UI.Tests;

public class NavigationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public NavigationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task HomePage_ShouldContainDiscordLink()
    {
        // Act
        var response = await _client.GetAsync("/");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Parse HTML using AngleSharp
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(responseString));
        
        // Find the Discord navigation link
        var discordLink = document.QuerySelector("a[href='https://discord.gg/conference']");
        
        Assert.NotNull(discordLink);
    }

    [Fact]
    public async Task DiscordLink_ShouldHaveCorrectAttributes()
    {
        // Act
        var response = await _client.GetAsync("/");
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Parse HTML
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(responseString));
        
        // Find the Discord navigation link
        var discordLink = document.QuerySelector("a[href='https://discord.gg/conference']") as IHtmlAnchorElement;
        
        // Assert
        Assert.NotNull(discordLink);
        Assert.Equal("https://discord.gg/conference", discordLink.Href);
        Assert.Equal("_blank", discordLink.Target);
        Assert.Equal("Join our Discord Community", discordLink.Title);
    }

    [Fact]
    public async Task DiscordLink_ShouldHaveDiscordIcon()
    {
        // Act
        var response = await _client.GetAsync("/");
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Parse HTML
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(responseString));
        
        // Find the Discord navigation link
        var discordLink = document.QuerySelector("a[href='https://discord.gg/conference']");
        
        // Find the icon inside the link
        var discordIcon = discordLink?.QuerySelector("i.fab.fa-discord");
        
        // Assert
        Assert.NotNull(discordIcon);
        Assert.True(discordIcon.ClassList.Contains("fab"));
        Assert.True(discordIcon.ClassList.Contains("fa-discord"));
    }

    [Fact]
    public async Task DiscordLink_ShouldHaveCorrectText()
    {
        // Act
        var response = await _client.GetAsync("/");
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Parse HTML
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(responseString));
        
        // Find the Discord navigation link
        var discordLink = document.QuerySelector("a[href='https://discord.gg/conference']");
        
        // Assert
        Assert.NotNull(discordLink);
        Assert.Contains("Discord", discordLink.TextContent);
    }

    [Fact]
    public async Task Navigation_ShouldContainAllExpectedLinks()
    {
        // Act
        var response = await _client.GetAsync("/");
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Parse HTML
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(responseString));
        
        // Find all navigation links
        var navLinks = document.QuerySelectorAll("ul.navbar-nav .nav-link");
        
        // Assert expected navigation items are present
        var linkTexts = navLinks.Select(link => link.TextContent.Trim()).ToList();
        
        Assert.Contains("Dashboard", linkTexts);
        Assert.Contains("Conferences", linkTexts);
        Assert.Contains("Speakers", linkTexts);
        Assert.Contains("Sessions", linkTexts);
        Assert.Contains("Attendees", linkTexts);
        Assert.Contains("Discord", linkTexts);
    }

    [Fact]
    public async Task DiscordLink_ShouldBeInNavigationBar()
    {
        // Act
        var response = await _client.GetAsync("/");
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Parse HTML
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(responseString));
        
        // Find the Discord link within the navigation structure
        var navbarNav = document.QuerySelector("ul.navbar-nav");
        var discordNavItem = navbarNav?.QuerySelector("li.nav-item a[href='https://discord.gg/conference']");
        
        // Assert
        Assert.NotNull(discordNavItem);
        Assert.True(discordNavItem.ClassList.Contains("nav-link"));
    }
}