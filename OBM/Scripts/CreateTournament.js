$(document).ready(function () {
    drawForm();
});





function ProcessReturnedTournament(response)
{
    console.log("PISSSSSSS");
    console.log(response);
    }



function drawForm() {
    $("#scriptContainer").append('<div class="col-sm" required="true">')
    $("#scriptContainer").append('<row><input type="String" class="form-control" id="TournamentName" placeholder="Tournament Name" required="true"></row>');
    $("#scriptContainer").append('<row><input type="String" class="form-control" id="ApiKey" value='+myApiKey+'></row>');
    $("#scriptContainer").append('<row><input type="SubDomain" class="form-control" id="SubDomain" placeholder="Sub-Domain" required="true"></row>');
    $("#scriptContainer").append('<row><input type="String" class="form-control" id="Description" placeholder="Description" required="true"></row>');
    $("#scriptContainer").append('<row><input type="String" class="form-control" id="Game" placeholder="Game" required="true"></row>');
    $("#scriptContainer").append('<row><input type="String" class="form-control" id="url" placeholder="URL" required="true"></row>');

    $("#scriptContainer").append('<row><input type="checkbox" class="form-check-input" id="Private" label="Private?" required="true"><label class="form-check-label" for="Private">Is your tournament private?</label></row>');
    $("#scriptContainer").append('<div  class="row" ><button id="submit" type="button" class="btn btn-secondary">Submit to Challonge</button></row>');
    $("#scriptContainer").append('</div>');
    $("#submit").click(request);
}



function errorOnAjax()
{
    console.log("Error on AJAX.");
}


var request = function () {
    var tournament = {};
    tournament.api_key = myApiKey;
    tournament.myURL = $('#url').val();
    tournament.name = $('#TournamentName').val();

    //  tournament.description = $('#Description').val();
    //tournament.subdomain = $('#Sub-Domain').val();
    //tournament.game_name = $('#Game').val();
   // tournament.private = false;

    data = JSON.stringify(tournament, null, '\t');
    console.log(tournament);
    console.log(JSON.stringify(tournament));

    ////$.post('https://api.challonge.com/v1/tournaments.json', tournament, "JSON");


    $.ajax({
        type: 'POST',
        data: (tournament),
        //contentType: "application/json; charset=utf-8",
        url: 'ChallongeCreate',
        success: function (response) { ProcessReturnedTournament(response) },
        error: errorOnAjax()
    }).done(function (response) { ProcessReturnedTournament(response) });



}