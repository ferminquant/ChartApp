module.exports.callURL = callURL;

var $ = require('jquery');

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