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

function drawNodes() {
    var node = {
        data: [
            {
                x: moment().add((r * 15), 'minutes'),
                    y: h + offset
            }
        ],
        fill: false,
        //label: parent.MatchID,
        pointBackgroundColor: 'red',
        //radius: -1,
        //pointRadius: [-1]
    };
    return node;
}

function tryThis(data, parent, h, e, offset) {
    console.log("count");
    var base = 1;
    var r = parent.Round;
    var dataList = [];
    var branch1 = h + (base / Math.pow(2, e)) + offset;
    var branch2 = h - (base / Math.pow(2, e)) + offset;

    if (parent.PrereqMatch1ID != null) {
        var child = data.find(item => item.MatchID === parent.PrereqMatch1ID);
        dataList = dataList.concat(tryThis(data, child, branch1, e + 1, offset));
    }
    if (parent.PrereqMatch2ID != null) {
        var child = data.find(item => item.MatchID === parent.PrereqMatch2ID);
        dataList = dataList.concat(tryThis(data, child, branch2, e + 1, offset));
    }


    var temp = {
        data: [
            {
                x: moment().add((r * 15), 'minutes'),
                y: h + offset
            },
            {
                x: moment().add(((r - 1) * 15), 'minutes'),
                y: branch1
            }
        ],
        fill: false,
        label: parent.TournamentID,
        //pointBackgroundColor: 'red',
        //radius: -1,
        //pointRadius: [-1]
    };
    dataList.push(temp);

    temp = {
        data: [
            {
                x: moment().add((r * 15), 'minutes'),
                y: h + offset
            },
            {
                x: moment().add(((r - 1) * 15), 'minutes'),
                y: branch2
            }
        ],
        fill: false,
        label: parent.TournamentID,
        //radius: -1,
       // pointRadius: [-1]
        //pointBackgroundColor: 'green',
    };
    dataList.push(temp);

    return dataList;
}

function tryThis2(data, current, currY, childY) {
    var dataList = [];
    var base = 1;
    //var branch1 = h + (base / Math.pow(2, e)) + offset;
    //var branch2 = h - (base / Math.pow(2, e)) + offset;

    var latest = moment('2020-4-10 10:00');

    if (current.PrereqMatch1ID != null || current.PrereqMatch2ID != null) {
        latest = latest.add((1 * 15), 'minutes');
    }

    if (current.PrereqMatch1ID != null) {
        var parent = data.find(item => item.MatchID === current.PrereqMatch1ID);
        var parentY = currY + (base / Math.pow(2, current.Round));
        var parentResults = tryThis2(data, parent, parentY, currY);
        dataList = dataList.concat(parentResults.dl);
        //latest = parentResults.last;
    }

    if (current.PrereqMatch2ID != null) {
        var parent = data.find(item => item.MatchID === current.PrereqMatch2ID);
        var parentY = currY - (base / Math.pow(2, current.Round));
        var parentResults = tryThis2(data, parent, parentY, currY);
        dataList = dataList.concat(parentResults.dl);
        //latest = parentResults.last;
    }
    if (current.TournamentID == 3002) {
        //console.log(current.TournamentID, currY, parentY, latest.toLocaleString(), latest.add((1 * 15), 'minutes').toLocaleString());
    }

    var node = [{
        data: [{
            x: latest,//.add((1 * 15), 'minutes'),
            y: currY
        }],
        fill: false,
        label: current.Identifier
    },
    {
        data: [{
            x: latest,//.add((1 * 15), 'minutes'),
            y: currY
        },{
            x: moment(latest).add((15), 'minutes'),
            y: childY
        }],
        fill: false,
        //label: "right" + current.TournamentID
        radius: -1,
        pointRadius: [-1]
        }];

    dataList = dataList.concat(node);

    

    var send = {
        dl: dataList,
        last: latest
    };

    //console.log(send);

    return send;
}

function drawTree(data) {
    /*var trees = [{
        round : 2,
        final : data[0]
    }]
    var tournList = [data[0].TournamentID];

    for (var j = 0; j < trees.length; j++) {
        for (var i = 0; i < data.length; i++) {
            if (tournList.includes(data[i].TournamentID)) {
                if (data[i].Round > trees[j].round) {
                    console.log(data[i].TournamentID);
                    trees[j].round = data[i].Round;
                    trees[j].final = data[i];
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
        console.log(tournList);
    }*/

    var trees = [data[0]];

    for (var j = 0; j < trees.length; j++) {
        for (var i = 0; i < data.length; i++) {
            if (data[i].TournamentID == trees[j].TournamentID) {
                if (data[i].Round > trees[j].Round) {
                    //console.log(data[i].TournamentID);
                    trees[j] = data[i];
                }
            }
            else {
                var flag = 1;
                for (var h = 0; h < trees.length; h++) {
                    //console.log(data[i].TournamentID + "-" + trees[h].TournamentID)
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
    console.log(trees);

    var dataList = [];
    for (var i = 0; i < 1; i++) {//trees.length; i++) {
        //console.log("here");
        //dataList = dataList.concat(tryThis2(data, trees[i], 1, 1, i*trees.length));//maybe use final round size
        dataList = dataList.concat(tryThis2(data, trees[i], 1.5, 1).dl);
        //dataList = dataList.concat(tryThis2(data, trees[i], .5, 1).dl);//maybe use final round size
        console.log("here");
        //console.log(tryThis2(data, trees[i]).dl);

    }
    
    var ctx = document.getElementById('myChart').getContext('2d');
    //ctx.height = 5000;
    var myChart = new Chart(ctx, {
        type: 'line',
        data: { datasets: dataList },
        options: {
            responsive: true,
            maintainAspectRatio: true,
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
                    },
                    /*ticks: {
                        max: moment().add(1, 'hours'),
                        min: moment('2020-4-09 19:00')
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