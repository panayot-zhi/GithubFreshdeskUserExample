using System.Net;
using GithubFreshdeskUserExample.Contracts;
using GithubFreshdeskUserExample.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;

namespace GithubFreshdeskUserExample.Services
{
    public class FreshdeskClient : IFreshdeskClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<FreshdeskClient> _logger;

        public FreshdeskClient(IHttpClientFactory httpClientFactory, ILogger<FreshdeskClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public Task CreateOrUpdateContact(FreshdeskContact contact, string freshdeskDomain)
        {
            return CreateOrUpdateContactInternal(contact, freshdeskDomain, null);
        }

        private async Task CreateOrUpdateContactInternal(FreshdeskContact contact, string freshdeskDomain, long? freshdeskContactId)
        {
            var client = _httpClientFactory.CreateClient(nameof(CreateOrUpdateContactInternal));

            var requestUri = $"https://{freshdeskDomain}.freshdesk.com/api/v2/contacts";
            if (freshdeskContactId.HasValue)
            {
                requestUri += $"/{freshdeskContactId}";
            }

            var freshdeskApiKey = Environment.GetEnvironmentVariable("FRESHDESK__TOKEN");
            var authentication = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{freshdeskApiKey}:X"));

            // Prepare default request headers
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authentication);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Serialize contact object to JSON
            var jsonContent = JsonConvert.SerializeObject(contact);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Make a POST or PUT request
            var response = freshdeskContactId.HasValue ?
                await client.PutAsync(requestUri, httpContent) :
                await client.PostAsync(requestUri, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode is HttpStatusCode.Conflict)
                {
                    var errorResponse = JsonConvert.DeserializeObject<FreshdeskErrorResponse>(responseContent);
                    var existingRecordId = errorResponse?.Errors
                        .SingleOrDefault(x => "duplicate_value".Equals(x.Code))?
                        .AdditionalInfo?.UserId;

                    if (existingRecordId.HasValue)
                    {
                        await CreateOrUpdateContactInternal(contact, freshdeskDomain, existingRecordId);
                        return;
                    }
                }

                _logger.LogError("Freshdesk contact creation failed: {ResponseStatus}, {Details}",
                    $"{response.StatusCode} {response.ReasonPhrase}",
                    responseContent);
            }
        }
    }
}
