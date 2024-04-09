using Application;
using Carter;
using FluentValidation;
using System.Reflection;
using UploaderPrototype.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCarter();

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Upload>();
    cfg.RegisterServicesFromAssemblyContaining<UploadEndpoint>();
});

builder.Services.AddAWSService<Amazon.S3.IAmazonS3>();

builder.Configuration.AddSystemsManager(cfg =>
{   
    cfg.Path = builder.Configuration["SsmPath"];
    cfg.ReloadAfter = TimeSpan.FromHours(4);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapCarter();

app.Run();

