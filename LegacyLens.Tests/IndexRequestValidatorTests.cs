using System.Net;
using System.Text.Json;
using LegacyLens.Api.Models;
using LegacyLens.Api.Validation;
using Microsoft.AspNetCore.Http;

namespace LegacyLens.Tests;

[TestClass]
public class IndexRequestValidatorTests
{
    [TestMethod]
    public void Validate_ShouldReturnBadRequest_WhenRootPathIsEmpty()
    {
        IndexRequestValidator validator = new();

        IndexRequest request = new()
        {
            RootPath = "",
            Recursive = true
        };

        IResult? result = validator.Validate(request);

        Assert.IsNotNull(result);

        (int statusCode, ApiError error) = ReadApiErrorResult(result);

        Assert.AreEqual((int)HttpStatusCode.BadRequest, statusCode);
        Assert.Contains("RootPath is required", error.Error);
    }

    [TestMethod]
    public void Validate_ShouldReturnNotFound_WhenDirectoryDoesNotExist()
    {
        IndexRequestValidator validator = new();

        IndexRequest request = new()
        {
            RootPath = Path.Combine(
                Path.GetTempPath(),
                $"MissingLegacyLensDirectory_{Guid.NewGuid()}"
            ),
            Recursive = true
        };

        IResult? result = validator.Validate(request);

        Assert.IsNotNull(result);

        (int statusCode, ApiError error) = ReadApiErrorResult(result);

        Assert.AreEqual((int)HttpStatusCode.NotFound, statusCode);
        Assert.Contains("Directory not found", error.Error);
    }

    [TestMethod]
    public void Validate_ShouldReturnNull_WhenRequestIsValid()
    {
        string rootPath = Path.Combine(
            Path.GetTempPath(),
            $"LegacyLensValidatorTests_{Guid.NewGuid()}"
        );

        try
        {
            Directory.CreateDirectory(rootPath);

            IndexRequestValidator validator = new();

            IndexRequest request = new()
            {
                RootPath = rootPath,
                Recursive = true
            };

            IResult? result = validator.Validate(request);

            Assert.IsNull(result);
        }
        finally
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, recursive: true);
            }
        }
    }

    private static (int StatusCode, ApiError Error) ReadApiErrorResult(IResult result)
    {
        IStatusCodeHttpResult statusCodeResult = (IStatusCodeHttpResult)result;
        IValueHttpResult valueResult = (IValueHttpResult)result;

        object? value = valueResult.Value;

        Assert.IsNotNull(statusCodeResult.StatusCode);
        Assert.IsNotNull(value);
        Assert.IsInstanceOfType<ApiError>(value);

        ApiError error = (ApiError)value;

        return (statusCodeResult.StatusCode.Value, error);
    }
}