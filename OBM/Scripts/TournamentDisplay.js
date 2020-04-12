/*var Colors = {
    aqua: "#00ffff",
    azure: "#f0ffff",
    beige: "#f5f5dc",
    black: "#000000",
    blue: "#0000ff",
    brown: "#a52a2a",
    cyan: "#00ffff",
    darkblue: "#00008b",
    darkcyan: "#008b8b",
    darkgrey: "#a9a9a9",
    darkgreen: "#006400",
    darkkhaki: "#bdb76b",
    darkmagenta: "#8b008b",
    darkolivegreen: "#556b2f",
    darkorange: "#ff8c00",
    darkorchid: "#9932cc",
    darkred: "#8b0000",
    darksalmon: "#e9967a",
    darkviolet: "#9400d3",
    fuchsia: "#ff00ff",
    gold: "#ffd700",
    green: "#008000",
    indigo: "#4b0082",
    khaki: "#f0e68c",
    lightblue: "#add8e6",
    lightcyan: "#e0ffff",
    lightgreen: "#90ee90",
    lightgrey: "#d3d3d3",
    lightpink: "#ffb6c1",
    lightyellow: "#ffffe0",
    lime: "#00ff00",
    magenta: "#ff00ff",
    maroon: "#800000",
    navy: "#000080",
    olive: "#808000",
    orange: "#ffa500",
    pink: "#ffc0cb",
    purple: "#800080",
    violet: "#800080",
    red: "#ff0000",
    silver: "#c0c0c0",
    white: "#ffffff",
    yellow: "#ffff00"
};*/

// random number generator
function rand(frm, to) {
    return ~~(Math.random() * (to - frm)) + frm;
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
        if (current.Time == null) {
            nodeLineColor = 'gray';
        }
        else {
            nodeLineColor = '#3895d3';
        }
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

function drawTree(data) {
    var id = $('#TournamentID').val();
    var largestRound = 2;
    var endNode = data[0];
    var preciseData = [];
    var compList = [];

    for (var i = 0; i < data.length; i++) {
        if (data[i].TournamentID == id) {
            if (data[i].Round > endNode.Round) {
                endNode = data[i];
                if (data[i].Round > largestRound) {
                    largestRound = data[i].Round;
                }
            }
            if (!compList.includes(data[i].Competitor1ID) && data[i].Competitor1ID != null) {
                var temp = {
                    compID: data[i].Competitor1ID,
                    color: randomColor()
                }
                compList.push(temp);
            }
            if (!compList.includes(data[i].Competitor2ID) && data[i].Competitor2ID != null) {
                var temp = {
                    compID: data[i].Competitor2ID,
                    color: randomColor()
                }
                compList.push(temp);
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

    var ctx = document.getElementById('myChart');
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

function errorOnAjax(data) {
    console.log("ERROR in ajax request.");
}

window.onload = ajaxMatches;