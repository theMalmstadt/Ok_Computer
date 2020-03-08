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
    //console.log(matchArray);
    var id = $('#compID').val();
    $("#matches").empty();
    var arrLength = matchArray.length;
    for (var i = 0; i < arrLength; i++) {
        //console.log(matchArray[i]);
        var cardCol = "";
        var status = "";
        if ((id == matchArray[i].Competitor1ID) || (id == matchArray[i].Competitor2ID)) {
            if ((matchArray[i].Score1 == null) || (matchArray[i].Score2 == null)) {
                if ((matchArray[i].Time > new Date()) || (matchArray[i].Time == null)) {
                    cardCol = "text-primary border-primary";
                    status = "Upcoming";
                }
                else {
                    cardCol = "text-warning border-warning";
                    status = "In Progress";
                }
            }
            else if ((id == matchArray[i].Competitor1ID) && (matchArray[i].Score1 > matchArray[i].Score2)) {
                cardCol = "text-success border-success";
                status = "Win";
            }
            else if ((id == matchArray[i].Competitor2ID) && (matchArray[i].Score2 > matchArray[i].Score1)) {
                cardCol = "text-success border-success";
                status = "Win";
            }
            else {
                cardCol = "text-danger border-danger";
                status = "Loss";
            }

            //console.log(new Date('2020-03-07 16:20:15.000').getDate());
            //console.log(new Date().getDate());

            $('#matches').append('<div class="card ' + cardCol + ' mb-3"><div class="card-header bg-dark">'
                + status + '</div><div class="card-body text-white bg-secondary"><h5>' 
                + matchArray[i].Competitor1ID + " " + matchArray[i].Competitor2ID + '</h5></div></div>');
        }
    }
}

function errorOnAjax() {
    console.log("ERROR in ajax request.");
}

window.setTimeout(ajaxMatches, 0);








