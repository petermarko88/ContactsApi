using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ContactsApi.Tests;

public class ContactTests
{
    [Fact]
    public async Task CreateContact()
    {
        await using var application = new TestingApplication();

        var client = application.CreateClient();

        var result = await client.PostAsJsonAsync("/contact", new ContactPerson
        {
            FirstName = "Peter", 
            LastName = "Marko",
            Email = "x@y.z",
            Phone = "1234567"
        });

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("\"It works!\"", await result.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task CreateContactValidatesObject()
    {
        await using var application = new WebApplicationFactory<Program>();
        var client = application.CreateClient();

        var result = await client.PostAsJsonAsync("/contact", new ContactPerson());

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

        var validationResult = await result.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        Assert.NotNull(validationResult);
        Assert.Equal("The FirstName field is required.", validationResult!.Errors["FirstName"][0]);
        Assert.Equal("The LastName field is required.", validationResult!.Errors["LastName"][0]);
        Assert.Equal("The Email field is required.", validationResult!.Errors["Email"][0]);
        Assert.Equal("The Phone field is required.", validationResult!.Errors["Phone"][0]);
    }
}