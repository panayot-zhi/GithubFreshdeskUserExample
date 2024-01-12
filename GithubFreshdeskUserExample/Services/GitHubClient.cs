using System.Net.Http.Headers;
using GithubFreshdeskUserExample.Contracts;
using GithubFreshdeskUserExample.Models;
using Newtonsoft.Json;


namespace GithubFreshdeskUserExample.Services
{
    public class GitHubClient : IGitHubClient
    {
        private readonly ILogger<GitHubClient> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public GitHubClient(IHttpClientFactory httpClientFactory, ILogger<GitHubClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<GitHubUser> GetGitHubUser(string username)
        {
            var client = _httpClientFactory.CreateClient(nameof(GetGitHubUser));

            var requestUri = $"https://api.github.com/users/{username}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
            request.Headers.Add("User-Agent", nameof(GithubFreshdeskUserExample));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            var githubToken = Environment.GetEnvironmentVariable("GITHUB__TOKEN");
            if (!string.IsNullOrEmpty(githubToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Token", githubToken);
            }

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                _logger.LogError("GitHub user retrieval failed: {ResponseStatus}, {Details}", 
                    $"{response.StatusCode} {response.ReasonPhrase}",
                    message);

                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GitHubUser>(content);
        }
    }


}
