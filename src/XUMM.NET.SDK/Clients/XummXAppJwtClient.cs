﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using XUMM.NET.SDK.Clients.Interfaces;
using XUMM.NET.SDK.Configs;
using XUMM.NET.SDK.Models.XAppJwt;
using XUMM.NET.SDK.Models.XAppJWT;

namespace XUMM.NET.SDK.Clients;

public class XummXAppJwtClient : IXummXAppJwtClient
{
    private readonly IXummHttpClient _httpClient;
    private readonly ApiConfig _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="XummXAppJwtClient"/> class.
    /// </summary>
    public XummXAppJwtClient(IXummHttpClient httpClient,
        IOptions<ApiConfig> options)
    {
        _httpClient = httpClient;
        _config = options.Value;
    }

    /// <inheritdoc />
    public async Task<XummXAppJwtAuthorizeResponse> AuthorizeAsync(string oneTimeToken)
    {
        if (string.IsNullOrWhiteSpace(oneTimeToken))
        {
            throw new ArgumentException("Value cannot be null or white space", nameof(oneTimeToken));
        }

        var httpClient = _httpClient.GetHttpClient(true);
        httpClient.DefaultRequestHeaders.Add("X-API-OTT", oneTimeToken);
        return await _httpClient.GetAsync<XummXAppJwtAuthorizeResponse>(httpClient, "xapp-jwt/authorize");
    }

    /// <inheritdoc />
    public async Task<XummXAppJwtUserDataResponse> GetUserDataAsync(string jwt, string key)
    {
        var httpClient = _httpClient.GetHttpClient(false);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");

        return await _httpClient.GetAsync<XummXAppJwtUserDataResponse>(httpClient, $"xapp-jwt/userdata/{key}");
    }

    /// <inheritdoc />
    public async Task<XummXAppJwtUserDataUpdateResponse> SetUserDataAsync(string jwt, string key, string json)
    {
        var httpClient = _httpClient.GetHttpClient(false);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");

        return await _httpClient.PostAsync<XummXAppJwtUserDataUpdateResponse>(httpClient, $"xapp-jwt/userdata/{key}", json);
    }

    /// <inheritdoc />
    public async Task<XummXAppJwtUserDataUpdateResponse> DeleteUserDataAsync(string jwt, string key)
    {
        var httpClient = _httpClient.GetHttpClient(false);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");

        return await _httpClient.DeleteAsync<XummXAppJwtUserDataUpdateResponse>(httpClient, $"xapp-jwt/userdata/{key}");
    }
}
