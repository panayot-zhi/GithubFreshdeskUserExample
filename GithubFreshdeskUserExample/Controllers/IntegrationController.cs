using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GithubFreshdeskUserExample.Contracts;
using GithubFreshdeskUserExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GithubFreshdeskUserExample.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> Integrate(string githubUsername, string freshdeskDomain)
        {
            if (string.IsNullOrEmpty(githubUsername))
            {
                return BadRequest();
            }

            if (!githubUsername.Equals(User.Identity?.Name))
            {
                return Forbid();
            }

            if (string.IsNullOrEmpty(freshdeskDomain))
            {
                return BadRequest();
            }

            await _integrationService.IntegrateUser(githubUsername, freshdeskDomain);
            
            return Ok();
        }
    }
}
