using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.Ai.QnA;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AnimalQna01
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBot<AnimalQnaBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);

                options.Middleware.Add(new CatchExceptionMiddleware<Exception>(async (context, exception) =>
                {
                    await context.TraceActivity("AnimalQnaBot Exception", exception);
                    await context.SendActivity("Sorry, it looks like something went wrong!");
                }));

                var qnaEndpoint = GetQnAMakerEndpoint(Configuration);
                options.Middleware.Add(new QnAMakerMiddleware(qnaEndpoint));
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }

        public static QnAMakerEndpoint GetQnAMakerEndpoint(IConfiguration configuration)
        {
            var host = configuration.GetSection("QnAMaker-Endpoint-Url")?.Value;
            var knowledgeBaseId = configuration.GetSection("QnAMaker-KnowledgeBaseId")?.Value;
            var endpointKey = configuration.GetSection("QnAMaker-SubscriptionKey")?.Value;
            return new QnAMakerEndpoint { Host = host, KnowledgeBaseId = knowledgeBaseId, EndpointKey = endpointKey };
        }
    }
}
