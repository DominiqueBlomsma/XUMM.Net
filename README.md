# XUMM.NET [![XUMM.NET](https://github.com/DominiqueBlomsma/XUMM.Net/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/DominiqueBlomsma/XUMM.Net/actions/workflows/dotnet.yml)
Interact with the XUMM SDK from .NET / C# environments.

## Install XUMM.NET in server-side Blazor App

1. Create a new ASP.NET Core Blazor Server-App project.
2. Install NuGet-package: `XUMM.NET`.
3. Add the following code block at the end of the services in `Program.cs`:

```c#
builder.Services.AddXummNet(builder.Configuration);
```

4. Add the following configuration in `appsettings.json`:

```json
  "Xumm": {
    "RestClientAddress": "https://xumm.app/api/v1",
    "ApiKey": "", // API Key which can be obtained from the xumm Developer Console.
    "ApiSecret": "" // API Secret which can be obtained from the xumm Developer Console.
  },
````

5. Hit `F5`: you're now running a completely empty Blazor server-side App with XUMM.NET. 
6. Start building your app. For reference, browse the [XUMM.NET.ServerApp](https://github.com/DominiqueBlomsma/XUMM.Net/tree/main/XUMM.NET.ServerApp) to see all the options.

### Credentials

The SDK will look in your appsettings for the `ApiKey` and `ApiSecret` values. Optionally the `RestClientAddress` can be provided. An [example appsettings](https://github.com/DominiqueBlomsma/XUMM.Net/blob/main/XUMM.NET.ServerApp/appsettings.json) file is provided in this repository. Alternatively you can provide your XUMM API Key & Secret by passing them like:

```C#
builder.Services.AddXummNet(o =>
    {
        o.ApiKey = "00000000-0000-0000-000-000000000000";
        o.ApiSecret = "00000000-0000-0000-000-000000000000";
    });
```

Create your app and get your XUMM API credentials at the XUMM Developer Console:

- https://apps.xumm.dev

More information about the XUMM API, payloads, the API workflow, sending Push notifications, etc. please check the XUMM API Docs: 

- https://xumm.readme.io/docs


##### IXummMiscClient.GetPingAsync()

The `ping` method allows you to verify API access (valid credentials) and returns some info on your XUMM APP:

```C#
@inject IXummMiscClient _miscClient
var pong = await _miscClient.GetPingAsync();
```

Returns: [`XummPong`](https://github.com/DominiqueBlomsma/XUMM.Net/blob/main/XUMM.Net/Models/Misc/XummPong.cs)
```C#
var pong = new XummPong
{
    Pong = true,
    Auth = new XummAuth
    {
        Quota = new Dictionary<string, object>
        {
            { "ratelimit", null}
        },
        Application = new XummApplication
        {
            Uuidv4 = "00000000-1111-2222-3333-aaaaaaaaaaaa",
            Name = "My XUMM APP",
            WebhookUrl = "",
            Disabled = 0
        },
        Call = new XummCall
        {
            Uuidv4 = "bbbbbbbb-cccc-dddd-eeee-111111111111"
        }
    }
}
```


##### IXummMiscClient.GetKycStatusAsync()

The `GetKycStatusAsync` return the KYC status of a user based on a user_token, issued after the
user signed a Sign Request (from your app) before (see Payloads - Intro).

If a user token specified is invalid, revoked, expired, etc. the method will always
return `XummKycStatus.None`, just like when a user didn't go through KYC. You cannot distinct a non-KYC'd user
from an invalid token.

Alternatively, KYC status can be retrieved for an XPRL account address: the address selected in
XUMM when the session KYC was initiated by.

```C#
@inject IXummMiscClient _miscClient
var kycStatus = await _miscClient.GetKycStatusAsync("00000000-0000-0000-0000-000000000000");
```

Returns: [`XummKycStatus`](https://github.com/DominiqueBlomsma/XUMM.Net/blob/main/XUMM.Net/Enums/XummKycStatus.cs)

###### Notes on KYC information

- Once an account has successfully completed the XUMM KYC flow, the KYC flag will be applied to the account even if the identity document used to KYC expired. The flag shows that the account was **once** KYC'd by a real person with a real identity document.
- Please note that the KYC flag provided by XUMM can't be seen as a "all good, let's go ahead" flag: it should be used as **one of the data points** to determine if an account can be trusted. There are situations where the KYC flag is still `true`, but an account can no longer be trusted. Eg. when account keys are compromised and the account is now controlled by a 3rd party. While unlikely, depending on the level of trust required for your application you may want to mitigate against these kinds of fraud.


##### IXummMiscClient.GetTransactionAsync()

The `GetTransactionAsync` method allows you to get the transaction outcome (mainnet)
live from the XRP ledger, as fetched for you by the XUMM backend.

**Note**: it's best to retrieve these results **yourself** instead of relying on the XUMM platform to get live XRPL transaction information! You can use the **[xrpl-txdata](https://www.npmjs.com/package/xrpl-txdata)** package to do this:  
[![npm version](https://badge.fury.io/js/xrpl-txdata.svg)](https://www.npmjs.com/xrpl-txdata)

```C#
@inject IXummMiscClient _miscClient
var txInfo = await _miscClient.GetTransactionAsync("00000000-0000-0000-0000-000000000000");
```

Returns: [`XummTransaction`](https://github.com/DominiqueBlomsma/XUMM.Net/blob/main/XUMM.Net/Models/Misc/XummTransaction.cs)


#### App Storage

App Storage allows you to store a JSON object at the XUMM API platform, containing max 60KB of data.
Your XUMM APP storage is stored at the XUMM API backend, meaning it persists until you overwrite or delete it.

This data is private, and accessible only with your own API credentials. This private JSON data can be used to store credentials / config / bootstrap info / ... for your headless application (eg. POS device).

```C#
@inject IXummMiscAppStorageClient _miscAppStorageClient

