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
using Microsoft.Data.SqlClient;

namespace Company.Function
{
    public static class GetLabels
    {
        [FunctionName("GetLabels")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetLabels processed a request.");
            List<string> responseMessage = new List<string>();

            var connectionString = Environment.GetEnvironmentVariable("SqlConnection");  
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var text = @"select format(date, 'yyyyMM') as yearmonth
                            from transactions
                            group by format(date, 'yyyyMM')
                            order by 1 " ;
                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                responseMessage.Add(reader["yearmonth"].ToString());
                            }
                        }
                    }
                }
            }
            
            return new OkObjectResult(responseMessage);
        }
    }
}
