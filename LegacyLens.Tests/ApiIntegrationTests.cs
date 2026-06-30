using System.Net;
using System.Net.Http.Json;
using LegacyLens.Api.Models;
using LegacyLens.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LegacyLens.Tests;

[TestClass]
public class ApiIntegrationTests
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    private static JsonSerializerOptions CreateJsonOptions()
    {
        JsonSerializerOptions options = new(JsonSerializerDefaults.Web);

        options.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        );

        return options;
    }

    [TestMethod]
    public async Task Health_ShouldReturnOk()
    {
        await using WebApplicationFactory<Program> factory = new();

        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/health", TestContext.CancellationToken);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        string responseText = await response.Content.ReadAsStringAsync(TestContext.CancellationToken);

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

    [TestMethod]
    public async Task Index_ShouldReturnCodebaseIndexForExistingDirectory()
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
                "    public void Run()",
                "    {",
                "    }",
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
                "/index",
                request,
                cancellationToken: TestContext.CancellationToken
            );

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            CodebaseIndex? codebaseIndex = await response.Content.ReadFromJsonAsync<CodebaseIndex>(
                JsonOptions,
                TestContext.CancellationToken
            );

            Assert.IsNotNull(codebaseIndex);

            Assert.AreEqual(rootPath, codebaseIndex.RootPath);
            Assert.AreEqual(1, codebaseIndex.Summary.FileCount);
            Assert.AreEqual(0, codebaseIndex.Summary.ReadErrorCount);
            Assert.AreEqual(6, codebaseIndex.Summary.TotalLineCount);
            Assert.AreEqual(6, codebaseIndex.Summary.TotalNonEmptyLineCount);
            Assert.AreEqual(1, codebaseIndex.Summary.TotalCodeItemCount);

            Assert.HasCount(1, codebaseIndex.Files);

            FileIndexEntry file = codebaseIndex.Files[0];

            Assert.AreEqual("Program.cs", file.RelativePath);
            Assert.AreEqual(".cs", file.Extension);
            Assert.IsTrue(file.IsReadSuccessfully);
            Assert.AreEqual(6, file.LineCount);
            Assert.AreEqual(6, file.NonEmptyLineCount);

            Assert.HasCount(1, file.CodeItems);

            CodeItem codeItem = file.CodeItems[0];

            Assert.AreEqual(CodeItemKind.Class, codeItem.Kind);
            Assert.AreEqual("Program", codeItem.Name);
            Assert.AreEqual(1, codeItem.LineNumber);
            Assert.AreEqual("public class Program", codeItem.Signature);

            Assert.IsNotEmpty(file.CodeChunks);
        }
        finally
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, recursive: true);
            }
        }
    }
}