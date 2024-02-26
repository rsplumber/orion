using Core.Files;
using FastEndpoints;
using FluentValidation;

namespace Application.Endpoints.V1.Converts;

file sealed class Endpoint : Endpoint<Request>
{
    public override void Configure()
    {
        Get("converts/files/{id}");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await SendOkAsync(new Response
        {
            Link = IdLink.From(req.Id)
        }, ct);
    }
}

file sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Get file link by guid";
        Description = "Get file link by guid";
        Response(200, "Successful");
    }
}

file sealed record Request
{
    public Guid Id { get; set; }
}

file sealed record Response
{
    public string Link { get; set; } = default!;
}

file sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty().WithMessage("Enter Id")
            .NotNull().WithMessage("Enter Id");
    }
}