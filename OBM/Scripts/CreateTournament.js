$(document).ready(function () {
    drawForm();
});





function ProcessReturnedTournament(response)
{
    console.log(response);
    response = JSON.parse(response);
    console.log(response.tournament.full_challonge_url);


    alert(response.tournament.full_challonge_url);

    
    console.log(response.tournament.full_challonge_url);


    $("#linkBox").append('<div  class="row" ><button id="challongeLink" type="button" class="btn btn-secondary">Go to Challonge</button></row>');
    $("#linkBox").click(function () { gotoChallonge(response.tournament.full_challonge_url) });
}

function gotoChallonge(url)
{
    window.location.replace(url);
}



function drawForm() {

    $("#scriptContainer").append('<div class="col-sm">');
    $("#scriptContainer").append('<row><input type="String" class="form-control" id="TournamentName" placeholder="Tournament Name" required="true"></row>');
    $("#scriptContainer").append('<row><input type="String" class="form-control" id="ApiKey" value='+myApiKey+'></row>');
    $("#scriptContainer").append('<row><input type="String" class="form-control" id="Description" placeholder="Description" required="true"></row>');
    $("#scriptContainer").append('<row><input type="String" class="form-control" id="Game" placeholder="Game" required="true"></row>');
    $("#scriptContainer").append('<row><input type="String" class="form-control" id="url" placeholder="URL" required="true"></row>');


    $("#scriptContainer").append('<row><input type="checkbox" class="form-check-input" id="Private" label="Private?" required="true"><label class="form-check-label" for="Private">Is your tournament private?</label></row>');
    $("#scriptContainer").append('<row  class="row" ><button id="submit" type="button" class="btn btn-secondary">Submit to Challonge</button></row>');
    $("#submit").click(request);
    $("#scriptContainer").append('<div id=linkBox></div>');
    $("#scriptContainer").append('</div>');
    

}



function errorOnAjax()
{
    console.log("Error on AJAX.");
}


var request = function () {
    var tournament = {};
    tournament.api_key = $('#ApiKey').val();
    tournament.myURL = $('#url').val();
    tournament.name = $('#TournamentName').val();

    tournament.description = $('#Description').val();
    tournament.subdomain = $('#Sub-Domain').val();
    tournament.game_name = $('#Game').val();
    tournament.private = $("#Private").is(":checked");
    tournament.event_id = $("#eventID").val();
    data = JSON.stringify(tournament, null, '\t');
    console.log(tournament);
    console.log(JSON.stringify(tournament));

    ////$.post('https://api.challonge.com/v1/tournaments.json', tournament, "JSON");


    $.ajax({
        type: 'POST',
        data: (tournament),
        //contentType: "application/json; charset=utf-8",
        url: 'ChallongeCreate',
        //success: function (response) { ProcessReturnedTournament(response) },
        error: errorOnAjax()
    }).done(function (response) { ProcessReturnedTournament(response) });



}