using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazorClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

           // builder.Services.AddTransient(sp => new HttpClient ());
            //builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddHttpClient("api")
    .AddHttpMessageHandler(sp =>
    {
        var handler = sp.GetService<AuthorizationMessageHandler>()
            .ConfigureHandler(
                authorizedUrls: new[] { "https://localhost:5002" },
                scopes: new[] { "weatherapi" });
        return handler;
    });
            builder.Services.AddScoped(
   sp => sp.GetService<IHttpClientFactory>().CreateClient("api"));
            builder.Services.AddOidcAuthentication(options =>
            {
                // Configure your authentication provider options here.
                // For more information, see https://aka.ms/blazor-standalone-auth
                builder.Configuration.Bind("oidc", options.ProviderOptions);
            });
           
            await builder.Build().RunAsync();
        }
    }
}
