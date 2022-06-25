using Microsoft.AspNetCore.Mvc.Testing;

namespace ContactsApi.Tests;

public class HelloWorldTests
{
    [Fact]
    public async void TestRootEndpoint()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        var response = await client.GetStringAsync("/");

        Assert.Equal("Hello World!", response);
    }
}