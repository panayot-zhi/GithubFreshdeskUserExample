using GithubFreshdeskUserExample.Contracts;
using GithubFreshdeskUserExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GithubFreshdeskUserExample.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IntegrationController : ControllerBase
    {
        private readonly IIntegrationService _integrationService;

        public IntegrationController(IIntegrationService integrationService)
        {
            _integrationService = integrationService;
        }

        [HttpPost]
        public async Task Integrate(string githubUsername, string freshdeskDomain)
        {
            await _integrationService.IntegrateUser(githubUsername, freshdeskDomain);
        }
    }
}
