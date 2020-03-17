using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
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
        public static void Run(
            [BlobTrigger("csv/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, 
            [Blob("csv/{name}", Connection = "AzureWebJobsStorage")]CloudBlockBlob myBlob2, 
            [CosmosDB(
                databaseName: "budgetdb",
                collectionName: "transactions",
                ConnectionStringSetting = "CosmosDBConnection")]
                IAsyncCollector<CSVItem> itemsOut,
            [CosmosDB(ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
            string name, 
            ILogger log)
        {
            try {
                log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} ");

                // Step 1: delete all existing documents
                client.DeleteDocumentCollectionAsync("transactions");
                client.CreateDocumentCollectionIfNotExistsAsync("budgetdb", new DocumentCollection { Id = "transactions" });
                RequestOptions requestOptions = new RequestOptions { PartitionKey = new PartitionKey("1"), EnableScriptLogging = true };
                //client.ExecuteStoredProcedureAsync<string>(UriFactory.CreateStoredProcedureUri("budgetdb", "transactions", "bulkDelete"), requestOptions, "SELECT * FROM c");  
                log.LogInformation($"Executed Delete Store Procedure"); 
                
                // Step 2: read CSV and save to db
                List<CSVItem> items = readCSV(myBlob, log);
                // tried using IAsyncCollector but it would lose data with 200+ lines in a file.
                // calling an sproc once per item would also lose data
                //foreach (CSVItem item in items){
                //    itemsOut.AddAsync(item);
                //}
                client.ExecuteStoredProcedureAsync<string>(UriFactory.CreateStoredProcedureUri("budgetdb", "transactions", "createDoc"), requestOptions, JsonConvert.SerializeObject(items));  
                log.LogInformation($"items count: {items?.Count}");

                // Step 3: at the end delete file
                myBlob2.DeleteIfExistsAsync();
                log.LogInformation($"Executed Delete File"); 
            }
            catch (Exception ex){
                log.LogInformation($"Error in Run: {ex?.Message}");
                throw;
            }
        }

        private static string readString(string[] splitLine, int index, out int newIndex){
            newIndex = index + 1;
            return (splitLine.Length >= (index+1)) ? splitLine[index] : "";
        }

        private static List<CSVItem> readCSV(Stream myBlob, ILogger log){
            // tried using LINQtoCSV, but it failed randomly with files that had 200+ lines
            // it was probably a bug with file upload from VS Code. The file got corrupted when uploaded from the extension, but not from the portal.
            try {
                List<CSVItem> items = new List<CSVItem>();
                using (StreamReader sr = new StreamReader(myBlob))
                {
                    string line;
                    int lineCnt = 1;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //log.LogInformation($"Line {lineCnt}: {line}");
                        if(lineCnt == 1) {
                            lineCnt++;
                            continue;
                        }
                        lineCnt++;
                        string[] splitLine = line.Split(';');
                        int index = 0;
                        CSVItem item = new CSVItem();
                        item.Date = readString(splitLine, index, out index);
                        item.Title = readString(splitLine, index, out index);
                        item.Comment = readString(splitLine, index, out index);
                        item.MainCategory = readString(splitLine, index, out index);
                        item.Subcategory = readString(splitLine, index, out index);
                        item.Account = readString(splitLine, index, out index);
                        item.Amount = readString(splitLine, index, out index);
                        items.Add(item);
                    }                    
                }
                return items;
            }
            catch (Exception ex){
                log.LogInformation($"Error in readCSV: {ex?.Message}");
                throw;
            }
        }
    }

    public class CSVItem
    {
        public CSVItem(){
            partition = "1";
        }

        public string partition { get; set;}

        public string Date { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public string MainCategory { get; set; }
        public string Subcategory { get; set; }
        public string Account { get; set; }
        public string Amount { get; set; }
    }
}
