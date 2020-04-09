function sharedFunction(id) {
    //console.log("hello " + id);
    var state = $('#busyState' + id).val();

    if (state == "b") {
        $("#busyState").toggleClass("btn-outline-danger btn-outline-success");
        $("#busyState").val("a");
        $("#busyState").text("a");
    }
    else {
        $("#busyState").toggleClass("btn-outline-success btn-outline-danger");
        $("#busyState").val("b");
        $("#busyState").text("b");
    }

    $.ajax({
        type: 'POST',
        url: '/Events/Competitor/' + id,
        success: ajax_match,
        error: errorOnAjax
    });
}

var interval = 1000 * 5;

var ajax_match_update = function () {
    var eventID = $("#eventID").val();
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/MatchUpdate?id=' + eventID,
        complete: setTimeout(ajax_match, 0),
        error: errorOnAjax
    });
}

var ajax_call = function () {
    var eventID = $('#eventID').val();
    //console.log(eventID);
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/MatchList?id=' + eventID,
        success: MatchList,
        complete: setTimeout(ajax_match_update, 0),
        error: errorOnAjax
    });
}

var ajax_match = function () {
    var eventID = $('#eventID').val();
    //console.log(eventID);
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/CompetitorList?id=' + eventID,
        success: CompetitorList,
        complete: setTimeout(ajax_call, interval),
        error: errorOnAjax
    });
}

function CompetitorList(data) {
    $('#Competitors').html(data["compTable"]);
}

function MatchList(data) {
    $('#Matches').html(data["matchTable"]);
}

function errorOnAjax() {
    console.log("ERROR in ajax request.");
}

window.setTimeout(ajax_call, 0);

var ajaxMatches = function () {
    var id = $('#EventID').val();
    //console.log(id);
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/Matches/' + id.toString(),
        success: drawTree,
        error: errorOnAjax
    });
}

function tryThis(data, parent, h, e) {
    var base = 1;
    var r = parent.Round;
    var dataList = [];
    var branch1 = h + (base / Math.pow(2, e));
    var branch2 = h - (base / Math.pow(2, e));

    var temp = {
        data: [
            {
                x: moment().add((r * 15), 'minutes'),
                y: h
            },
            {
                x: moment().add(((r - 1) * 15), 'minutes'),
                y: branch1
            }
        ],
        fill: false,
        label: "temp"
    };
    dataList.push(temp);

    temp = {
        data: [
            {
                x: moment().add((r * 15), 'minutes'),
                y: h
            },
            {
                x: moment().add(((r - 1) * 15), 'minutes'),
                y: branch2
            }
        ],
        fill: false,
        label: "temp"
    };
    dataList.push(temp);

    if (parent.PrereqMatch1ID != null) {
        var child = data.find(item => item.MatchID === parent.PrereqMatch1ID);
        dataList = dataList.concat(tryThis(data, child, branch1, e + 1));
    }
    if (parent.PrereqMatch2ID != null) {
        var child = data.find(item => item.MatchID === parent.PrereqMatch2ID);
        dataList = dataList.concat(tryThis(data, child, branch2, e + 1));
    }

    return dataList;
}

function drawTree(data) {
    var trees = [{
        round : 2,
        final : data[0]
    }]
    var tournList = [data[0].TournamentID];

    for (var j = 0; j < trees.length; j++) {
        for (var i = 0; i < data.length; i++) {
            if (tournList.includes(data[i].TournamentID)) {
                if (data[i].Round > trees[0].round) {
                    trees[0].round = data[i].Round;
                    trees[0].final = data[i];
                }
            }
            else {
                var newTree = {
                    round: 2,
                    final: data[i]
                }
                tournList.push(data[i].TournamentID);
                trees.push(newTree);
            }
        }
    }

    var dataList = [];
    for (var i = 0; i < trees.length; i++) {
        dataList = dataList.concat(tryThis(data, trees[i].final, 1, 1));
    }
    
    var ctx = document.getElementById('myChart').getContext('2d');
    var myChart = new Chart(ctx, {
        type: 'line',
        data: { datasets: dataList },
        options: {
            responsive: true,
            layout: {
                padding: {
                    top: 5,
                    left: 5,
                    right: 5,
                    bottom: 5
                }
            },
            title: {
                display: false
            },
            legend: {
                display: false
            },
            scales: {
                xAxes: [{
                    type: 'time',
                    display: true,
                    time: {
                        displayFormats: {
                            quarter: 'h:mm a'
                        }
                    }
                }],
                yAxes: [{
                    display: false
                }]
            }
        }
    });
}

window.onload = ajaxMatches;

function hideShow(div) {
    console.log("trying");
    var x = document.getElementById(div);
    if (window.getComputedStyle(x).display === "none") {
        x.style.display = "block";
    }
    else {
        x.style.display = "none";
    }
}