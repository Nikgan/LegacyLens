using System.Net;
using System.Net.Http.Json;
using LegacyLens.Api.Models;
using LegacyLens.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LegacyLens.Tests;

[TestClass]
public class ApiIntegrationTests
{
    [TestMethod]
    public async Task Health_ShouldReturnOk()
    {
        await using WebApplicationFactory<Program> factory = new();

        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/health");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        string responseText = await response.Content.ReadAsStringAsync();

        Assert.Contains("LegacyLens.Api", responseText);
        Assert.Contains("ok", responseText);
    }

    [TestMethod]
    public async Task IndexSummary_ShouldReturnSummaryForExistingDirectory()
    {
        string rootPath = Path.Combine(
            Path.GetTempPath(),
            $"LegacyLensApiTests_{Guid.NewGuid()}"
        );

        try
        {
            Directory.CreateDirectory(rootPath);

            File.WriteAllLines(
                Path.Combine(rootPath, "Program.cs"),
                [
                    "public class Program",
                    "{",
                    "}"
                ]
            );

            await using WebApplicationFactory<Program> factory = new();

            HttpClient client = factory.CreateClient();

            IndexRequest request = new()
            {
                RootPath = rootPath,
                Recursive = true
            };

            HttpResponseMessage response = await client.PostAsJsonAsync(
                "/index/summary",
                request,
                cancellationToken: TestContext.CancellationToken
            );

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            IndexSummary? summary = await response.Content.ReadFromJsonAsync<IndexSummary>(TestContext.CancellationToken);

            Assert.IsNotNull(summary);
            Assert.AreEqual(1, summary.FileCount);
            Assert.AreEqual(0, summary.ReadErrorCount);
            Assert.AreEqual(3, summary.TotalLineCount);
            Assert.AreEqual(3, summary.TotalNonEmptyLineCount);
            Assert.AreEqual(1, summary.TotalCodeItemCount);
            Assert.AreEqual(1, summary.TotalCodeChunkCount);
        }
        finally
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task Index_ShouldReturnNotFoundForMissingDirectory()
    {
        await using WebApplicationFactory<Program> factory = new();

        HttpClient client = factory.CreateClient();

        IndexRequest request = new()
        {
            RootPath = Path.Combine(
                Path.GetTempPath(),
                $"MissingLegacyLensDirectory_{Guid.NewGuid()}"
            ),
            Recursive = true
        };

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/index",
            request,
            cancellationToken: TestContext.CancellationToken
            );

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

        ApiError? error = await response.Content.ReadFromJsonAsync<ApiError>(TestContext.CancellationToken);

        Assert.IsNotNull(error);
        Assert.Contains("Directory not found", error.Error);
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public async Task Index_ShouldReturnBadRequestWhenRootPathIsEmpty()
    {
        await using WebApplicationFactory<Program> factory = new();

        HttpClient client = factory.CreateClient();

        IndexRequest request = new()
        {
            RootPath = "",
            Recursive = true
        };

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/index",
            request,
            cancellationToken: TestContext.CancellationToken
        );

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        ApiError? error = await response.Content.ReadFromJsonAsync<ApiError>(
            TestContext.CancellationToken
        );

        Assert.IsNotNull(error);
        Assert.Contains("RootPath is required", error.Error);
    }
}