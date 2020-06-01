var token = $('[name=__RequestVerificationToken]').val();

function sharedFunction(id) {
    var state = $('#busyState-' + id).val();

    if (state == "b") {
        $("#busyState-" + id).toggleClass("btn-outline-danger btn-outline-success");
        $("#busyState-" + id).val("a");
        $("#busyState-" + id).text("a");
    }
    else {
        $("#busyState-" + id).toggleClass("btn-outline-success btn-outline-danger");
        $("#busyState-" + id).val("b");
        $("#busyState-" + id).text("b");
    }

    $.ajax({
        type: 'POST',
        url: '/Events/Competitor/' + id,
        data: { __RequestVerificationToken: token },
        success: ajax_match,
        error: errorOnAjax
    });
}

var interval = 1000 * 100000;

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

function StartTournament(id)
{
    $.ajax({
        type: 'POST',
        dataType: 'json',
        url: '/Events/StartTournament?id=' + id,
        data: { __RequestVerificationToken: token },
        success: console.log("it even went good"),
        complete: console.log("TOURNAMENT START COMPLETED"),
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

function download(filename, text) {
    var element = document.createElement('a');
    element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
    element.setAttribute('download', filename);

    element.style.display = 'none';
    document.body.appendChild(element);

    element.click();

    document.body.removeChild(element);
}

// Start file download.
function updateAll(){
    // Generate download of hello.txt file with some content
    var text = document.getElementById("round").value;
    var filename = "round.txt";
    download(filename, text);

    text = document.getElementById("player1").value;
    filename = "player1.txt";
    download(filename, text);

    text = document.getElementById("score1").value;
    filename = "score1.txt";
    download(filename, text);

    text = document.getElementById("player2").value;
    filename = "player2.txt";
    download(filename, text);

    text = document.getElementById("score2").value;
    filename = "score2.txt";
    download(filename, text);
}

function toggleStream() {
    var x = document.getElementById("streamForm");
    if (x.style.display === "none") {
        x.style.display = "block";
    } else {
        x.style.display = "none";
    }
}

function StreamMatch(round, player1, player2, mymatch) {
    var field = document.getElementById("round");
    field.value = round;

    field = document.getElementById("player1");
    field.value = player1;

    field = document.getElementById("score1");
    field.value = 0;

    field = document.getElementById("player2");
    field.value = player2;

    field = document.getElementById("score2");
    field.value = 0;

    field = document.getElementById("matchSerial");
    field.value = mymatch;

    updateAll();
}



function ResetMatch(mymatch) {
    console.log(mymatch);
    //if (mymatch["PrereqMatch1ID"] == null && mymatch["PrereqMatch1ID"] == null) {
    //MAKE REQUEST TO START MATCH
    mymatch.__RequestVerificationToken = token;
    $.ajax({
        type: 'POST',
        url: '/Events/ResetMatch/',
        data: (mymatch),
        success: ajax_call,
        error: errorOnAjax
    });

    alert("Match Reset");
}



function StartMatch(mymatch)
{
    mymatch.__RequestVerificationToken = token;
    console.log(mymatch);
    //if (mymatch["PrereqMatch1ID"] == null && mymatch["PrereqMatch1ID"] == null) {
        //MAKE REQUEST TO START MATCH

        $.ajax({
            type: 'POST',
            url: '/Events/StartMatch/',
            data: mymatch,
            success: ajax_call,
            error: errorOnAjax
        });
    
    alert("Match Started");
//ajax_call;
    
}

function SubmitScore(mymatch)
{
    var score1;
    score1 = parseInt(prompt("competitor 1 score:", "0"));

    var score2;
    score2 = parseInt(prompt("competitor 2 score:", "0"));

    console.log(mymatch);

    if (Number.isInteger(score1) && Number.isInteger(score2))
    {
        console.log("Submitted Scores: " + score1 + " " + score2);
        mymatch.scoreCsv = score1 + "-" + score2;
        mymatch.score1 = score1;
        mymatch.score2 = score2;

        console.log(mymatch.scoreCsv);

        
        PostScore(mymatch);
    }
    else
        alert("Please Enter a number for each score.");
}

function SubmitStream()
{
    var mymatch = JSON.parse(document.getElementById("matchSerial").value);

    var score1 = parseInt(document.getElementById("score1").value);

    var score2 = parseInt(document.getElementById("score2").value);

    console.log(mymatch);

    if (Number.isInteger(score1) && Number.isInteger(score2)) {
        console.log("Submitted Scores: " + score1 + " " + score2);
        mymatch.scoreCsv = score1 + "-" + score2;
        mymatch.score1 = score1;
        mymatch.score2 = score2;

        console.log(mymatch.scoreCsv);


        PostScore(mymatch);
    }
    else
        alert("Please Enter a number for each score.");
}

function ValidateScore(score)
{
    if (score.isInteger)
        return true;
    else return false;

}

function PostScore(mymatch)
{
    mymatch.__RequestVerificationToken = token;
    $.ajax({
        type: 'POST',
        url: '/Events/SubmitScore/',
        data: (mymatch),
        success: ajax_call,
        error: errorOnAjax
    });
}

var ajaxMatches = function () {
    var id = $('#EventID').val();
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/Matches/' + id.toString(),
        success: drawTree,
        error: errorOnAjax
    });
}

var graphText = "";

function lineage(data, current, currY, childY, e, offset) {
    var compList = [];
    var dataList = [];
    var base = 1;
    var nodeLineColor = 'red';
    var lineDash = [1, 0];

    var latest = moment().add(5, 'minutes').startOf('minute');
    var latest = moment(latest).add(((current.Round - 2) * 15), 'minutes').startOf('minute');
    if (current.Time != null) {
        latest = moment(current.Time).startOf('minute');
    }

    if (current.PrereqMatch1ID != null) {
        var parent = data.find(item => item.MatchID === current.PrereqMatch1ID);
        var parentY = currY + (base / Math.pow(2, e));
        var parentResults = recursiveCall(data, parent, parentY, currY, e + 1, offset, latest);
        dataList = dataList.concat(parentResults.dl);
    }

    if (current.PrereqMatch2ID != null) {
        var parent = data.find(item => item.MatchID === current.PrereqMatch2ID);
        var parentY = currY - (base / Math.pow(2, e));
        var parentResults = recursiveCall(data, parent, parentY, currY, e + 1, offset, latest);
        dataList = dataList.concat(parentResults.dl);
    }

    if (current.Winner == null) {
        if (current.Time == null) {
            nodeLineColor = 'gray';
            lineDash = [3, 3];
        }
        else {
            nodeLineColor = '#3895d3';
        }
    }
    else {
        nodeLineColor = '#03c04a';
    }

    var matchName = "";
    if (current.Competitor1Name != "" && current.Competitor2Name != "") {
        matchName = current.Competitor1Name + " vs " + current.Competitor2Name;
    }
    else if (current.Competitor1Name != "") {
        matchName = current.Competitor1Name + " vs --";
    }
    else if (current.Competitor2Name != "") {
        matchName = current.Competitor2Name + " vs --";
    }
    else {
        matchName = "Round " + current.Round + " Match";
    }

    if (compList.includes(current.Competitor1Name) || compList.includes(current.Competitor1Name)) {
        if (current.Time == latest) {
            nodeLineColor = 'yellow';
        }
    }

    var node = [{
        data: [{
            x: latest,
            y: currY + offset
        }],
        fill: false,
        title: matchName,
        label: latest.format('LT') + " - " + current.TournamentName,
        backgroundColor: nodeLineColor,
        pointHoverBackgroundColor: nodeLineColor,
        pointHoverRadius: 30
    }];

    graphText += "(" + node[0].title + " at " + node[0].label + ") ";

    return {
        node: node,
        nodeLineColor: nodeLineColor,
        lineDash: lineDash,
        dataList: dataList,
        latest: latest
    };
}

function recursiveCall(data, current, currY, childY, e, offset, next) {
    var dataList = [];
    var nodeLineColor = 'red';
    var lineDash = [1, 0];

    var get = lineage(data, current, currY, childY, e, offset);
    dataList = dataList.concat(get.dataList);
    nodeLineColor = get.nodeLineColor;
    lineDash = get.lineDash;
    var node = get.node;
    latest = get.latest;

    var line = {
        data: [{
            x: latest,
            y: currY + offset
        },{
            x: next,
            y: childY + offset
        }],
        fill: false,
        radius: -1,
        pointRadius: [-1],
        backgroundColor: nodeLineColor,
        borderWidth: 5,
        borderColor: nodeLineColor,
        borderDash: lineDash
    };

    node.push(line);
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

        var get = lineage(data, current, base, base, base, offset);
        dataList = dataList.concat(get.dataList);
        nodeLineColor = get.nodeLineColor;
        var node = get.node;

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
    
    var ctx = document.getElementById('EventGraph');
    ctx.height = 100 * trees.length * largestRound;
    var myChart = new Chart(ctx, {
        type: 'line',
        data: { datasets: dataList.reverse() },
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
                display: false,
                /* This legend works but i prefer no legend. Code is ready to implement
                 * true,
                labels: {
                    generateLabels(chart) {
                        return [{
                            text: "completed",
                            fillStyle: '#03c04a',
                            }, {
                                text: "ready",
                                fillStyle: '#3895d3',
                            }, {
                                text: "unreachable",
                                fillStyle: 'gray',
                            }];
                    }
                }
                */
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
                    distribution: 'series'
                }],
                yAxes: [{
                    display: false
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
                        return data.datasets[tooltipItem[0].datasetIndex].title;
                    },
                    label: function (tooltipItem, data) {
                        return data.datasets[tooltipItem.datasetIndex].label;
                    }
                },
                titleFontSize: 17,
                titleFontColor: '#DCDCDC',
                bodyFontSize: 14,
                bodyFontColor: '#DCDCDC',
                displayColors: false
            }
        }
    });
    $('#EventGraph').html(graphText);
}

window.onload = ajaxMatches;

function hideShow(div) {
    var x = document.getElementById(div);
    if (window.getComputedStyle(x).display === "none") {
        x.style.display = "block";
    }
    else {
        x.style.display = "none";
    }
    ajaxMatches;
}