var storageSet = await _miscClient.StoreAsync({name: 'Dominique', age: 32, male: true});
Console.WriteLine(storageSet.Stored)
// true

var storageGet = await _miscAppStorageClient.GetAsync()
Console.WriteLine(storageGet.Data)
// { name: 'Dominique', age: 32, male: true }

var storageDelete = await _miscAppStorageClient.ClearAsync()
Console.WriteLine(storageSet.Stored)
// true

var storageGetAfterDelete = await _miscAppStorageClient.GetAsync()
Console.WriteLine(storageGetAfterDelete.Data)
// null
```


#### Payloads

##### Intro

Payloads are the primary reason for the XUMM API (thus this SDK) to exist. The [XUMM API Docs explain '**Payloads**'](https://xumm.readme.io/docs/introduction) like this:

>  An XRPL transaction "template" can be posted to the XUMM API. Your transaction tample to sign (so: your "sign request") will be persisted at the XUMM API backend. We now call it a  a **Payload**. XUMM app user(s) can open the Payload (sign request) by scanning a QR code, opening deeplink or receiving push notification and resolve (reject or sign) on their own device.

A payload can contain an XRPL transaction template. Some properties may be omitted, as they will be added by the XUMM app when a user signs a transaction. A simple payload may look like this:

```C#
var payload = new XummPostJsonPayload(
        "{ \"TransactionType\": \"Payment\", " + 
        "\"Destination\": \"rwiETSee2wMz3SBnAG8hkMsCgvGy9LWbZ1\", " + 
        "\"Amount\": \"1337\" }");
