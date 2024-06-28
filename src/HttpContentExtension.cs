using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Soenneker.Extensions.Task;

namespace Soenneker.Extensions.HttpContent;

/// <summary>
/// A collection of helpful HttpContent extension methods
/// </summary>
public static class HttpContentExtension
{
    public static async ValueTask<System.Net.Http.HttpContent> Clone(this System.Net.Http.HttpContent content)
    {
        if (content == null)
            return null;

        var ms = new System.IO.MemoryStream();

        await content.CopyToAsync(ms).NoSync();
        ms.Position = 0;

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

        if (path == null && Uri.TryCreate(domainOrUri, UriKind.Absolute, out Uri? uri))
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
}