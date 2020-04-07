function sharedFunction(id) {
    //console.log("hello " + id);
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

window.onload = ajaxMatches;

var ajaxMatches = function () {
    var id = $('#EventID').val();
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/Matches/' + id,
        success: drawTree,
        error: errorOnAjax
    });
}

function drawTree (data) {
    //$('#MinimalGraph').append("<svg viewBox=\"-450 -450 900 900\" style=\"background-color: slategray;\"><g fill=\"none\" fill-rule=\"evenodd\" stroke=\"currentColor\" stroke-width=\"7\" class=\"lines\"><circle cx=\"-15\" cy=\"0\" r=\"15\" stroke-width=\"3\" /><circle cx=\"-15\" cy=\"0\" r=\"7\" fill=\"currentColor\" /><circle cx=\"-15\" cy=\"100\" r=\"15\" stroke-width=\"3\" /><circle cx=\"-15\" cy=\"100\" r=\"7\" fill=\"currentColor\" /><path d=\"M0,0 C50,0 50,50 100,50\" /><path d=\"M0,100 C50,100 50,50 100,50\" /><circle cx=\"115\" cy=\"50\" r=\"15\" stroke-width=\"3\" /></g></svg>");
    /*
     <svg viewBox="-450 -450 900 900" style="background-color: slategray;">
        <g fill="none" fill-rule="evenodd" stroke="currentColor" stroke-width="7" class="lines">

            <circle cx="-15" cy="0" r="15" stroke-width="3" />
            <circle cx="-15" cy="0" r="7" fill="currentColor" />

            <circle cx="-15" cy="100" r="15" stroke-width="3" />
            <circle cx="-15" cy="100" r="7" fill="currentColor" />

            <path d="M0,0 C50,0 50,50 100,50" />
            <path d="M0,100 C50,100 50,50 100,50" />
            <circle cx="115" cy="50" r="15" stroke-width="3" />

        </g>
    </svg>
    */
    var temp = "<svg viewBox=\"-450 -450 900 900\" style=\"background-color: slategray;\"><g id=\"vBox\" fill=\"none\" fill-rule=\"evenodd\" stroke=\"currentColor\" stroke-width=\"7\" class=\"lines\">";
    temp += "<circle cx=\"-15\" cy=\"0\" r=\"15\" stroke-width=\"3\" />";
    temp += "</g></svg >";
    $('#MinimalGraph').append(temp);
}