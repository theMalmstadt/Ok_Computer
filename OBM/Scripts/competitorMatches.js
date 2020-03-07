var interval = 1000 * 5;

var ajaxCompetitors = function () {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/MatchList',
        success: getTournamentList,
        error: errorOnAjax
    });
}
function errorOnAjax() {
    console.log('Error on AJAX return');
}
