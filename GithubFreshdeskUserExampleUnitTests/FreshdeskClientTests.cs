using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using GithubFreshdeskUserExample.Contracts;
using GithubFreshdeskUserExample.Models;
using GithubFreshdeskUserExample.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GithubFreshdeskUserExampleUnitTests
{
    public class FreshdeskClientTests
    {
        public const string FreshdeskSubdomain = "testdomain";

        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private MockHttpMessageHandler _mockHttpMessageHandler;
        private Mock<ILogger<FreshdeskClient>> _mockLogger;
        private FreshdeskClient _freshdeskClient;

        [SetUp]
        public void Setup()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<FreshdeskClient>>();
            _mockHttpMessageHandler = new MockHttpMessageHandler();
            var httpClient = new HttpClient(_mockHttpMessageHandler);

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            _freshdeskClient = new FreshdeskClient(_mockHttpClientFactory.Object, _mockLogger.Object);
        }

        [Test]
        public async Task CreateOrUpdateContact_ShouldCreateContact_WhenContactDoesNotExist()
        {
            // Arrange
            var freshdeskContact = new FreshdeskContact { Email = "test@example.com" };

            // Act
            await _freshdeskClient.CreateOrUpdateContact(freshdeskContact, FreshdeskSubdomain);

            // Assert
            var capturedRequest = _mockHttpMessageHandler.RequestsQueue.Pop();
            Assert.That(1, Is.EqualTo(_mockHttpMessageHandler.NumberOfCalls));
            Assert.That(capturedRequest.Headers.Contains("Authorization"));
        }

        [Test]
        public async Task CreateOrUpdateContact_ShouldUpdateContact_WhenContactExists()
        {
            // Arrange
            var veryUniqueUserId = RandomNumberGenerator.GetInt32(1, 1000);
            var freshdeskContact = new FreshdeskContact { Email = "test@example.com" };
            var freshdeskErrorResponse = new FreshdeskErrorResponse()
            {
                Errors = new List<FreshdeskError>()
                {
                    new()
                    {
                        Code = "duplicate_value",
                        AdditionalInfo = new FreshdeskErrorAdditionalInfo()
                        {
                            UserId = veryUniqueUserId
                        }
                    }
                }
            };

            var successResponse = new HttpResponseMessage(HttpStatusCode.OK);
            var conflictResponse = new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent(JsonConvert.SerializeObject(freshdeskErrorResponse))
            };

            _mockHttpMessageHandler.ResponsesQueue.Enqueue(conflictResponse);
            _mockHttpMessageHandler.ResponsesQueue.Enqueue(successResponse);

            // Act
            await _freshdeskClient.CreateOrUpdateContact(freshdeskContact, FreshdeskSubdomain);

            // Assert
            var lastRequest = _mockHttpMessageHandler.RequestsQueue.Pop();
            Assert.That(2, Is.EqualTo(_mockHttpMessageHandler.NumberOfCalls));
            Assert.That(lastRequest.RequestUri.ToString().EndsWith(veryUniqueUserId.ToString()));
            Assert.That(lastRequest.Headers.Contains("Authorization"));
        }

        [Test]
        public async Task CreateOrUpdateContact_LogError_WhenUnsuccessful()
        {
            // Arrange
            var freshdeskContact = new FreshdeskContact { Email = "test@example.com" };
            var mockResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("log_me")
            };

            _mockHttpMessageHandler.ResponsesQueue.Enqueue(mockResponse);

            // Act
            await _freshdeskClient.CreateOrUpdateContact(freshdeskContact, FreshdeskSubdomain);

            // Assert
            _mockLogger.VerifyLog(logger => logger.LogError(It.Is<string>(
                message => message.Contains("log_me"))));
        }
    }
}