namespace GithubFreshdeskUserExample.Contracts
{
    public interface IIntegrationService
    {
        Task IntegrateUser(string githubUsername, string freshdeskDomain);
    }
}
