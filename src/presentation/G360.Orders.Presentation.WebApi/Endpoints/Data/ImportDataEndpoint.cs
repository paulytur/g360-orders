using System.Net;
using FastEndpoints;
using G360.Orders.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace G360.Orders.Presentation.WebApi.Endpoints.Data;

public class ImportDataEndpoint(IDataImportService importService, IWebHostEnvironment env, IConfiguration config) : EndpointWithoutRequest<DataImportResult>
{
    public override void Configure()
    {
        Post("/data/import");
        AllowAnonymous();
        Description(d => d.WithTags("Data"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Use config "DataImport:Folder" if set, else default docs/Data relative to repo root
        var configuredPath = config["DataImport:Folder"];
        var dataPath = string.IsNullOrWhiteSpace(configuredPath)
            ? System.IO.Path.GetFullPath(System.IO.Path.Combine(env.ContentRootPath, "..", "..", "..", "docs", "Data"))
            : System.IO.Path.GetFullPath(configuredPath);
        if (!Directory.Exists(dataPath))
        {
            await SendAsync(
                new DataImportResult { Errors = { $"Data folder not found: {dataPath}" } },
                (int)HttpStatusCode.NotFound,
                ct);
            return;
        }

        var result = await importService.ImportFromFolderAsync(dataPath, ct);
        await SendAsync(result, result.Success ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest, ct);
    }
}
