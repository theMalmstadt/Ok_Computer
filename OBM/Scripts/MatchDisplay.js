function sharedFunction(id) {
    console.log("hello " + id);
    var state = $('#busyState' + id).val();

    if (state == "b") {
        $("#busyState").toggleClass("btn-outline-danger btn-outline-success");
        $("#busyState").val("a");
        $("#busyState").text("a");
    }
    else {
        $("#busyState").toggleClass("btn-outline-success btn-outline-danger");
        $("#busyState").val("b");
        $("#busyState").text("b");
    }

    $.ajax({
        type: 'POST',
        url: '/Events/Competitor/' + id,
        success: ajax_match,
        error: errorOnAjax
    });
}

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

function StartMatch(mymatch)
{
    console.log(mymatch);
    //if (mymatch["PrereqMatch1ID"] == null && mymatch["PrereqMatch1ID"] == null) {
        //MAKE REQUEST TO START MATCH

        $.ajax({
            type: 'POST',
            url: '/Events/StartMatch/',
            data: (mymatch),
            success: ajax_call ,
            error: errorOnAjax
        });
    
    alert("Match Started");
//ajax_call;
    
}

function SubmitScore(mymatch)
{
    var score1;
    score1 = parseInt(prompt("competitor 1 score:", "0"));

    var score2;
    score2 = parseInt(prompt("competitor 2 score:", "0"));

    console.log(mymatch);

    if (Number.isInteger(score1) && Number.isInteger(score2))
    {
        console.log("Submitted Scores: " + score1 + " " + score2);
        mymatch.scoreCsv = score1 + "-" + score2;
        mymatch.score1 = score1;
        mymatch.score2 = score2;

        console.log(mymatch.scoreCsv);

        
        PostScore(mymatch);
    }
    else
        alert("Please Enter a number for each score.");
}




function ValidateScore(score)
{
    if (score.isInteger)
        return true;
    else return false;

}

function PostScore(mymatch)
{
    $.ajax({
        type: 'POST',
        url: '/Events/SubmitScore/',
        data: (mymatch),
        success: ajax_call,
        error: errorOnAjax
    });
}

