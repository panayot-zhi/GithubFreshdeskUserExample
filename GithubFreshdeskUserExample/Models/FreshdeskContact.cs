using Newtonsoft.Json;

namespace GithubFreshdeskUserExample.Models
{
    public class FreshdeskContact
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("unique_external_id")]
        public string UniqueExternalId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; } 
        
        [JsonProperty("twitter_id")]
        public string TwitterId { get; set; }


        [JsonProperty("custom_fields")]
        public Dictionary<string, string> CustomFields { get; set; } = new();
    }
}
