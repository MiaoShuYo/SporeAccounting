using Microsoft.AspNetCore.Mvc;
using SP.Common.Nacos;

namespace SP.Gateway.Controllers;

[ApiController]
[Route("openapi/proxy")]
public class OpenApiProxyController : ControllerBase
{
    private readonly INacosClient _nacosClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWebHostEnvironment _env;

    public OpenApiProxyController(
        INacosClient nacosClient,
        IHttpClientFactory httpClientFactory,
        IWebHostEnvironment env)
    {
        _nacosClient = nacosClient;
        _httpClientFactory = httpClientFactory;
        _env = env;
    }

    [HttpGet("{serviceName}")]
    public async Task<IActionResult> GetOpenApiDoc(string serviceName, CancellationToken ct)
    {
        if (!_env.IsDevelopment() && _env.EnvironmentName != "Local")
        {
            return NotFound();
        }

        Uri baseUri;
        try
        {
            baseUri = await _nacosClient.ResolveAsync(serviceName, ct: ct);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                $"No healthy instances found for service '{serviceName}'.");
        }

        var docUrl = new Uri(baseUri, "/openapi/v1.json");

        var httpClient = _httpClientFactory.CreateClient("OpenApiProxy");
        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync(docUrl, ct);
        }
        catch (HttpRequestException)
        {
            return StatusCode(StatusCodes.Status502BadGateway,
                $"Failed to reach service '{serviceName}' at {docUrl}.");
        }

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode,
                $"Service '{serviceName}' returned {(int)response.StatusCode} for its OpenAPI document.");
        }

        var json = await response.Content.ReadAsStringAsync(ct);
        return Content(json, "application/json");
    }
}
