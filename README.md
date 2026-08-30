[![](https://img.shields.io/nuget/v/soenneker.extensions.httpcontent.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.httpcontent/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.httpcontent/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.httpcontent/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.httpcontent.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.httpcontent/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.httpcontent/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.httpcontent/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.HttpContent
Extensions for cloning, inspecting, and logging `HttpContent` without forcing every caller to choose its own buffering strategy.

## Installation

```bash
dotnet add package Soenneker.Extensions.HttpContent
```

## Clone content for another request

```csharp
using Soenneker.Extensions.HttpContent;

using HttpContent? copy = await originalContent.Clone(cancellationToken: cancellationToken);

if (copy is not null)
{
    using var request = new HttpRequestMessage(HttpMethod.Post, destination)
    {
        Content = copy
    };

    await httpClient.SendAsync(request, cancellationToken);
}
```

`Clone()` copies the body and content headers into independently disposable content. It returns `null` for a `null` input. Cloning reads the original body, so do not assume a non-seekable original stream can be read again afterward.

An `IMemoryStreamUtil` can be supplied when the application already uses pooled memory streams:

```csharp
using HttpContent? copy = await originalContent.Clone(memoryStreamUtil, cancellationToken);
```

## Choose buffering or streaming

`ShouldUseStream()` returns `true` when `Content-Length` is unknown or greater than 64 KiB. `GetSmallContentBytes()` reads a body only when its declared length is at most 64 KiB:

```csharp
if (content.ShouldUseStream())
{
    await using Stream stream = await content.ReadAsStreamAsync(cancellationToken);
    await ProcessStream(stream, cancellationToken);
}
else
{
    ReadOnlyMemory<byte> bytes = await content.GetSmallContentBytes(cancellationToken);
    ProcessBytes(bytes.Span);
}
```

`GetSmallContentBytes()` returns empty memory for `null`, zero-length, unknown-length, and oversized content. Use `ShouldUseStream()` when the distinction matters. These decisions trust the declared `Content-Length`; they are a routing aid, not an enforcement limit.

## Add a cookie header

```csharp
content.AddCookie("session", token, "https://api.example.com/orders");

// Or provide the domain and path separately:
content.AddCookie("session", token, "api.example.com", "/orders");
```

When given an absolute URI and no explicit path, `AddCookie()` uses the URI host and absolute path. Otherwise, it treats the third argument as a domain and defaults the path to `/`. The resulting header has the form `name=value; Domain=domain; Path=path`.

## Debug logging

```csharp
await content.Log(logger, cancellationToken);
```

`Log()` reads the complete body as text and writes it at `Debug` level. It does not read the body when debug logging is disabled. Avoid it for large, binary, credential-bearing, or personally identifiable payloads unless the configured log destination is appropriate for that data.
