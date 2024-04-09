using Carter;
using MediatR;
using Application;

using Result = Microsoft.AspNetCore.Http.HttpResults.Results<
    Microsoft.AspNetCore.Http.HttpResults.Ok,
    Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult,
    Microsoft.AspNetCore.Http.HttpResults.NotFound
    >;
using UploaderPrototype.Filters;

namespace UploaderPrototype.Endpoints;

public class ViewEndpoint : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/{fileName}", ViewGet)
            .AddEndpointFilter<ExceptionFilter>();
    }

    private async Task<Result> ViewGet(ISender sender, HttpContext httpContext, string fileName)
    {
        var result = await sender.Send(new View.Request
        {
            FileName = fileName,
        });

        if (!result.Found) TypedResults.NotFound();

        httpContext.Response.Headers.ContentDisposition = $"inline; filename=\"{fileName}\"";
        using var fs = new FileStream(result.FilePath!, FileMode.Open);
        await fs.CopyToAsync(httpContext.Response.Body);
        
        return TypedResults.Ok();
    }
}
