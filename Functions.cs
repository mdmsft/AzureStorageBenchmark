using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureStorageBenchmark
{
    public class Functions
    {
        private readonly BlobServiceClient blobServiceClient;

        public Functions(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
        }

        [Function("blob")]
        public async Task<HttpResponseData> Blob(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req,
            [BlobInput("samples/Sample.mp4", Connection = "AzureWebJobsStorage")] Stream sample,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("HttpFunction");
            logger.LogInformation("message logged");

            var blobContainerClient = blobServiceClient.GetBlobContainerClient("test");

            await blobContainerClient.CreateIfNotExistsAsync();

            var blobClient = blobContainerClient.GetBlobClient("sample.mp4");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await blobClient.UploadAsync(sample);

            stopwatch.Stop();

            var response = req.CreateResponse(HttpStatusCode.OK);
            var buffer = Encoding.UTF8.GetBytes(stopwatch.ElapsedMilliseconds.ToString());
            await response.Body.WriteAsync(buffer, 0, buffer.Length);
            return response;
        }
    }
}