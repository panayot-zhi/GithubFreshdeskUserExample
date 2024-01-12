using Newtonsoft.Json;

namespace GithubFreshdeskUserExample.Models
{
    public class FreshdeskErrorResponse
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("errors")]
        public List<FreshdeskError> Errors { get; set; }
    }

    public class FreshdeskErrorAdditionalInfo
    {
        [JsonProperty("user_id")]
        public long UserId { get; set; }
    }

    public class FreshdeskError
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("additional_info")]
        public FreshdeskErrorAdditionalInfo AdditionalInfo { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
