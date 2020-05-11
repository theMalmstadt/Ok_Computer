var interval = 1000 * 5;
var token = $('[name=__RequestVerificationToken]').val();

var ajax_match = function () {
    var tournamentID = $('#tournamentID').val();
    //console.log(eventID);
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/HiddenList?id=' + tournamentID,
        success: MatchList,
        complete: setTimeout(ajax_match_update, interval),
        error: errorOnAjax
    });
}

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

function ResetMatch(mymatch) {
    console.log(mymatch);
    //if (mymatch["PrereqMatch1ID"] == null && mymatch["PrereqMatch1ID"] == null) {
    //MAKE REQUEST TO START MATCH
    mymatch.__RequestVerificationToken = token;

    $.ajax({
        type: 'POST',
        url: '/Events/ResetMatch/',
        data: (mymatch),
        success: function (response_data_json) { resetSuccess(data) },
        error: errorOnAjax
    });

    alert("Match Reset");
}

function resetSuccess(data)
{
    console.log(data);
}

function MatchList(data) {
    $('#Matches').html(data["matchTable"]);
}

function errorOnAjax() {
    console.log("ERROR in ajax request.");
}

window.setTimeout(ajax_match_update, 0);