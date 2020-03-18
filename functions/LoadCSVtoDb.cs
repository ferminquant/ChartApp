using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Data.SqlClient;
using LINQtoCSV;
using System.Linq;

namespace Company.Function
{
    public static class LoadCSVtoDb
    {
        [FunctionName("LoadCSVtoDb")]
        public static void Run(
            [BlobTrigger("csv/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, 
            [Blob("csv/{name}", Connection = "AzureWebJobsStorage")]CloudBlockBlob myBlob2,
            string name, 
            ILogger log)
        {
            try {
                DateTime startTime = DateTime.Now;
                log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} ");

                // Step 1: read CSV
                List<Transaction> items = readCSV(myBlob, log);
                log.LogInformation($"items count: {items?.Count}");

                // Step 2: write to db                
                var str = Environment.GetEnvironmentVariable("SqlConnection");         
                dbWriter dbw = new dbWriter(items, str);
                dbw.writeToDb();
                log.LogInformation($"Updated db"); 

                // Step 3: delete file at the end
                myBlob2.DeleteIfExistsAsync();
                log.LogInformation($"Executed Delete File"); 
                
                log.LogInformation($"Time to execute: {DateTime.Now - startTime}"); 
            }
            catch (Exception ex){
                log.LogInformation($"Error in Run: {ex?.Message}");
                throw;
            }
        }

        private static List<Transaction> readCSV(Stream myBlob, ILogger log){
            try {
                List<Transaction> items;

                CsvFileDescription inputFileDescription = new CsvFileDescription
                {
                    SeparatorChar = ';',
                    FirstLineHasColumnNames = true
                };
                CsvContext cc = new CsvContext();
                using (StreamReader sr = new StreamReader((Stream)myBlob))
                {
                    items = cc.Read<Transaction>(sr, inputFileDescription).ToList();
                }
                return items;
            }
            catch (Exception ex){
                log.LogInformation($"Error in readCSV: {ex?.Message}");
                throw;
            }
        }
    }

    public class dbWriter {
        
        public dbWriter(List<Transaction> transactions, string connectionString){
            this.transactions = transactions;
            this.connectionString = connectionString;
        }
        
        private List<Transaction> transactions;
        private string connectionString;
        
        public bool writeToDb(){
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var text = "TRUNCATE TABLE transactions; " + getInsertSqls();
                var tran = conn.BeginTransaction();
                using (SqlCommand cmd = new SqlCommand(text, conn, tran))
                {
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
            }
            return true;
        }

        private string getInsertSqls(){
            string value = "";
            foreach (Transaction i in transactions) {
                // TODO: this code is vulnerable to SQL Injection.
                value += $"INSERT INTO transactions (date,title,comment,main_category,sub_category,account,amount) VALUES ('{i.Date}','{i.Title.Replace("'","''")}','{i.Comment.Replace("'","''")}','{i.MainCategory.Replace("'","''")}','{i.Subcategory.Replace("'","''")}','{i.Account.Replace("'","''")}','{i.Amount}'); ";
            }
            return value;
        }
    }

    public class Transaction
    {
        [CsvColumn(Name = "Date")]
        public DateTime Date { get; set; }

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
        public double Amount { get; set; }
    }
}
