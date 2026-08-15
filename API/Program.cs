using API;
using API.Helpers;
using API.Routes;
using API.Services;
using Application;
using Hangfire;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.GetValidationServices();
builder.Services.GetInfrastructureServices(builder);
builder.Services.GetApplicationServices();
builder.Services.GetApiServices();
builder.Services.AddAuthorization();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(
        "/docs",
        options =>
        {
            options.Title = "TrackFlow API";
            options.DarkMode = true;
            options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
            options.AddPreferredSecuritySchemes("Bearer");

            // Remove the nested MapScalarApiReference call—it's redundant
        }
    );
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        await db.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to apply database migrations");
        throw;
    }
}
app.UseGlobalExceptionHandling(app.Environment);
app.UseHttpsRedirection();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = [new JobsAuth()] });
app.MapGraphQL();
app.MapEndpoints();
app.Run();
