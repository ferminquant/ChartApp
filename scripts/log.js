module.exports.logError = logError;
module.exports.logMsg = logMsg;

function logError(error){
    console.error(`There was an error: ${error.status} (${error.statusText})`);
}

function logMsg(msg){
    console.log(msg);
}