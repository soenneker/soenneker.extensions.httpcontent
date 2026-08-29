[![](https://img.shields.io/nuget/v/soenneker.extensions.httpcontent.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.httpcontent/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.httpcontent/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.httpcontent/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.httpcontent.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.httpcontent/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.httpcontent/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.httpcontent/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.HttpContent
A collection of helpful HttpContent extension methods.

## Installation

```bash
dotnet add package Soenneker.Extensions.HttpContent
```

## Quick start

```csharp
using Soenneker.Extensions.HttpContent;

// Given an existing System.Net.Http.HttpContent? named content:
var result = content.Clone();
```

## Common operations

- `Clone()` - Asynchronously copies the content body and headers into a new `HttpContent`; a null input returns `null`.
- `AddCookie()` - Adds a cookie to the HTTP content's headers.
- `Log()` - Reads the content as a string and writes it at debug level; it returns immediately without reading when debug logging is disabled.
- `ShouldUseStream()` - Determines whether the specified HTTP content should be processed using a stream based on its content length.
- `GetSmallContentBytes()` - Attempts to eagerly materialize the content of an `System.Net.Http.HttpContent` into a byte array, but only if the payload is small enough (below the internal threshold). This method avoids unnecessary allocations by only materializing a byte array when the content length is definitively small.
