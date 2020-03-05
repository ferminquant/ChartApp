

$( document ).ready(function() {
    $.ajax({
        type: "GET",
        url: "http://localhost:7071/api/GetLabels",
        dataType: "json"
    }).done(function (res) {

        //create chart
        var label_list = res;        
        
        $.ajax({
            type: "GET",
            url: "http://localhost:7071/api/GetData",
            dataType: "json"
        }).done(function (data) {

            new Chartist.Line('#chart1', {
                labels: label_list,
                series: data
                }, {
                fullWidth: true,
                chartPadding: {
                right: 40
                }
            });

        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.log("AJAX call to GetData failed: " + textStatus + ", " + errorThrown);
        });


    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log("AJAX call to GetLabels failed: " + textStatus + ", " + errorThrown);
    });


    
});