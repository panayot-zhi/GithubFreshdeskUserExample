using System.ComponentModel.DataAnnotations;
using GithubFreshdeskUserExample.Contracts;
using GithubFreshdeskUserExample.Models;
using GithubFreshdeskUserExample.Utility;

namespace GithubFreshdeskUserExample.Services
{
    public class IntegrationService : IIntegrationService
    {
        private readonly IGitHubClient _githubClient;
        private readonly IFreshdeskClient _freshdeskClient;

        public IntegrationService(IGitHubClient githubClient, IFreshdeskClient freshdeskClient)
        {
            _githubClient = githubClient;
            _freshdeskClient = freshdeskClient;
        }

        public async Task IntegrateUser(string githubUsername, string freshdeskDomain)
        {
            var githubUser = await _githubClient.GetGitHubUser(githubUsername);
            if (githubUser is null)
            {
                return;
            }

            var freshdeskContact = CreateFreshdeskContact(githubUser);
            await _freshdeskClient.CreateOrUpdateContact(freshdeskContact, freshdeskDomain);
        }

        private static FreshdeskContact CreateFreshdeskContact(GitHubUser user)
        {
            var contact = new FreshdeskContact()
            {
                Email = user.Email,
                Name = user.Name ?? user.Login,
                UniqueExternalId = user.Id.ToString(),
                TwitterId = user.TwitterUsername,
                Description = user.Bio,

                // NOTE: These need to be created as custom fields in freshdesk
                // Check this: https://support.freshdesk.com/en/support/solutions/articles/216553
                // CustomFields = new Dictionary<string, string>()
                // {
                //     {"github_" + nameof(user.Url).ToSnakeCaseString(), user.Url},
                //     {"github_" + nameof(user.AvatarUrl).ToSnakeCaseString(), user.AvatarUrl},
                //     {"github_" + nameof(user.ReposUrl).ToSnakeCaseString(), user.ReposUrl},
                //     {"github_" + nameof(user.CreatedAt).ToSnakeCaseString(), user.CreatedAt?.ToString(Extensions.DateTimeISOStringFormat)},
                //     {"github_" + nameof(user.UpdatedAt).ToSnakeCaseString(), user.UpdatedAt?.ToString(Extensions.DateTimeISOStringFormat)}
                // }
            };

            return contact;

        }
    }
}
