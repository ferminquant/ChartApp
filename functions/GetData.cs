using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Company.Function
{
    public static class GetData
    {
        [FunctionName("GetData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetData processed a request.");
            List<List<double>> responseMessage = new List<List<double>> { 
                new List<double> {12, 9, 7, 8, 5 },
                new List<double> {2, 1, 3.5, 7, 3},
                new List<double> {1, 3, 4, 5, 6}
            };

            return new OkObjectResult(responseMessage);
        }
    }
}
