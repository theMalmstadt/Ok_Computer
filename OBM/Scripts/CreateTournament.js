function ProcessReturnedTournament(data)
    {
        console.log(data);
    }



function drawForm() {
    $("#scriptContainer").append('<div class="col-sm" required="true">')
    $("#scriptContainer").append('<row><input type="String" class="form-control" id="TournamentName" placeholder="Tournament Name" required="true"></row>');
    $("#scriptContainer").append('<row><input type="String" class="form-control" id="ApiKey" value='+myApiKey+'></row>');
    $("#scriptContainer").append('<row><input type="SubDomain" class="form-control" id="SubDomain" placeholder="Sub-Domain" required="true"></row>');
    $("#scriptContainer").append('<row><input type="String" class="form-control" id="Description" placeholder="Description" required="true"></row>');
    $("#scriptContainer").append('<row><input type="String" class="form-control" id="Game" placeholder="Game" required="true"></row>');
    $("#scriptContainer").append('<row><input type="checkbox" class="form-check-input" id="Private" label="Private?" required="true"><label class="form-check-label" for="Private">Is your tournament private?</label></row>');
    $("#scriptContainer").append('<div id="submit" class="row" ><button type="button" class="btn btn-secondary">Submit to Challonge</button></row>');
    $("#scriptContainer").append('</div>');
    $("#submit").click(sendRequest());
}

drawForm();

function errorOnAjax()
{
    console.log("Error on AJAX.");
}

var ajax_call = function () {
    var name = $('#TournamentName').val();
    var description = $('#Description').val();
    var api_key = myApiKey;
    var subdomain = $('#Sub-Domain').val();
    var game = $('#Game').val();
    var private =('#Private');

    console.log(eventID);
    $.ajax({
        type: 'POST',
        data:data,
        dataType: 'json',
        url: 'https://api.challonge.com/v1/tournaments/create?' + api_key + '&' + name + '&' + description + '&' + subdomain + '&' + game + '&' + private,
        success: ProcessReturnedTournament(data),
        complete: console.log("asdasd"),
        error: errorOnAjax()
    });
}



function sendRequest()
{
    var name = $('#TournamentName').val();
    var description = $('#Description').val();
    var api_key = myApiKey;
    var subdomain = $('#Sub-Domain').val();
    var game = $('#Game').val();
    var private = $('#Private').val();
       
    $.post('https://api.challonge.com/v1/tournaments?', { name, description, api_key, subdomain, game, private }, JSON);
}

