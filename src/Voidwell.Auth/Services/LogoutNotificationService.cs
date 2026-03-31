using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Server;
using Voidwell.Auth.Data;
using Voidwell.Auth.Data.Entities;
using Voidwell.Auth.Services.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Voidwell.Auth.Services;

public class LogoutNotificationService : ILogoutNotificationService
{
    private readonly AuthDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<OpenIddictServerOptions> _serverOptions;
    private readonly string _issuer;
    private readonly ILogger<LogoutNotificationService> _logger;

    public LogoutNotificationService(
        AuthDbContext dbContext,
        IHttpClientFactory httpClientFactory,
        IOptions<OpenIddictServerOptions> serverOptions,
        IConfiguration configuration,
        ILogger<LogoutNotificationService> logger)
    {
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
        _serverOptions = serverOptions;
        _issuer = configuration["Issuer"] ?? "https://auth.voidwell.com";
        _logger = logger;
    }

    public async Task<IReadOnlyList<string>> GetFrontChannelLogoutUrisAsync(string subjectId)
    {
        var clients = await GetLogoutClientsAsync(subjectId);
        return clients
            .Where(c => c.FrontChannelLogoutUri is not null)
            .Select(c => c.FrontChannelLogoutUri!)
            .ToList();
    }

    public async Task SendBackChannelLogoutNotificationsAsync(string subjectId)
    {
        var clients = await GetLogoutClientsAsync(subjectId);
        var backChannelClients = clients.Where(c => c.BackChannelLogoutUri is not null).ToList();

        if (backChannelClients.Count == 0)
        {
            return;
        }

        var signingCredentials = _serverOptions.Value.SigningCredentials.FirstOrDefault();
        if (signingCredentials is null)
        {
            _logger.LogWarning("No signing credentials configured; skipping back-channel logout notifications.");
            return;
        }

        var httpClient = _httpClientFactory.CreateClient("BackChannelLogout");

        foreach (var client in backChannelClients)
        {
            try
            {
                var token = CreateLogoutToken(_issuer, subjectId, client.ClientId!, signingCredentials);
                using var content = new FormUrlEncodedContent(
                [
                    new KeyValuePair<string, string>("logout_token", token)
                ]);
                var response = await httpClient.PostAsync(client.BackChannelLogoutUri, content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Back-channel logout for client {ClientId} returned {StatusCode}.",
                        client.ClientId, (int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Back-channel logout failed for client {ClientId}.", client.ClientId);
            }
        }
    }

    private async Task<List<AuthApplication>> GetLogoutClientsAsync(string subjectId)
    {
        var authAppIds = await _dbContext.Set<AuthAuthorization>()
            .Where(a => a.Subject == subjectId
                     && a.Status == Statuses.Valid
                     && a.Application != null)
            .Select(a => a.Application.Id)
            .Distinct()
            .ToListAsync();

        if (authAppIds.Count == 0)
        {
            return [];
        }

        return await _dbContext.Set<AuthApplication>()
            .Where(app => authAppIds.Contains(app.Id)
                       && (app.BackChannelLogoutUri != null || app.FrontChannelLogoutUri != null))
            .ToListAsync();
    }

    private static string CreateLogoutToken(string issuer, string subject, string audience, SigningCredentials signingCredentials)
    {
        var handler = new JwtSecurityTokenHandler();
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            IssuedAt = DateTime.UtcNow,
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("events",
                    "{\"http://schemas.openid.net/event/backchannel-logout\":{}}",
                    JsonClaimValueTypes.Json)
            ]),
            SigningCredentials = signingCredentials
        };

        return handler.CreateEncodedJwt(descriptor);
    }
}
