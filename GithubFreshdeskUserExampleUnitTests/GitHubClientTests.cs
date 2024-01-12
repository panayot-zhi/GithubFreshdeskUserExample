using System.Net;
using System.Net.Http;
using GithubFreshdeskUserExample.Models;
using GithubFreshdeskUserExample.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GithubFreshdeskUserExampleUnitTests
{
    public class GitHubClientTests
    {
        public const string GitHubUser = "testuser";

        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private MockHttpMessageHandler _mockHttpMessageHandler;
        private Mock<ILogger<GitHubClient>> _mockLogger;
        private GitHubClient _gitHubClient;

        [SetUp]
        public void Setup()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<GitHubClient>>();

            _mockHttpMessageHandler = new MockHttpMessageHandler();
            var httpClient = new HttpClient(_mockHttpMessageHandler);

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            _gitHubClient = new GitHubClient(_mockHttpClientFactory.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetGitHubUser_ReturnsUser_WhenSuccessful()
        {
            // Arrange
            Environment.SetEnvironmentVariable("GITHUB__TOKEN", "SUPER_SECRET");

            var gitHubUser = new GitHubUser
            {
                Login = GitHubUser
            };

            var mockResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(gitHubUser))
            };

            _mockHttpMessageHandler.ResponsesQueue.Enqueue(mockResponse);

            // Act
            var result = await _gitHubClient.GetGitHubUser(GitHubUser);

            // Assert
            var capturedRequest = _mockHttpMessageHandler.RequestsQueue.Pop();

            Assert.IsNotNull(result);
            Assert.That(gitHubUser.Login, Is.EqualTo(result.Login)); ;
            Assert.That(capturedRequest.Headers.Contains("X-GitHub-Api-Version"));
            Assert.That(capturedRequest.Headers.Contains("User-Agent"));
            Assert.That(capturedRequest.Headers.Contains("Authorization"));
        }

        [Test]
        public async Task GetGitHubUser_LogError_WhenUnsuccessful()
        {
            // Arrange
            var mockResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("log_me")
            };

            _mockHttpMessageHandler.ResponsesQueue.Enqueue(mockResponse);

            // Act
            var result = await _gitHubClient.GetGitHubUser(GitHubUser);

            // Assert
            Assert.IsNull(result);

            _mockLogger.VerifyLog(logger => logger.LogError(It.Is<string>(
                message => message.Contains("log_me"))));
        }
    }
}