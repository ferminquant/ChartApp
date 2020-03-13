using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LINQtoCSV;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace Company.Function
{
    public static class LoadCSVtoCosmosdb
    {
        [FunctionName("LoadCSVtoCosmosdb")]
        public static void Run([BlobTrigger("csv/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} ");
            try{
                List<CSVItem> items;

                CsvFileDescription inputFileDescription = new CsvFileDescription
                {
                    SeparatorChar = ';',
                    FirstLineHasColumnNames = true,

                };

                CsvContext cc = new CsvContext();
                
                using (StreamReader sr = new StreamReader((Stream)myBlob))
                {
                    items = cc.Read<CSVItem>(sr, inputFileDescription).ToList();
                }

                foreach(CSVItem item in items){
                    string json = JsonConvert.SerializeObject(item, Formatting.Indented);
                    log.LogInformation($"Reading CSV file :{json} ");
                }
            }
            catch(Exception ex){
                log.LogInformation($"Error Message :{ex.Message} ");
                if (ex.InnerException != null) {
                    log.LogInformation($"Inner Exception :{ex.InnerException.Message} ");
                }
            }

            //at the end delete file
            // myBlob has to be a CloudBlockBlob and needs: using Microsoft.WindowsAzure.Storage.Blob
            //myBlob.DeleteIfExistsAsync();
        }
    }

    public class CSVItem
    {
        [CsvColumn(Name = "Date")]
        public string Date { get; set; }

        [CsvColumn(Name = "Title")]
        public string Title { get; set; }

        [CsvColumn(Name = "Comment")]
        public string Comment { get; set; }

        [CsvColumn(Name = "Main category")]
        public string MainCategory { get; set; }

        [CsvColumn(Name = "Subcategory")]
        public string Subcategory { get; set; }

        [CsvColumn(Name = "Account")]
        public string Account { get; set; }

        [CsvColumn(Name = "Amount")]
        public string Amount { get; set; }
    }
}
