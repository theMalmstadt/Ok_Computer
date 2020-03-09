var interval = 1000 * 5;

var ajax_match_update = function () {
    var eventID = $("#eventID").val();
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/MatchUpdate?id=' + eventID,
        complete: setTimeout(ajax_match, 0),
        error: errorOnAjax
    });
}

var ajax_call = function () {
    var eventID = $('#eventID').val();
    console.log(eventID);
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/MatchList?id=' + eventID,
        success: MatchList,
        complete: setTimeout(ajax_match_update, 0),
        error: errorOnAjax
    });
}

var ajax_match = function () {
    var eventID = $('#eventID').val();
    console.log(eventID);
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/CompetitorList?id=' + eventID,
        success: CompetitorList,
        complete: setTimeout(ajax_call, interval),
        error: errorOnAjax
    });
}

function CompetitorList(data) {
    $('#Competitors').html(data["compTable"]);
}

function MatchList(data) {
    $('#Matches').html(data["matchTable"]);
}

function errorOnAjax() {
    console.log("ERROR in ajax request.");
}

window.setTimeout(ajax_call, 0);