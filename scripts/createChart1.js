module.exports = main;

var $ = require('jquery');

const labelsURL = "http://localhost:7071/api/GetLabels";
const dataURL = "http://localhost:7071/api/GetData";

// According to (https://v8.dev/blog/fast-async), this is faster than promises, even though it does the same thing
async function callURL(url) {
    return $.ajax({
        type: "POST",
        url: url,
        dataType: "json",
        success: function(data) {
            return data
        },
        error: function(error) {
            return error
        }
    })
}

function createChart1([labels,data]) {
    new Chartist.Line(
        '#chart1', 
        {
            labels: labels,
            series: data
        }, 
        {
            fullWidth: true,
            chartPadding: 
            {
                right: 40
            }
        }
    );
}

function logError(error){
    console.error(`There was an error: ${error.status} (${error.statusText})`);
}

async function main(){
    await Promise.all([callURL(labelsURL), callURL(dataURL)]).then(createChart1).catch(logError);
}