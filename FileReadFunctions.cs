using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;

namespace hello;

public class FileReadFunctions
{
    private readonly IConfiguration _configuration;
    private const string HardcodedFilePath = @"C:\Users\Inno\azure\sample.txt";

    public FileReadFunctions(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [Function("Home")]
    public HttpResponseData Home([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/html; charset=utf-8");
        response.WriteString("""
<html>
  <body>
    <h2>Read file demo</h2>
    <ul>
      <li><a href="/api/file-hardcoded">Hard-coded path</a></li>
      <li><a href="/api/file-config">Config path</a></li>
    </ul>
  </body>
</html>
""");
        return response;
    }

    [Function("FileHardcoded")]
    public HttpResponseData FileHardcoded([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "file-hardcoded")] HttpRequestData req)
    {
        return ReadFile(req, HardcodedFilePath);
    }

    [Function("FileConfig")]
    public HttpResponseData FileConfig([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "file-config")] HttpRequestData req)
    {
        var configuredPath = _configuration["FilePath"] ?? string.Empty;
        return ReadFile(req, configuredPath);
    }

    private static HttpResponseData ReadFile(HttpRequestData req, string path)
    {
        var response = req.CreateResponse(string.IsNullOrWhiteSpace(path) ? HttpStatusCode.BadRequest : File.Exists(path) ? HttpStatusCode.OK : HttpStatusCode.NotFound);
        response.Headers.Add("Content-Type", "text/html; charset=utf-8");

        if (string.IsNullOrWhiteSpace(path))
        {
            response.WriteString("<html><body><h3>File path is missing.</h3></body></html>");
            return response;
        }

        if (!File.Exists(path))
        {
            response.WriteString($"<html><body><h3>File not found: {System.Net.WebUtility.HtmlEncode(path)}</h3></body></html>");
            return response;
        }

        var content = File.ReadAllText(path);
        response.WriteString($"""
<html>
  <body>
    <h3>File path: {System.Net.WebUtility.HtmlEncode(path)}</h3>
    <pre>{System.Net.WebUtility.HtmlEncode(content)}</pre>
  </body>
</html>
""");

        return response;
    }
}
