using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.Auth.Admin.Models;
using Voidwell.Auth.Admin.Services;
using Voidwell.Auth.Data.Models;

namespace Voidwell.Auth.Admin.Controllers
{
    [Route("admin/client")]
    [SecurityHeaders]
    [Authorize("IsAdmin")]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<ClientApiDto>>> GetAllClients([FromQuery]string search = "", [FromQuery]int page = 1)
        {
            var clients = await _clientService.GetClientsAsync(search, page);

            return Ok(clients);
        }

        [HttpGet("{clientId}")]
        public async Task<ActionResult<ClientApiDto>> GetClientById(string clientId)
        {
            var client = await _clientService.GetClientAsync(clientId);
            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        [HttpPost]
        public async Task<ActionResult<ClientApiDto>> CreateClient([FromBody]ClientApiDto client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdClientDto = await _clientService.CreateClientAsync(client);

            return Created("client", createdClientDto);
        }

        [HttpPut("{clientId}")]
        public async Task<ActionResult<ClientApiDto>> UpdateClient(string clientId, [FromBody]ClientApiDto clientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedClientDto = await _clientService.UpdateClientAsync(clientId, clientDto);

            return Ok(updatedClientDto);
        }

        [HttpGet("{clientId}/secret")]
        public async Task<ActionResult<IEnumerable<SecretApiDto>>> GetClientSecrets(string clientId)
        {
            var secrets = await _clientService.GetClientSecretsAsync(clientId);

            return Ok(secrets);
        }

        [HttpPost("{clientId}/secret")]
        public async Task<ActionResult<CreatedSecretResponse>> CreateClientSecret(string clientId, [FromBody]SecretRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var secret = await _clientService.CreateClientSecretAsync(clientId, request);

            return Created($"client/{clientId}/secret", secret);
        }

        [HttpDelete("{clientId}/secret/{secretId}")]
        public async Task<ActionResult> DeleteClientSecret(string clientId, int secretId)
        {
            await _clientService.DeleteClientSecretAsync(clientId, secretId);

            return NoContent();
        }

        [HttpDelete("{clientId}")]
        public async Task<ActionResult> DeleteClient(string clientId)
        {
            await _clientService.RemoveClientAsync(clientId);

            return NoContent();
        }
    }
}
