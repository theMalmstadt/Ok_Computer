var ajaxMatches = function () {
    var id = $('#EventID').val();
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/Matches/' + id.toString(),
        success: loopActivator,
        error: errorOnAjax
    });
}

function lineage(data, current, currY, childY, e, compList) {
    var dataList = [];
    var base = 1;
    var nodeLineColor = 'red';

    var latest = moment().add(5, 'minutes').startOf('minute');
    var latest = moment(latest).add(((current.Round - 2) * 15), 'minutes').startOf('minute');
    if (current.Time != null) {
        latest = moment(current.Time).startOf('minute');
    }

    if (current.PrereqMatch1ID != null) {
        var parent = data.find(item => item.MatchID === current.PrereqMatch1ID);
        var parentY = currY + (base / Math.pow(2, e));
        var parentResults = recursiveCall(data, parent, parentY, currY, e + 1, latest, compList);
        dataList = dataList.concat(parentResults.dl);
    }

    if (current.PrereqMatch2ID != null) {
        var parent = data.find(item => item.MatchID === current.PrereqMatch2ID);
        var parentY = currY - (base / Math.pow(2, e));
        var parentResults = recursiveCall(data, parent, parentY, currY, e + 1, latest, compList);
        dataList = dataList.concat(parentResults.dl);
    }

    if (current.Winner == null) {
        nodeLineColor = 'gray';
    }
    else {
        var winner = compList.find(x => x.compID === current.Winner);
        nodeLineColor = winner.color;
    }

    title = "Round " + current.Round + " Match";
    label = "";

    var comp1 = (current.Competitor1Name) ? current.Competitor1Name : "--";
    var comp2 = (current.Competitor2Name) ? current.Competitor2Name : "--";
    if (comp1 != "--" && comp2 != "--") {
        var score1 = (current.Score1) ? current.Score1 : 0;
        var score2 = (current.Score2) ? current.Score2 : 0;
        title = comp1 + " - " + comp2;
        label = score1 + " - " + score2;
    }

    var node = [{
        data: [{
            x: latest,
            y: currY
        }],
        fill: false,
        title: title,
        label: label,
        backgroundColor: nodeLineColor,
        pointHoverBackgroundColor: nodeLineColor,
        pointHoverRadius: 30
    }];

    return {
        node: node,
        nodeLineColor: nodeLineColor,
        dataList: dataList,
        latest: latest
    };
}

function recursiveCall(data, current, currY, childY, e, next, compList) {
    var dataList = [];
    var nodeLineColor = 'red';

    var get = lineage(data, current, currY, childY, e, compList);
    dataList = dataList.concat(get.dataList);
    nodeLineColor = get.nodeLineColor;
    var node = get.node;
    latest = get.latest;

    var line = {
        data: [{
            x: latest,
            y: currY
        }, {
            x: next,
            y: childY
        }],
        fill: false,
        radius: -1,
        pointRadius: [-1],
        backgroundColor: nodeLineColor,
        borderWidth: 5,
        borderColor: nodeLineColor
    };

    node.push(line);
    dataList = dataList.concat(node);

    var send = {
        dl: dataList,
        last: latest
    };

    return send;
}

function drawTree(data, id) {
    //var id = $('#TournamentID').val();
    //$("#MinimalGraph").append($("<canvas id=\"" + id + "\"></canvas>"));

    var largestRound = 2;
    var endNode = data[0];
    var preciseData = [];
    var compList = [];
    var compIDs = [];

    for (var i = 0; i < data.length; i++) {
        if (data[i].TournamentID == id) {
            if (data[i].Round > endNode.Round) {
                endNode = data[i];
                if (data[i].Round > largestRound) {
                    largestRound = data[i].Round;
                }
            }

            if (!compIDs.includes(data[i].Competitor1ID) && data[i].Competitor1ID != null) {
                var temp = {
                    compID: data[i].Competitor1ID,
                    compName: data[i].Competitor1Name,
                    color: randomColor()
                }
                compList.push(temp);
                compIDs.push(temp.compID);
            }
            if (!compIDs.includes(data[i].Competitor2ID) && data[i].Competitor2ID != null) {
                var temp = {
                    compID: data[i].Competitor2ID,
                    compName: data[i].Competitor2Name,
                    color: randomColor()
                }
                compList.push(temp);
                compIDs.push(temp.compID);
            }

            preciseData.push(data[i]);
        }
    }

    var dataList = [];
    var base = 1;

    var get = lineage(preciseData, endNode, base, base, base, compList);
    dataList = dataList.concat(get.dataList);
    nodeLineColor = get.nodeLineColor;
    var node = get.node;

    dataList = dataList.concat(node);

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

    var ctx = document.getElementById(id.toString());
    ctx.height = 100 * largestRound;
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
                display: true,
                labels: {
                    generateLabels(chart) {
                        var labelList = []
                        for (var i = 0; i < compList.length; i++) {
                            var temp = {
                                text: compList[i].compName,
                                fillStyle: compList[i].color,
                            }
                            labelList.push(temp);
                        }
                        return labelList;
                    }
                }
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
                bodyFontSize: 21,
                bodyFontColor: '#DCDCDC',
                displayColors: false,
                titleAlign: 'center',
                bodyAlign: 'center'
            }
        }
    });
}

function loopActivator(data) {
    var trees = [data[0].TournamentID];
    for (var j = 0; j < data.length; j++) {
        if (!trees.includes(data[j].TournamentID)) {
            trees.push(data[j].TournamentID);
        }
    }
    console.log(trees);
    for (var i = 0; i < trees.length; i++) {
        drawTree(data, trees[i]);
    }
}

function errorOnAjax(data) {
    console.log("ERROR in ajax request.");
}

window.onload = ajaxMatches;