```

As you can see the payload looks like a regular XRPL transaction, wrapped in an `TxJson` object, omitting the mandatory `Account`, `Fee` and `Sequence` properties. They will be added containing the correct values when the payload is signed by an app user.

Optionally (besides `TxJson`) a payload can contain these properties ([XummPayloadBodyBase definition](https://github.com/DominiqueBlomsma/XUMM.Net/blob/main/XUMM.Net/Models/Payload/XummPayloadBodyBase.cs)):

- `XummPayloadOptions` to define payload options like a return URL, expiration, etc.
- `XummPayloadCustomMeta` to add metadata, user instruction, your own unique ID, ...
- `UserToken` to push the payload to a user (after [obtaining a user specific token](https://xumm.readme.io/docs/pushing-sign-requests))

A [reference for payload options & custom meta](https://xumm.readme.io/reference/post-payload) can be found in the [API Docs](https://xumm.readme.io/reference/post-payload).

Instead of providing a `TxJson` transaction, a transaction formatted as HEX blob (string) can be provided in a `TxBlob` property.

##### IXummPayloadClient.GetAsync

To get payload details, status and if resolved & signed: results (transaction, transaction hash, etc.) you can `GetAsync()` a payload.

Note! Please don't use _polling_! The XUMM API offers Webhooks (configure your Webhook endpoint in the [Developer Console](https://apps.xumm.dev)) or use [a subscription](#payload-subscriptions-live-updates) to receive live payload updates (for non-SDK users: [Webhooks](https://xumm.readme.io/docs/payload-status)).

You can `GetAsync()` a payload by:
- Payload UUID  
  ```C#
  @inject IXummPayloadClient _payloadClient
  var payload = await _payloadClient.GetAsync("00000000-0000-0000-0000-000000000000");
  ```

- Passing a created Payload object (see: [IXummPayloadClient.CreateAsync](#sdkpayloadcreate))  
  ```C#
  @inject IXummPayloadClient _payloadClient
  var newPayload = new XummPostJsonPayload("{...}");
  var created = await _payloadClient.CreateAsync(newPayload);
  var payload = await _payloadClient.GetAsync(created);
  ```

If a payload can't be fetched (eg. doesn't exist), `null` will be returned, unless a second param (boolean) is provided to get the SDK to throw an Exception in case a payload can't be retrieved:

```C#
@inject IXummPayloadClient _payloadClient
var payload = await _payloadClient.GetAsync("00000000-0000-0000-0000-000000000000", true);
```

##### IXummPayloadClient.CreateAsync

To create a payload, a `TxJson` XRPL transaction can be provided. Alternatively, a transaction formatted as HEX blob (string) can be provided in a `TxBlob` property. **See the [intro](#intro) for more information about payloads.** Take a look at the [Developer Docs for more information about payloads](https://xumm.readme.io/docs/your-first-payload).

The response (see: [Developer Docs](https://xumm.readme.io/docs/payload-response-resources)) of a `IXummPayloadClient.CreateAsync()` operation, a `<XummPayloadResponse>` object, looks like this:

```C#
var payload = new XummPayloadResponse
{
    Uuid = "1289e9ae-7d5d-4d5f-b89c-18633112ce09",
    Next = new XummPayloadNextResponse
    {
        Always = "https://xumm.app/sign/1289e9ae-7d5d-4d5f-b89c-18633112ce09",
        NoPushMessageReceived = "https://xumm.app/sign/1289e9ae-7d5d-4d5f-b89c-18633112ce09/qr"
    },
    Refs = new XummPayloadRefsResponse
    {
        QrPng = "https://xumm.app/sign/1289e9ae-7d5d-4d5f-b89c-18633112ce09_q.png",
        QrMatrix = "https://xumm.app/sign/1289e9ae-7d5d-4d5f-b89c-18633112ce09_q.json",
        QrUriQualityOpts = new List<string> { "m", "q", "h" },
        WebsocketStatus = "wss://xumm.app/sign/1289e9ae-7d5d-4d5f-b89c-18633112ce09"
    },
    Pushed = true
};
```

The `Next.Always` URL is the URL to send the end user to, to scan a QR code or automatically open the XUMM app (if on mobile). If a `UserToken` has been provided as part of the payload data provided to `IXummPayloadClient.CreateAsync()`, you can see if the payload has been pushed to the end user. A button "didn't receive a push notification" could then take the user to the `Next.NoPushMessageReceived` URL. The alternatively user routing / instruction flows can be custom built using the QR information provided in the `XummPayloadRefsResponse` object, and a subscription for live status updates (opened, signed, etc.) using a WebSocket client can be setup by conneting to the `Refs.WebsocketStatus` URL. **Please note: this SDK already offers subscriptions. There's no need to setup your own WebSocket client, see [Payload subscriptions: live updates](#payload-subscriptions-live-updates).** There's more information about the [payload workflow](https://xumm.readme.io/docs/payload-workflow) and a [payload lifecycle](https://xumm.readme.io/docs/doc-payload-life-cycle) in the Developer Docs.
