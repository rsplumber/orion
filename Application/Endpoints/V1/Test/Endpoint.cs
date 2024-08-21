using Core.Files;
using FastEndpoints;
using Newtonsoft.Json;

namespace Application.Endpoints.V1.Test;

file sealed class Endpoint : EndpointWithoutRequest
{
    private readonly IDeleteFileService _deleteFileService;

    public Endpoint(IDeleteFileService deleteFileService)
    {
        _deleteFileService = deleteFileService;
    }

    public override void Configure()
    {
        Get("test");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // //read json file
        // using StreamReader reader = new("/home/say1/dev/orion/Application/Endpoints/V1/Test/detective_net_publicntification_file_info.json");
        // var json = await reader.ReadToEndAsync(ct);
        // var links = JsonConvert.DeserializeObject<List<LinkModel>>(json);
        // var count = 0;
        //
        // if (links != null)
        //     foreach (var link in links)
        //     {
        //         try
        //         {
        //             if (link.Link != null) await _deleteFileService.DeleteAsync(link.Link, ct);
        //             count++;
        //         }
        //         catch (Exception e)
        //         {
        //             Console.WriteLine(count);
        //         }
        //     }

        await SendOkAsync(ct);
    }
}

file
    sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "test endpoint system";
        Description = "test endpoint the system";
        Response(200, "Successfully");
    }
}