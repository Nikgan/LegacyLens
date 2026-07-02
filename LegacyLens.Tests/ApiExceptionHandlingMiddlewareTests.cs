using LegacyLens.Api.Middleware;
using LegacyLens.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Text.Json;

namespace LegacyLens.Tests;

[TestClass]
public class ApiExceptionHandlingMiddlewareTests
{
    [TestMethod]
    public async Task InvokeAsync_ShouldReturnInternalServerError_WhenUnhandledExceptionIsThrown()
    {
        DefaultHttpContext context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = _ => throw new InvalidOperationException("Test exception.");

        ApiExceptionHandlingMiddleware middleware = new(next, NullLogger<ApiExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        Assert.AreEqual(
            (int)HttpStatusCode.InternalServerError,
            context.Response.StatusCode
        );

        Assert.IsTrue(context.Response.ContentType?.StartsWith("application/json"));

        context.Response.Body.Position = 0;

        ApiError? error = await JsonSerializer.DeserializeAsync<ApiError>(
            context.Response.Body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web),
            TestContext.CancellationToken
        );

        Assert.IsNotNull(error);
        Assert.AreEqual("Unexpected server error.", error.Error);
    }

    public TestContext TestContext { get; set; } = null!;
}