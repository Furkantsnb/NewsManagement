using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace NewsManagement2;

public class NewsManagement2WebTestStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplication<NewsManagement2WebTestModule>();
    }

    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
        app.InitializeApplication();
    }
}
