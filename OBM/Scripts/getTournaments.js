const months = ["January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
];

function ajaxTournaments(api_key) {
    var chal = 'https://api.challonge.com/v1/tournaments.json?api_key=';
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: chal.concat(api_key),
        success: getTournamentList,
        error: errorOnAjax
    });
}

function errorOnAjax() {
    console.log('Error on AJAX return');
}

function getTournamentList(jsonString) {
    console.log('Successfully got list with ' + jsonString.length + ' elements');
    $("#tourList").empty();
    i = 0;
    for (var tour of jsonString) {
        var name = tour["tournament"].name.replace(/</g, "&lt;").replace(/>/g, "&gt;");
        var date = new Date(tour["tournament"].start_at);
        var url = tour["tournament"].url;
        var dispDate = months[date.getMonth()] + ' ' + date.getDate();
        $('#tourList').append('<form method="get"><p id="' + i + '">' + name + ' - ' + dispDate +
            ' - <input class="form-control" id="search" name="search" type="hidden" value="https://challonge.com/' + url +
            '" ><input name="submit" class="button" type="submit" value=Add /></p></form>');
        i++;
    }
}


