using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AzureStorageBenchmark
{
    public class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices((context, services) => services.AddSingleton(new BlobServiceClient(context.Configuration.GetValue<string>("AzureWebJobsStorage"))))
                .Build();

            await host.RunAsync();
        }
    }
}