# ChartApp

A simple app to test charting with a javascript library that reads data using Azure serverless functions and serverless SQL DB.

Azure Functions:
1) LoadCSVtoDb: it triggers with a CSV file from an Azure Blob storage, and saves it to SQL Database.
2) GetLabels: gets the list of year/months with data
3) GetData: gets the data with an accumulated sum per year/month

Your Azure account is expected to already have:
* An SQL database with the transactions database
* A Blob Storage account with a csv folder

The web app is simple, and I think I modularized it a bit too much for its purpose:
* ChartApp.html: it shows a chart built from the javascript calls to the azure functions
* ChartApp.js: simply calls createChart1.js. It would call createChart2.js as well, if it existed.
* createChart1.js: calls Azure Functions to get the data for the chart and creates it. Uses helper.js to POST the functions, and log.js to log any info or errors.
* helper.js: has a function to POST to a URL. A GET function would be placed here, but I didn't need any GET.
* log.js: two simple functions to log errors of info. Not really needed, and maybe it would have been better to use an existing logging package instead of this.
* bundle.js: I create this with browserify and watchify, to be able to code javascript with modules and avoid getting into callback hell.
  * For manual building: browserify ChartApp.js -o bundle.js
  * For automatic building: watchify ChartApp.js -o bundle.js