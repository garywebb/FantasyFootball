using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ScoreScraper
{
    public static class ScoreScraper
    {
        [FunctionName("ScoreScraper")]
        [return: Blob("player-data")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest request,
            //[Microsoft.Azure.WebJobs.Queue("playerDataQueue"), StorageAccount("AzureWebJobsStorage")] ICollector<string> msg,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = request.Query["name"];

            string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return $"Hello, {name}";

            //if (!String.IsNullOrEmpty(name))
            //{
            //    msg.Add($"Name passed to the function: {name}");
            //}

            //return name != null
            //    ? (ActionResult)new OkObjectResult($"Hello, {name}")
            //    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
