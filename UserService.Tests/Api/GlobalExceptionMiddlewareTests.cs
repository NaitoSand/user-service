using System.Net;
using System.Text.Json;
using UserService.Api.V1;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace UserService.Tests.Api;

/// <summary>
/// Integration tests for <see cref="GlobalExceptionMiddleware"/> without relying on Program or Host project.
/// </summary>
public class GlobalExceptionMiddlewareTests
{
    [Fact]
    public async Task Should_Return_ProblemDetails_On_Unhandled_Exception()
    {
        // Arrange
        using var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder.UseEnvironment("Development");
                webBuilder
                    .UseTestServer()
                    .Configure(app =>
                    {
                        app.UseMiddleware<GlobalExceptionMiddleware>();
                        app.Run(_ => throw new InvalidOperationException("Test exception"));
                    });
            })
            .StartAsync();

        var client = host.GetTestClient();

        // Act
        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");

        var problem = JsonSerializer.Deserialize<ProblemDetails>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        problem.Should().NotBeNull();
        problem.Title.Should().Be("Internal Server Error");
        problem.Status.Should().Be((int)HttpStatusCode.InternalServerError);
        problem.Detail.Should().Be("Test exception");
    }

    [Fact]
    public async Task Should_Pass_Through_When_No_Exception()
    {
        // Arrange
        using var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .Configure(app =>
                    {
                        app.UseMiddleware<GlobalExceptionMiddleware>();
                        app.Run(async ctx => await ctx.Response.WriteAsync("OK"));
                    });
            })
            .StartAsync();

        var client = host.GetTestClient();

        // Act
        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Be("OK");
    }
}
