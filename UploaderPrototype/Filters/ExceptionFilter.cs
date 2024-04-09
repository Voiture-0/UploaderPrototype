using UploaderPrototype.Logging;

namespace UploaderPrototype.Filters;

public class ExceptionFilter : IEndpointFilter
{
    private readonly ILogger _logger;
    
    public ExceptionFilter(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ExceptionFilter>();
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            return await next(context);
        }
        catch (FileNotFoundException ex)
        {
            LogEvents.LogError(_logger, ex);
            return TypedResults.NotFound();
        }
        catch (Exception ex)
        {
            LogEvents.LogError(_logger, ex);
            return TypedResults.Problem(ex.Message);
        }
    }
}
