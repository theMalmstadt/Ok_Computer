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

    $("#scriptContainer").append('<div class="col">');
    $("#scriptContainer").append('<div class="row"><input type="String" class="form-control" id="TournamentName" placeholder="Tournament Name" required="true"></div>');
    $("#scriptContainer").append('<div class="row"><input type="String" class="form-control" id="Description" placeholder="Description" required="true"></div>');
    $("#scriptContainer").append('<div class="row"><input type="String" class="form-control" id="Game" placeholder="Game" required="true"></div>');
    $("#scriptContainer").append('<div class="row"><input type="String" class="form-control" id="url" placeholder="URL" required="true"></div>');

    $("#scriptContainer").append("<div class='row'><select id='RankedBy' class='form-control'> <option value='match wins'>Match wins</option>  <option value='game wins'>Game wins</option>  <option value='points scored'>Points scored</option>    <option value= 'points difference'>Points difference</option>   <option value= 'custom'>custom</option></select></div>");
    $("#scriptContainer").append('<div class="row">Points for Bye:<input type="Integer" class="form-control" id="pointsForBye" placeholder="Points for Bye" required="true"></div>');
    $("#scriptContainer").append('<div class="row">Signup Cap<input type="Integer" class="form-control" id="signupCap" placeholder="Sign-up cap" required="true"></div>');

    $("#scriptContainer").append("<div class='row'>Start date and time:<input class='form-control' id='dateTime' type='text' ></div>");
    var myDatepicker = $('#dateTime').datetimepicker
        ({
            ampm: true, // FOR AM/PM FORMAT
            use12Hours :  true,
            format: 'd/M/Y H:m:s'        });
  
    $("#scriptContainer").append('<div class="row">Check-in Duration<input type="Integer" class="form-control" id="checkinDuration" placeholder="Minutes" required="true"></div>');

    $("#scriptContainer").append('<div  class="row"><input type="checkbox" class="form-check-input" id="Private" label="Private?" required="true"><label class="form-check-label" for="Private">Is your tournament private?</label>  </div>');
    $("#scriptContainer").append('<div  class="row"><input type="checkbox" class="form-check-input" id="OpenSignUp" label="OpenSignUp?" required="true"><label class="form-check-label" for="OpenSignUp">Open signup?</label></div>');



    $("#scriptContainer").append('<div  class="row" ><button id="submit" type="button" class="btn btn-secondary">Submit to Challonge</button></div>');
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
    tournament.myURL = $('#url').val();
    tournament.name = $('#TournamentName').val();
    tournament.start_at = $("#dateTime").val();

    tournament.ranked_by = $("#RankedBy").val();
    tournament.open_signup = $("#OpenSignUp").is(":checked");
    tournament.pts_for_bye = $("#pointsForBye").val()
    tournament.signup_cap = $("#signupCap").val();
    tournament.check_in_duration = $("#checkinDuration").val();




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