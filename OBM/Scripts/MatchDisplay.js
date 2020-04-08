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
    console.log(eventID);
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
    console.log(eventID);
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
    console.log(id);
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/Matches/' + id.toString(),
        success: drawTree,
        error: errorOnAjax
    });
}

function drawTree(data) {
    var here = moment.utc(data[0]["Time"]);
    var test2 = new Date();
    test2.setHours(16);
    test2.setMinutes(30);
    
    console.log(here.toLocaleString())
    console.log(moment.utc(test2).toLocaleString())
    var dataList = [];
    for (var i = 0; i < data.length; i++) {
        var temp = {
            data: [{
                x: here,
                y: 80
            }, {
                x: test2,
                y: 75
            }],
            fill: false
        }
        dataList.push(temp);
        console.log(temp);
    }
    /*
    var here2 = {
        data: [{
            x: here,
            y: 80
        }, {
            x: test2,
            y: 75
        }],
        fill: false
    };*/
    console.log(dataList);
    /*console.log([{
                data: [{
                    x: here,
                    y: 80
                    }, {
                    x: test2,
                    y: 75
                }],
                fill: false
                }, {
                data: [{
                    x: test1,
                    y: 70
                    }, {
                    x: test2,
                    y: 75
                }],
			    fill: false
            
                /*data: [80, 75],
			    fill: false
                }, {
                    data: [70, 75, 65],
                    fill: false
                }, {
                    data: [60, 55],
                    fill: false
                }, {
                    data: [50, 55, 65, 45],
                    fill: false
                }, {
                    data: [40, 35],
                    fill: false
                }, {
                    data: [30, 35, 25, 45],
                    fill: false
                }, {
                    data: [20, 15],
                    fill: false
                }, {
                    data: [10, 15, 25],
                    fill: false
                }]);

    var test1 = new Date();
    test1.setHours(16);
    test1.setMinutes(15);
    console.log(test1.toLocaleString());*/

    var ctx = document.getElementById('myChart').getContext('2d');
    var myChart = new Chart(ctx, {
        type: 'line',
        data: {
            //labels: [test1.toLocaleString(), test2.toLocaleString()],
            datasets: dataList
            
        },
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
