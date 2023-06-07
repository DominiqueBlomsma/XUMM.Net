﻿using System.Threading.Tasks;
using XUMM.NET.SDK.Models.XAppJwt;
using XUMM.NET.SDK.Models.XAppJWT;

namespace XUMM.NET.SDK.Clients.Interfaces;

public interface IXummXAppJwtClient
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="oneTimeToken">UUID (token) received (URL get param.) when Xumm launches your xApp URL.</param>
    Task<XummXAppJwtAuthorizeResponse> AuthorizeAsync(string oneTimeToken);

    /// <summary>
    /// Key value store on a per user, per xApp basis. Store key/value data for one or more keys.
    /// </summary>
    /// <param name="jwt">The JWT obtained by calling the /authorize endpoint first.</param>
    /// <param name="key">Key/value store: one or more keys, separated by comma (lower case, a-z, 0-9, min. 3 chars)</param>
    Task<XummXAppJwtUserDataResponse> GetUserDataAsync(string jwt, string key);

    /// <summary>
    /// Key value store on a per user, per xApp basis. Store key/value data for one specific key. On a duplicate key, the existing value is updated.
    /// </summary>
    /// <param name="jwt">The JWT obtained by calling the /authorize endpoint first.</param>
    /// <param name="key">Key/value store: key (lower case, a-z, 0-9, min. 3 chars)</param>
    /// <param name="json">JSON body containing the data to store for this key.</param>
    Task<XummXAppJwtUserDataUpdateResponse> SetUserDataAsync(string jwt, string key, string json);

    /// <summary>
    /// Key value store on a per user, per xApp basis. Remove key&amp;value data for one specific key.
    /// </summary>
    /// <param name="jwt">The JWT obtained by calling the /authorize endpoint first.</param>
    /// <param name="key">Key/value store: key (lower case, a-z, 0-9, min. 3 chars)</param>
    Task<XummXAppJwtUserDataUpdateResponse> DeleteUserDataAsync(string jwt, string key);
}
