using GithubFreshdeskUserExample.Contracts;
using GithubFreshdeskUserExample.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace GithubFreshdeskUserExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpClient();

            builder.Services.AddTransient<IGitHubClient, GitHubClient>();
            builder.Services.AddTransient<IFreshdeskClient, FreshdeskClient>();
            builder.Services.AddTransient<IIntegrationService, IntegrationService>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    // Configure JWT options
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
