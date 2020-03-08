var interval = 1000 * 5;

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

function dispMatches(jsonString) {
    //console.log(jsonString["data"][0]);
    var matchArray = jsonString["data"];
    var id = $('#compID').val();
    $("#matches").empty();
    var arrLength = matchArray.length;
    for (var i = 0; i < arrLength; i++) {
        //console.log(matchArray[i]);
        var cardCol = "";
        if ((id == matchArray[i].Competitor1ID) || (id == matchArray[i].Competitor2ID)) {
            if ((matchArray[i].Score1 == null) || (matchArray[i].Score2 == null)) {
                if ((matchArray[i].Time > new Date()) || (matchArray[i].Time == null)) {
                    cardCol = "bg-primary";
                }
                else {
                    cardCol = "bg-warning";
                }
            }
            else if ((id == matchArray[i].Competitor1ID) && (matchArray[i].Score1 > matchArray[i].Score2)) {
                cardCol = "bg-success";
            }
            else if ((id == matchArray[i].Competitor2ID) && (matchArray[i].Score2 > matchArray[i].Score1)) {
                cardCol = "bg-success";
            }
            else {
                cardCol = "bg-danger";
            }

            //console.log(new Date('2020-03-07 16:20:15.000').getDate());
            //console.log(new Date().getDate());

            $('#matches').append('<div class="card text-white ' + cardCol + '  mb-3"><div class="card-body"><h5>' 
                + matchArray[i].Competitor1ID + " " + matchArray[i].Competitor2ID + '</h5></div></div>');
        }
    }
}

function errorOnAjax() {
    console.log("ERROR in ajax request.");
}

window.setTimeout(ajaxMatches, 0);








