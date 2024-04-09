using Amazon.S3;
using Amazon.S3.Transfer;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using shortid;
using shortid.Configuration;

namespace Application;

public class Upload
{
    public class Request : IRequest<Response>
    {
        public required string FileName { get; set; }
        public required long Length { get; set; }
        public required Stream Content { get; set; }
    }

    public class Response
    {
        public required string FileName { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r).Must(r => r != null).WithMessage("Upload a file.");
            RuleFor(r => r.FileName).Must(fn => !string.IsNullOrWhiteSpace(fn)).WithMessage("A filename is required.");
            RuleFor(r => r.Length).Must(l => l > 0).WithMessage("A file is required.");
            RuleFor(r => r.Content).Must(c => c != null).WithMessage("A file is required.");
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
            var fileExtension = Path.GetExtension(request.FileName);
            var newFileName = $"{GenerateId()}{fileExtension}";

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(request.Content, _bucketName, newFileName, cancellationToken);

            return new Response
            {
                FileName = newFileName,
            };
        }

        private static string GenerateId()
        {
            return ShortId.Generate(new GenerationOptions(useNumbers: true, useSpecialCharacters: false, length: 8));
        }
    }
}
