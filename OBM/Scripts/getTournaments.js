const months = ["January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
];

var ajaxTournaments = function () {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: 'https://api.challonge.com/v1/tournaments.json?api_key=AHbBLmooY7VFlkmGO7DmMUii8tfHWAkDCy4ubAR3',
        success: getTournamentList,
        complete: setTimeout(ajaxTournaments, 5000),
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
        var name = tour["tournament"].name;
        var date = new Date(tour["tournament"].start_at);
        var dispDate = months[date.getMonth()] + ' ' + date.getDay();
        $('#tourList').append('<p id="' + i + '">' + name + ' - ' + dispDate + '</p>');
    }
}

window.setTimeout(ajaxTournaments, 0);