var ajax_call = function () {
    var eventID = $('#eventID').val();
    console.log(eventID);
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/CompetitorList?id=' + eventID,
        success: CompetitorList,
        error: errorOnAjax
    });
}

function CompetitorList(data) {
    $('#Competitors').html(data["compTable"]);
}

function errorOnAjax() {
    console.log("ERROR in ajax request.");
}


var interval = 1000 * 5;

window.setInterval(ajax_call, interval);