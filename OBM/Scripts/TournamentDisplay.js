$(function () {
    $("#tabs-1").tabs({
        activate: function (event, ui) {
            var ls = [];
            var $variable = $('.ui-selected').each(function () {
                ls.push($(this).text())
            });
            updateSortable(ls);
        }
    });
});

$("#selectable-rank").bind("mousedown", function (e) {
    e.metaKey = true;
}).selectable();

function updateSortable(ls) {
    var temp = rankList.concat(ls);
    rankList = [...new Set(temp)];
    rankList = rankList.filter(value => ls.includes(value));
    //console.log(rankList);
    $("#sortable-1").empty();
    for (var i = 0; i < rankList.length; i++) {
        $("#sortable-1").append("<li id=\"" + rankList[i] + "\">" + rankList[i] + "</li>");
    }
}

var rankList = [];

$("#sortable-1").sortable({
    update: function (event, ui) {
        var data = $(this).sortable('toArray');
        //console.log(data);
        rankList = data;
    }
});

var groupCount = 0;

function addGroup() {
    groupCount++;
    $("#groups").append("<div class=\"row card bg-light\"><ul id=\"sortable-2\" name=\"group-" + groupCount + "\" class=\"ui-sortable sortable\"><li class=\"hide\">bottom</li></ul ></div >");
    var str = 'ul[name="group-' + groupCount + '"]';
    $(function () {
        var oldList, newList, item;
        $(".sortable").sortable({
            start: function (event, ui) {
                item = ui.item;
                newList = oldList = ui.item.parent().parent();
            },
            change: function (event, ui) {
                if (ui.sender) newList = ui.placeholder.parent().parent();
            },
            receive: function (event, ui) {
                $(ui.item).appendTo(this);
            },
            connectWith: ".sortable",
            items: "li:not(.hide)"
        }).disableSelection();
    });
}

function saveSeed() {
    // PUT A DELAY ON THIS BUTTON SO IT CANNOT BE SPAMMED
    var id = $('#TournamentID').val();

    var allGroups = [];
    for (var i = 1; i <= groupCount; i++) {
        var currentGroup = []
        var name = 'ul[name="group-' + i + '"]'
        var html = document.querySelectorAll(name);
        html = html[0].childNodes;
        for (var j = 1; j < html.length; j++) {
            currentGroup.push(html[j].innerHTML);
        }
        allGroups.push(currentGroup);
    }

    var data = {
        id: id,
        method: "trad",//change this to button response
        ranks: rankList,
        groups: allGroups
    };

    $.ajax({
        type: 'POST',
        url: '/Events/Seed?json=' + JSON.stringify(data),
        dataType: "json",
        success: console.log("seed sent to Challonge"),
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

function lineage(data, current, currY, childY, e, compList) {
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
        lineDash = [3, 3];
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
        lineDash: lineDash,
        dataList: dataList,
        latest: latest
    };
}

function recursiveCall(data, current, currY, childY, e, next, compList) {
    var dataList = [];
    var nodeLineColor = 'red';
    var lineDash = [1, 0];

    var get = lineage(data, current, currY, childY, e, compList);
    dataList = dataList.concat(get.dataList);
    nodeLineColor = get.nodeLineColor;
    lineDash = get.lineDash;
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
    var id = $('#TournamentID').val();
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
    if (largestRound < 1) {
        largestRound = 1;
    }

    var ctx = document.getElementById('myChart');
    ctx.height = 100 * (largestRound + 1) ;
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
window.onload = addGroup();
window.onload = addGroup();