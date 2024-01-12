using System.Net;
using System.Net.Http;
using GithubFreshdeskUserExample.Contracts;
using GithubFreshdeskUserExample.Models;
using GithubFreshdeskUserExample.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GithubFreshdeskUserExampleUnitTests
{
    public class IntegrationServiceTests
    {
        private Mock<IGitHubClient> _mockGithubClient;
        private Mock<IFreshdeskClient> _mockFreshdeskClient;
        private IntegrationService _integrationService;

        [SetUp]
        public void Setup()
        {
            _mockGithubClient = new Mock<IGitHubClient>();
            _mockFreshdeskClient = new Mock<IFreshdeskClient>();
            _integrationService = new IntegrationService(_mockGithubClient.Object, _mockFreshdeskClient.Object);
        }

        [Test]
        public async Task IntegrateUser_ShouldCallFreshdesk_WhenGitHubUserExists()
        {
            // Arrange
            var githubUser = new GitHubUser
            {
                Name = "test",
                Email = "test@example.com"
            };

            _mockGithubClient.Setup(x => x.GetGitHubUser(It.IsAny<string>())).ReturnsAsync(githubUser);

            // Act
            await _integrationService.IntegrateUser(GitHubClientTests.GitHubUser, FreshdeskClientTests.FreshdeskSubdomain);

            // Assert
            _mockFreshdeskClient.Verify(x => x.CreateOrUpdateContact(It.IsAny<FreshdeskContact>(), FreshdeskClientTests.FreshdeskSubdomain), Times.Once);
            _mockFreshdeskClient.Verify(x => x.CreateOrUpdateContact(It.Is<FreshdeskContact>(c =>
                        c.Email == githubUser.Email &&
                        c.Name == githubUser.Name),
                    FreshdeskClientTests.FreshdeskSubdomain),
                Times.Once);
        }

        [Test]
        public async Task IntegrateUser_ShouldNotCallFreshdesk_WhenGitHubUserNotFound()
        {
            // Arrange
            _mockGithubClient.Setup(x => x.GetGitHubUser(It.IsAny<string>())).ReturnsAsync((GitHubUser) null);

            // Act
            await _integrationService.IntegrateUser("nonexistentuser", FreshdeskClientTests.FreshdeskSubdomain);

            // Assert
            _mockFreshdeskClient.Verify(x => x.CreateOrUpdateContact(It.IsAny<FreshdeskContact>(), FreshdeskClientTests.FreshdeskSubdomain), Times.Never);
        }

        [Test]
        public async Task IntegrateUser_ShouldCreateCorrectFreshdeskContact_WhenGitHubUserExists()
        {
            // Arrange
            var githubUser = new GitHubUser
            {
                Login = "test", 
                Email = "test@example.com"
            };

            var freshdeskDomain = FreshdeskClientTests.FreshdeskSubdomain;

            _mockGithubClient.Setup(x => x.GetGitHubUser(It.IsAny<string>())).ReturnsAsync(githubUser);

            // Act
            await _integrationService.IntegrateUser(GitHubClientTests.GitHubUser, freshdeskDomain);

            // Assert
            _mockFreshdeskClient.Verify(x => x.CreateOrUpdateContact(It.Is<FreshdeskContact>(c => 
                c.Email == githubUser.Email && 
                c.Name == githubUser.Login),
                FreshdeskClientTests.FreshdeskSubdomain), 
            Times.Once);
        }


    }
}