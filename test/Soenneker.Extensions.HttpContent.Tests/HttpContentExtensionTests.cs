using Soenneker.Tests.FixturedUnit;
using Xunit;
using Xunit.Abstractions;

namespace Soenneker.Extensions.HttpContent.Tests;

[Collection("Collection")]
public class HttpContentExtensionTests : FixturedUnitTest
{
    public HttpContentExtensionTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {

    }
}
