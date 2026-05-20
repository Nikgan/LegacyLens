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
        await using WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>();

        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/health");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        string responseText = await response.Content.ReadAsStringAsync();

        StringAssert.Contains(responseText, "LegacyLens.Api");
        StringAssert.Contains(responseText, "ok");
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

            await using WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>();

            HttpClient client = factory.CreateClient();

            IndexRequest request = new IndexRequest()
            {
                RootPath = rootPath,
                Recursive = true
            };

            HttpResponseMessage response = await client.PostAsJsonAsync(
                "/index/summary",
                request
            );

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            IndexSummary? summary = await response.Content.ReadFromJsonAsync<IndexSummary>();

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
        await using WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>();

        HttpClient client = factory.CreateClient();

        IndexRequest request = new IndexRequest()
        {
            RootPath = Path.Combine(
                Path.GetTempPath(),
                $"MissingLegacyLensDirectory_{Guid.NewGuid()}"
            ),
            Recursive = true
        };

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/index",
            request
        );

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

        ApiError? error = await response.Content.ReadFromJsonAsync<ApiError>();

        Assert.IsNotNull(error);
        StringAssert.Contains(error.Error, "Directory not found");
    }
}