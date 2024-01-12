namespace GithubFreshdeskUserExample.Models
{
    public class TokenConfig
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string Secret { get; set; }

        public int ExpiryDays { get; set; } = 1;
    }
}
