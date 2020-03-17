# ChartApp

Mar 13 2020

With this app I aim to use cloud technologies to upload a csv file and get simple graphs that will help me in my daily life. 
I use Azure Serverless Functions as the backend, CosmosDB as the database, and a File Storage for uploading and triggering functions.

I aim to make it as free as possible.
Azure provides 1,000,000 function requests per month for free, has a free tier for CosmosDB, and offers for 12 months free 5GB in File Storage. When the 12 months are up, I expect little cost, since the storage would be an intermediate steps to upload a file, trigger a function that uses the file and then deletes it. 

I expect some limitations. The file might be too big for a function to read, parse and send to CosmosDB, but for the files I intend to use it will probably not reach the 5 minutes of execution time for a serveless function. The file will be around 2 MB. I might eventually do a function to split the file into multiple pieces and call a different function to parse it multiple times. It is probably overkill for my intended use, so I will leave that for last.

Mar 17 2020

Gave up on using CosmosDb yesterday, because there is no simple way of deleting everything before loading. A delete stored procedure has to be executed multiple times before it deletes everything. Today I tried the Table Storage, and it has the same delete problem. You can delete the whole table, but it is not really deleted, it is queued to be deleted and you cannot use it or recreate it until that is done. So, delete and reinsert does not work. You can delete all rows, but again it is complex logic for a simple task. Will now try with a serverless Azure SQL Database.

That worked, reading from it in Azure Functions, first graph works.