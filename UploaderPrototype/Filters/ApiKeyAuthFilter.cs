namespace UploaderPrototype.Filters;

public class ApiKeyAuthFilter : IEndpointFilter
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    public ApiKeyAuthFilter(ILoggerFactory loggerFactory, IConfiguration configuration)
    {
        _logger = loggerFactory.CreateLogger<ApiKeyAuthFilter>();
        _configuration = configuration;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("API-Key", out var extractedApiKey) || extractedApiKey != _configuration["ApiKey"])
        {
            _logger.LogError($"API Key is invalid or missing: \"{extractedApiKey}\"");
            return TypedResults.Unauthorized();
        }
        return await next(context);
    }
}
