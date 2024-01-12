using GithubFreshdeskUserExample.Models;

namespace GithubFreshdeskUserExample.Contracts
{
    public interface IGitHubClient
    {
        Task<GitHubUser> GetGitHubUser(string username);
    }
}
