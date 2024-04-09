using Amazon.S3;
using Amazon.S3.Transfer;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application;

public class View
{
    public class Request : IRequest<Response>
    {
        public required string FileName { get; set; }
    }

    public class Response
    {
        public bool Found { get; set; } = true;
        public string? FilePath { get; set; }

        public Response(string filePath)
        {
            FilePath = filePath;
        }
        private Response() {}
        public static Response NotFound()
        {
            return new Response
            {
                Found = false,
            };
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.FileName).Must(fn => !string.IsNullOrWhiteSpace(fn)).WithMessage("A filename is required.");
        }
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public Handler(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _bucketName = configuration["S3BucketName"];
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                var filePath = Path.Combine("LocalFiles", request.FileName);

                if (!File.Exists(filePath))
                {
                    var transferUtility = new TransferUtility(_s3Client);
                    await transferUtility.DownloadAsync(filePath, _bucketName, request.FileName, cancellationToken);
                }
                
                return new Response(filePath);
            }
            catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Response.NotFound();
            }
        }
    }
}
