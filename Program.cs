var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var app = builder.Build();

const string HardcodedFilePath = @"C:\Users\Inno\azure\sample.txt";

app.MapGet("/", () => Results.Content("""
<html>
  <body>
    <h2>Read file demo</h2>
    <ul>
      <li><a href="/file-hardcoded">Hard-coded path</a></li>
      <li><a href="/file-config">Config path</a></li>
    </ul>
  </body>
</html>
""", "text/html"));

app.MapGet("/file-hardcoded", () => ReadTextFile(HardcodedFilePath));
app.MapGet("/file-config", (IConfiguration config) => ReadTextFile(config["FilePath"] ?? string.Empty));

app.Run();

static IResult ReadTextFile(string path)
{
    if (string.IsNullOrWhiteSpace(path))
    {
        return Results.BadRequest("File path is missing.");
    }

    if (!File.Exists(path))
    {
        return Results.NotFound($"File not found: {path}");
    }

    var content = File.ReadAllText(path);

    return Results.Content($"""
<html>
  <body>
    <h3>File path: {System.Net.WebUtility.HtmlEncode(path)}</h3>
    <pre>{System.Net.WebUtility.HtmlEncode(content)}</pre>
  </body>
</html>
""", "text/html");
}
