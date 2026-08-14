using API.helpers;
using API.Services;
using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureOpenApi();
builder.Services.GetValidationServices();
builder.Services.GetInfrastructureServices(builder);
builder.Services.GetApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
