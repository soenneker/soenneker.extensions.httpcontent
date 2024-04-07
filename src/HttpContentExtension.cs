using System.Collections.Generic;
using System.Net.Http;
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
}
