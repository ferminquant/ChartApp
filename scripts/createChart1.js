module.exports = main;

var helper = require('./helper');
var log = require('./log')

const labelsURL = "http://localhost:7071/api/GetLabels";
const dataURL = "http://localhost:7071/api/GetData";

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

async function main(){
    await Promise.all([helper.callURL(labelsURL), helper.callURL(dataURL)]).then(createChart1).catch(log.logError);
}