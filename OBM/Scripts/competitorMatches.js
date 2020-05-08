$("#busyState").click(function () {
    var state = $('#busyState').val();
    var id = $('#compID').val();

    if (state == "b") {
        $("#busyState").toggleClass("btn-outline-danger btn-outline-success");
        $("#busyState").val("a");
        $("#busyState").text("available");
    }
    else {
        $("#busyState").toggleClass("btn-outline-success btn-outline-danger");
        $("#busyState").val("b");
        $("#busyState").text("busy");
    }

    $.ajax({
        type: 'POST',
        url: '/Events/Competitor/' + id,
        data: {
            AddAntiForgeryToken({ }),
            id: parseInt($(this).attr("title"))
        },
        error: errorOnAjax
    });
});

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
        var status = "";
        var color = "";
        if ((id == matchArray[i].Competitor1ID) || (id == matchArray[i].Competitor2ID)) {
            if (matchArray[i].Status == 2) {
                status = "Upcoming";
                color = 'primary';
            }
            else if ((matchArray[i].Status == 1)) {
                status = "In Progress";
                color = 'warning';
            }
            else if (matchArray[i].Status == 3) {
                if (id == matchArray[i].Winner) {
                    status = "Win";
                    color = 'success';
                }
                else {
                    status = "Loss";
                    color = 'danger';
                }
            }
            else {
                status = "Upcoming";
                color = 'primary';
            }

            var comp1 = matchArray[i].Competitor1Name;
            var comp2 = matchArray[i].Competitor2Name;
            var score1 = (matchArray[i].Score1 || (matchArray[i].Score1 == 0)) ? matchArray[i].Score1 : '--';
            var score2 = (matchArray[i].Score2 || (matchArray[i].Score2 == 0)) ? matchArray[i].Score2 : '--';
            var round = matchArray[i].Round;
            var temp = new Date(matchArray[i].Time).toLocaleTimeString();
            var time = (temp != "Invalid Date" ? temp : "--");

            $('#matches').append('<div class="card text-white border-' + color + ' mb-3"><div class="card-header bg-' + color
                + '" align="center"><h3>' + status + '</h3></div><div class="card-body text-white bg-secondary"><div class="row">'
                + '<div class="col-sm" align="center"><h6>Competitors</h6><h3>' + comp1 + '</h3><h3>' + comp2
                + '</h3></div><div class="col-sm" align="center">' + '<h6>Score</h6><h3>' + score1 + '</h3><h3>' + score2
                + '</h3></div><div class="col-sm" align="center"><h6>Round</h6><br /><h3>' + round
                + '</h3></div><div class="col-sm" align="center"><h6>Time</h6><br /><h3>' + time + '</h3></div></div></div></div>');
        }
    }
}

function errorOnAjax() {
    console.log("ERROR in ajax request.");
}

window.setTimeout(ajaxMatches, 0);
