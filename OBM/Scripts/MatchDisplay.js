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

function tryThis2(data, current, currY, childY, e, offset) {
    var dataList = [];
    var base = 1;
    var latest = moment().add(5, 'minutes');
    var latest = moment(latest).add(((current.Round - 1) * 15), 'minutes');
    var nodeLineColor = 'red';

    if (current.PrereqMatch1ID != null) {
        var parent = data.find(item => item.MatchID === current.PrereqMatch1ID);
        var parentY = currY + (base / Math.pow(2, e));
        var parentResults = tryThis2(data, parent, parentY, currY, e + 1, offset);
        dataList = dataList.concat(parentResults.dl);
    }

    if (current.PrereqMatch2ID != null) {
        var parent = data.find(item => item.MatchID === current.PrereqMatch2ID);
        var parentY = currY - (base / Math.pow(2, e));
        var parentResults = tryThis2(data, parent, parentY, currY, e + 1, offset);
        dataList = dataList.concat(parentResults.dl);
    }

    if (current.winner == null) {
        nodeLineColor = 'gray';
    }

    //if ()

    var node = [{
        data: [{
            x: latest,
            y: currY + offset
        }],
        fill: false,
        label: latest.toLocaleString(),
        yLabel: "test",
        backgroundColor: nodeLineColor,
        pointHoverBackgroundColor: nodeLineColor,
        pointHoverRadius: 30
    },
    {
        data: [{
            x: latest,
            y: currY + offset
        },{
            x: moment(latest).add((15), 'minutes'),
            y: childY + offset
        }],
        fill: false,
        radius: -1,
        pointRadius: [-1],
        backgroundColor: nodeLineColor,
        borderWidth: 5,
        borderColor: nodeLineColor
        }];

    dataList = dataList.concat(node);

    var send = {
        dl: dataList,
        last: latest
    };

    return send;
}

function drawTree(data) {

    var trees = [data[0]];
    var largestRound = 2;

    for (var j = 0; j < trees.length; j++) {
        for (var i = 0; i < data.length; i++) {
            if (data[i].TournamentID == trees[j].TournamentID) {
                if (data[i].Round > trees[j].Round) {
                    trees[j] = data[i];
                    if (data[i].Round > largestRound) {
                        largestRound = data[i].Round;
                    }
                }
            }
            else {
                var flag = 1;
                for (var h = 0; h < trees.length; h++) {
                    if (data[i].TournamentID == trees[h].TournamentID) {
                        flag = 0;
                        h = trees.length;
                    }
                }
                if (flag == 1) {
                    trees.push(data[i]);
                }
            }
        }
    }

    var dataList = [];
    for (var i = 0; i < trees.length; i++) {

        var current = trees[i];
        var base = 1;
        var offset = i * (trees[i].Round - 1);

        var latest = moment();

        if (current.PrereqMatch1ID != null) {
            var parent = data.find(item => item.MatchID === current.PrereqMatch1ID);
            var parentY = base + (base / Math.pow(2, base));
            var parentResults = tryThis2(data, parent, parentY, base, (base + 1), offset);
            dataList = dataList.concat(parentResults.dl);
            latest = parentResults.last;
        }

        if (current.PrereqMatch2ID != null) {
            var parent = data.find(item => item.MatchID === current.PrereqMatch2ID);
            var parentY = base - (base / Math.pow(2, base));
            var parentResults = tryThis2(data, parent, parentY, base, (base + 1), offset);
            dataList = dataList.concat(parentResults.dl);
            latest = parentResults.last;
        }

        var node = [{
            data: [{
                x: moment(latest).add((1 * 15), 'minutes'),
                y: base + offset
            }],
            fill: false,
            label: current.Identifier
        }];

        dataList = dataList.concat(node);
    }

    var hidden = [{
        data: [{
            x: moment(),
            y: 0
        }],
        fill: false,
        radius: -1,
        pointRadius: [-1]
    }];

    dataList = dataList.concat(hidden);

    largestRound = largestRound - 2;
    if (largestRound < 0) {
        largestRound = 0;
    }
    
    var ctx = document.getElementById('myChart');
    ctx.height = 100 * trees.length * largestRound;
    var myChart = new Chart(ctx, {
        type: 'line',
        data: { datasets: dataList },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            layout: {
                padding: {
                    top: 50,
                    left: 50,
                    right: 50,
                    bottom: 50
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
                        },
                        unit: 'minute',
                        unitStepSize: 5
                    },
                    /*ticks: {
                        //max: moment().add(1, 'hours'),
                        //min: moment('2020-4-09 19:00')
                    }*/
                }],
                yAxes: [{
                    display: false/*,
                    ticks: {
                        max: 40,
                        min: 0,
                        stepSize: 0.1
                    }*/
                }]
            },
            elements: {
                point: {
                    radius: 15
                }
            },
            hover: {
                mode: 'nearest'
            },
            tooltips: {
                callbacks: {
                    title: function (tooltipItem, data) {
                        return "test";
                    }
                    /*,
                    label: function (tooltipItem, data) {
                        return;
                    },
                    afterLabel: function (tooltipItem, data) {
                        var dataset = data['datasets'][0];
                        var percent = Math.round((dataset['data'][tooltipItem['index']] / dataset["_meta"][0]['total']) * 100)
                        return '(' + percent + '%)';
                    }*/
                },
                backgroundColor: '#FFF',
                titleFontSize: 16,
                titleFontColor: '#0066ff',
                bodyFontColor: '#000',
                bodyFontSize: 14,
                displayColors: false
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