(function(){function r(e,n,t){function o(i,f){if(!n[i]){if(!e[i]){var c="function"==typeof require&&require;if(!f&&c)return c(i,!0);if(u)return u(i,!0);var a=new Error("Cannot find module '"+i+"'");throw a.code="MODULE_NOT_FOUND",a}var p=n[i]={exports:{}};e[i][0].call(p.exports,function(r){var n=e[i][1][r];return o(n||r)},p,p.exports,r,e,n,t)}return n[i].exports}for(var u="function"==typeof require&&require,i=0;i<t.length;i++)o(t[i]);return o}return r})()({1:[function(require,module,exports){
var createChart1 = require('./createChart1');

document.addEventListener("DOMContentLoaded", createChart1);

},{"./createChart1":2}],2:[function(require,module,exports){
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
    await Promise.all([helper.postData(labelsURL), helper.postData(dataURL)]).then(createChart1).catch(log.logError);
}
},{"./helper":3,"./log":4}],3:[function(require,module,exports){
module.exports.postData = postData;

// According to (https://v8.dev/blog/fast-async), this is faster than promises, even though it does the same thing
async function postData(url = '', data = {}) {
    // Default options are marked with *
    const response = await fetch(url, {
      method: 'POST', // *GET, POST, PUT, DELETE, etc.
      mode: 'cors', // no-cors, *cors, same-origin
      cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
      credentials: 'same-origin', // include, *same-origin, omit
      headers: {
        'Content-Type': 'application/json'
        // 'Content-Type': 'application/x-www-form-urlencoded',
      },
      redirect: 'follow', // manual, *follow, error
      referrerPolicy: 'no-referrer', // no-referrer, *client
      body: JSON.stringify(data) // body data type must match "Content-Type" header
    });
    return await response.json(); // parses JSON response into native JavaScript objects
}
},{}],4:[function(require,module,exports){
module.exports.logError = logError;
module.exports.logMsg = logMsg;

function logError(error){
    console.error(`There was an error: ${error.status} (${error.statusText})`);
}

function logMsg(msg){
    console.log(msg);
}
},{}]},{},[1]);
