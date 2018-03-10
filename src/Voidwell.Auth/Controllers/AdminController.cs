using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Auth.Models;
using Voidwell.VoidwellAuth.Client;

namespace Voidwell.Auth.Controllers
{
    [Route("admin")]
    [SecurityHeaders]
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly IVoidwellClientStore _clientStore;
        private readonly IVoidwellResourceStore _resourceStore;

        public AdminController(IVoidwellClientStore clientStore, IVoidwellResourceStore resourceStore)
        {
            _clientStore = clientStore;
            _resourceStore = resourceStore;
        }

        [HttpGet("client")]
        public async Task<ActionResult> GetAllClients()
        {
            var clients = await _clientStore.GetAllClientsAsync();

            clients.ToList().ForEach(a => a.ClientSecrets?.Clear());

            return Ok(clients);
        }

        [HttpGet("client/{clientId}")]
        public async Task<ActionResult> GetClientById(string clientId)
        {
            var client = await _clientStore.FindClientByIdAsync(clientId);
            if (client == null)
            {
                return NotFound();
            }

            client.ClientSecrets.Clear();

            return Ok(client);
        }

        [HttpPost("client")]
        public async Task<ActionResult> CreateClient([FromBody]Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var storeClient = await _clientStore.CreateClientAsync(client);

            return Created("client", storeClient);
        }

        [HttpPut("client/{clientId}")]
        public async Task<ActionResult> UpdateClient(string clientId, [FromBody]Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var storeClient = await _clientStore.UpdateClientAsync(clientId, client);

            return Ok(storeClient);
        }

        [HttpPost("client/{clientId}/secret")]
        public async Task<ActionResult> CreateClientSecret(string clientId, [FromBody]SecretRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var secret = await _clientStore.CreateSecretAsync(clientId, request.Description, request.Expiration);

            return Created($"client/{clientId}/secret", secret);
        }

        [HttpDelete("client/{clientId}/secret")]
        public async Task<ActionResult> DeleteClientSecret(string clientId, [FromQuery]SecretRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _clientStore.DeleteSecretAsync(clientId, request.Description);

            return NoContent();
        }

        [HttpGet("resource")]
        public async Task<ActionResult> GetAllApiResources()
        {
            var apiResources = await _resourceStore.GetAllApiResourcesAsync();

            apiResources.ToList().ForEach(a => a.ApiSecrets?.Clear());

            return Ok(apiResources);
        }

        [HttpGet("resource/{apiResourceId}")]
        public async Task<ActionResult> GetApiResourceById(string apiResourceId)
        {
            var apiResource = await _resourceStore.FindApiResourceAsync(apiResourceId);
            if (apiResource == null)
            {
                return NotFound();
            }

            apiResource.ApiSecrets.Clear();

            return Ok(apiResource);
        }

        [HttpPost("resource")]
        public async Task<ActionResult> CreateApiResource([FromBody]ApiResource apiResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var storeApiResource = await _resourceStore.CreateApiResourceAsync(apiResource);

            return Created("resource", storeApiResource);
        }

        [HttpPut("resource/{apiResourceId}")]
        public async Task<ActionResult> UpdateApiResource(string apiResourceId, [FromBody]ApiResource apiResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var storeApiResource = await _resourceStore.UpdateApiResourceAsync(apiResourceId, apiResource);

            return Ok(storeApiResource);
        }

        [HttpPost("resource/{apiResourceId}/secret")]
        public async Task<ActionResult> CreateApiResourceSecret(string apiResourceId, [FromBody]SecretRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var secret = await _resourceStore.CreateSecretAsync(apiResourceId, request.Description, request.Expiration);

            return Created($"resource/{apiResourceId}/secret", secret);
        }

        [HttpDelete("resource/{apiResourceId}/secret")]
        public async Task<ActionResult> DeleteApiResourceSecret(string apiResourceId, [FromQuery]SecretRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _resourceStore.DeleteSecretAsync(apiResourceId, request.Description);

            return NoContent();
        }
    }
}
