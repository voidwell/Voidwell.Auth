using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.Auth.Admin.Models;
using Voidwell.Auth.Admin.Services;
using Voidwell.Auth.Data.Models;
using Voidwell.Auth.IdentityServer.Models;

namespace Voidwell.Auth.Admin.Controllers;

[Route("admin/resource")]
[SecurityHeaders]
[Authorize("IsAdmin")]
public class ApiResourceController : Controller
{
    private readonly IApiResourceService _apiResourceService;

    public ApiResourceController(IApiResourceService apiResourceService)
    {
        _apiResourceService = apiResourceService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<ApiResourceApiDto>>> GetAllApiResources([FromQuery]string search = "", [FromQuery]int page = 1)
    {
        var apiResources = await _apiResourceService.GetApiResourcesAsync(search, page);

        return Ok(apiResources);
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<ApiResourceApiDto>> GetApiResourceById(string name)
    {
        var apiResource = await _apiResourceService.GetApiResourceAsync(name);
        if (apiResource == null)
        {
            return NotFound();
        }

        return Ok(apiResource);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResourceApiDto>> CreateApiResource([FromBody]ApiResourceApiDto apiResource)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdApiResourceDto = await _apiResourceService.CreateApiResourceAsync(apiResource);

        return Created("resource", createdApiResourceDto);
    }

    [HttpPut("{name}")]
    public async Task<ActionResult<ApiResourceApiDto>> UpdateApiResource(string name, [FromBody]ApiResourceApiDto apiResourceDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedApiResourceDto = await _apiResourceService.UpdateApiResourceAsync(name, apiResourceDto);

        return Ok(updatedApiResourceDto);
    }

    [HttpGet("{name}/secret")]
    public async Task<ActionResult<IEnumerable<SecretApiDto>>> GetApiResourceSecrets(string name)
    {
        var secrets = await _apiResourceService.GetApiResourceSecretsAsync(name);

        return Ok(secrets);
    }

    [HttpPost("{name}/secret")]
    public async Task<ActionResult<CreatedSecretResponse>> CreateApiResourceSecret(string name, [FromBody]SecretRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var secret = await _apiResourceService.CreateApiResourceSecretAsync(name, request);

        return Created($"resource/{name}/secret", secret);
    }

    [HttpDelete("{name}/secret/{secretId}")]
    public async Task<ActionResult> DeleteApiResourceSecret(string name, int secretId)
    {
        await _apiResourceService.DeleteApiResourceSecretAsync(name, secretId);

        return NoContent();
    }

    [HttpDelete("{name}")]
    public async Task<ActionResult> DeleteApiResource(string name)
    {
        await _apiResourceService.RemoveApiResourceAsync(name);

        return NoContent();
    }
}
