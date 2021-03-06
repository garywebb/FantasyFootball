using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace ScoreScraper
{
    public static class ScoreScraperOrchestrator
    {
        [FunctionName("ScoreScraperOrchestrator")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            [Blob("player-data/scores", FileAccess.Write)] Stream output)
        {
            var result = await context.CallActivityAsync<string>("ScoreScraperOrchestrator_Hello", null);
            using (var streamWriter = new StreamWriter(output))
            {
                await streamWriter.WriteAsync(result);
                await streamWriter.FlushAsync();
                output.Position = 0;
            }
        }

        [FunctionName("ScoreScraperOrchestrator_Hello")]
        public static string SayHello([ActivityTrigger] object _, ILogger log)
        {
            log.LogInformation($"Saying hello to Gary.");
            return $"Hello Gary from durable function!";
        }

        [FunctionName("ScoreScraperStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("ScoreScraperOrchestrator", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}