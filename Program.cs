using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OpenAIBlazorApp.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenAIBlazorApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            // Register HttpClient for API requests
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<OpenAIService>();


            await builder.Build().RunAsync();
        }
    }
}
