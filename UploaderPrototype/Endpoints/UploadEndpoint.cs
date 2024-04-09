using Carter;
using MediatR;
using Application;
using UploaderPrototype.Filters;

using Result = Microsoft.AspNetCore.Http.HttpResults.Results<
    Microsoft.AspNetCore.Http.HttpResults.Ok<string>,
    Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult,
    Microsoft.AspNetCore.Http.HttpResults.UnauthorizedHttpResult
    >;

namespace UploaderPrototype.Endpoints;

public class UploadEndpoint : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/upload", UploadPost)
            .AddEndpointFilter<ExceptionFilter>()
            .AddEndpointFilter<ApiKeyAuthFilter>()
            .DisableAntiforgery();
    }

    private async Task<Result> UploadPost(ISender sender, HttpContext httpContext, IFormFile file)
    {
        var result = await sender.Send(new Upload.Request
        {
            FileName = file.FileName,
            Length = file.Length,
            Content = file.OpenReadStream(),
        });

        var fileUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/{result.FileName}";
            
        return TypedResults.Ok(fileUrl);
    }
}
