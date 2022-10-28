using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FileRepo1.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileRepo1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrimaryController : ControllerBase
    {

        private readonly AppRegistrationInformation appRegistrationInformation;
        private readonly SecretClientOptions options;
        private readonly SecretClient client;
        public PrimaryController(IOptions<AppRegistrationInformation> appRegistrationInformation)
        {
            this.appRegistrationInformation = appRegistrationInformation.Value;
            options = new()
            {
                Retry = 
                {
                    Delay= TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    Mode = RetryMode.Exponential
                 }
            };
#if DEBUG
            client = new SecretClient(new Uri(this.appRegistrationInformation.VaultUri), new ClientSecretCredential(this.appRegistrationInformation.TenantId, this.appRegistrationInformation.ClientId, this.appRegistrationInformation.ClientSecret), options); ////  to be used for debug deployments
#else
            client = client ?? new SecretClient(new Uri(this.appRegistrationInformation.VaultUri), new DefaultAzureCredential(), options); ////  to be used for Azure deployments; just provide the Managed Identity Client Id
#endif
        }

        // GET: api/<PrimaryController>
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            List<string> list = new();
            KeyVaultSecret secret = await client.GetSecretAsync("TestKey");
            list.Add(secret.Value);
            return list;
        }

        // GET api/<PrimaryController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PrimaryController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PrimaryController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PrimaryController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
