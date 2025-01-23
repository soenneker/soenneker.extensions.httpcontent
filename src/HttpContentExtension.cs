using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Soenneker.Extensions.Stream;
using Soenneker.Extensions.Task;

namespace Soenneker.Extensions.HttpContent;

/// <summary>
/// A collection of helpful HttpContent extension methods
/// </summary>
public static class HttpContentExtension
{
    /// <summary>
    /// Clones the specified <see cref="System.Net.Http.HttpContent"/> instance asynchronously.
    /// </summary>
    /// <param name="content">The <see cref="System.Net.Http.HttpContent"/> instance to clone.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> representing the asynchronous operation,
    /// with a result of the cloned <see cref="System.Net.Http.HttpContent"/> instance, or <c>null</c> if the input content was <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This method creates a deep copy of the provided <see cref="System.Net.Http.HttpContent"/> instance,
    /// including its headers, by copying the content to a memory stream and then creating a new <see cref="StreamContent"/> instance.
    /// </remarks>
    /// <exception cref="OperationCanceledException">
    /// The task was canceled.
    /// </exception>
    public static async ValueTask<System.Net.Http.HttpContent?> Clone(this System.Net.Http.HttpContent? content, CancellationToken cancellationToken = default)
    {
        if (content is null)
            return null;

        var ms = new System.IO.MemoryStream();

        await content.CopyToAsync(ms, cancellationToken).NoSync();
        ms.ToStart();

        var result = new StreamContent(ms);

        foreach (KeyValuePair<string, IEnumerable<string>> header in content.Headers)
        {
            result.Headers.Add(header.Key, header.Value);
        }

        return result;
    }

    /// <summary>
    /// Adds a cookie to the HTTP content's headers.
    /// </summary>
    /// <param name="content">The HTTP content to which the cookie will be added.</param>
    /// <param name="cookieName">The name of the cookie.</param>
    /// <param name="cookieValue">The value of the cookie.</param>
    /// <param name="domainOrUri">
    /// The domain or URI to be used for the cookie.
    /// If both <paramref name="domainOrUri"/> and <paramref name="path"/> are provided, 
    /// <paramref name="domainOrUri"/> will be treated as the domain.
    /// If only <paramref name="domainOrUri"/> is provided, it will be treated as a URI.
    /// </param>
    /// <param name="path">
    /// The path to be used for the cookie.
    /// If <paramref name="path"/> is not provided and <paramref name="domainOrUri"/> is a URI, 
    /// the path will be extracted from the URI.
    /// If <paramref name="path"/> is not provided and <paramref name="domainOrUri"/> is a domain, 
    /// the default path will be set to "/".
    /// </param>
    public static void AddCookie(this System.Net.Http.HttpContent content, string cookieName, string cookieValue, string domainOrUri, string? path = null)
    {
        string domain;

        if (path is null && Uri.TryCreate(domainOrUri, UriKind.Absolute, out Uri? uri))
        {
            domain = uri.Host;
            path ??= uri.AbsolutePath;
        }
        else
        {
            domain = domainOrUri;
            path ??= "/";
        }

        StringBuilder cookieBuilder = new StringBuilder(cookieName.Length + cookieValue.Length + domain.Length + path.Length + 20)
            .Append(cookieName)
            .Append('=')
            .Append(cookieValue)
            .Append("; Domain=")
            .Append(domain)
            .Append("; Path=")
            .Append(path);

        content.Headers.Add("Cookie", cookieBuilder.ToString());
    }

    /// <summary>
    /// Asynchronously logs the content of an <see cref="System.Net.Http.HttpContent"/> instance using the provided <see cref="ILogger"/>.
    /// </summary>
    /// <param name="content">
    /// The <see cref="System.Net.Http.HttpContent"/> to be logged.
    /// </param>
    /// <param name="logger">
    /// The <see cref="ILogger"/> instance used to log the content.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method reads the HTTP content as a string asynchronously and logs it with a debug-level severity. 
    /// It utilizes dependency injection for the <see cref="ILogger"/> to ensure structured logging.
    /// Ensure the <see cref="HttpContent"/> is not disposed before calling this method.
    /// </remarks>
    public static async ValueTask Log(this System.Net.Http.HttpContent content, ILogger logger)
    {
        string log = await content.ReadAsStringAsync().NoSync();

        logger.LogDebug("{log}", log);
    }
}