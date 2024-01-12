using Newtonsoft.Json;

namespace GithubFreshdeskUserExample.Models
{
    public record GitHubUser
    {
        [JsonProperty("id")]
        public int Id { get; init; }

        [JsonProperty("login")]
        public string Login { get; init; }

        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("email")]
        public string Email { get; init; }

        [JsonProperty("twitter_username")]
        public string TwitterUsername { get; init; }

        [JsonProperty("bio")]
        public string Bio { get; init; }

        [JsonProperty("url")]
        public string Url { get; init; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; init; }

        [JsonProperty("repos_url")]
        public string ReposUrl { get; init; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; init; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; init; }
    }
}
