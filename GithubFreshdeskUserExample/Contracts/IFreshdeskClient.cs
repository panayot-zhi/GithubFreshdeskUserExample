using GithubFreshdeskUserExample.Models;

namespace GithubFreshdeskUserExample.Contracts
{
    public interface IFreshdeskClient
    {
        Task CreateOrUpdateContact(FreshdeskContact contact, string freshdeskDomain);
    }
}
