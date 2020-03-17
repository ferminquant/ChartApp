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
    public static class GetData
    {
        [FunctionName("GetData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetData processed a request.");
            List<List<double>> responseMessage = new List<List<double>>();
            List<double> list = new List<double>();

            var connectionString = Environment.GetEnvironmentVariable("SqlConnection");  
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var text = @"select yearmonth, sum(amount) over(order by yearmonth) amount 
                            from    (
                                    select  cast(format(date, 'yyyyMM') as int) as yearmonth, 
                                            sum(amount/17.5) as amount
                                    from    transactions
                                    group by cast(format(date, 'yyyyMM') as int)
                                    ) a " ;
                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                double amount;
                                if (double.TryParse(reader["amount"].ToString(), out amount)) {
                                    list.Add(amount);
                                }
                                else {
                                    list.Add(-1.1);
                                }                                
                            }
                        }
                    }
                }
            }

            responseMessage.Add(list);
            return new OkObjectResult(responseMessage);
        }
    }
}
