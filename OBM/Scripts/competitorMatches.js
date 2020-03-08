var interval = 1000 * 30;

var ajaxMatches = function () {
    var id = $('#EventID').val();
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/Matches/' + id,
        success: dispMatches,
        complete: setTimeout(ajaxMatches, interval),
        error: errorOnAjax
    });
}

function dispMatches(matchArray) {
    var id = $('#compID').val();
    $("#matches").empty();
    var arrLength = matchArray.length;
    for (var i = 0; i < arrLength; i++) {
        var cardCol = "";
        var status = "";
        if ((id == matchArray[i].Competitor1ID) || (id == matchArray[i].Competitor2ID)) {
            if ((matchArray[i].Score1 == null) || (matchArray[i].Score2 == null)) {
                if ((matchArray[i].Time > new Date()) || (matchArray[i].Time == null)) {
                    cardCol = "text-primary border-primary";
                    status = "Upcoming";
                    color = 'primary';
                }
                else {
                    cardCol = "text-warning border-warning";
                    status = "In Progress";
                    color = 'warning';
                }
            }
            else if (((id == matchArray[i].Competitor1ID) && (matchArray[i].Score1 > matchArray[i].Score2))
                        || ((id == matchArray[i].Competitor2ID) && (matchArray[i].Score2 > matchArray[i].Score1))) {
                cardCol = "text-success border-success";
                status = "Win";
                color = 'success';
            }
            else {
                cardCol = "text-danger border-danger";
                status = "Loss";
                color = 'danger';
            }

            //console.log(new Date('2020-03-07 16:20:15.000').getDate());
            //console.log(new Date().getDate());
            var score1 = '--';
            var score2 = '--';

            $('#matches').append('<div class="card text-white border-' + color + ' mb-3"><div class="card-header bg-'
                + color + '" align="center"><h3>Test</h3></div><div class="card-body text-white bg-secondary"><div class="row">'
                + '<div class="col-sm" align="center"><h6>Competitors</h6><h3>' + matchArray[i].Competitor1Name
                + '</h3><h3>' + matchArray[i].Competitor2Name + '</h3></div ><div class="col-sm" align="center">'
                + '<h6>Score</h6><h3>' + score1 + '</h3><h3>' + score2 + '</h3></div><div class="col-sm" align="center"><h6>Round</h6><h3>'
                + matchArray[i].Round + '</h3></div><div class="col-sm" align="center"><h6>Time</h6><h3>'
                + matchArray[i].Time + '</h3></div></div></div></div>');
        }
    }
}

function errorOnAjax() {
    console.log("ERROR in ajax request.");
}

window.setTimeout(ajaxMatches, 0);
