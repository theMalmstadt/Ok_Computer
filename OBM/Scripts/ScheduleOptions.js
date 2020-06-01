var token = $('[name=__RequestVerificationToken]').val();
var tourns = [];
var breaks = [];
var breakCount = 0;

$('input.timepicker').each(function () {
    var tourn = {
        tournID : this.getAttribute('tournid'),
        tournName: this.name,
        startTime: 0,
        matchTime: 10,
        stations: 1
    }
    tourns[tourns.length] = tourn;
    $(this).timepicker({
        uiLibrary: 'bootstrap4'
    })
});

function addBreak() {
    $("#breaks").append('<div id="break-div-' + breakCount + '" style="padding: 30px;" class="card"><div class="form-row">' +
        '<div class= "form-group col-md-4" > <label for="break-' + breakCount + '-name">Break Name</label>' +
        '<input id="break-' + breakCount + '-name" type="text" class="border-dark form-control" aria-describedby="breakName" value="Break ' + (breakCount+1) + '" />' +
        '</div><div class="form-group col-md-4"><label for="break-' + breakCount + '-value">Break Period</label>' +
        '<input type="text" id="break-' + breakCount + '-value" readonly style="border:0; color:#0b8a1a; font-weight:bold;"></div>' +
        '<div class="form-group col" align="right"><a onclick="removeBreak(' + breakCount + ')" style="color:red;">Remove</a>' +
        '</div></div><div id="slider-range-' + breakCount +'"></div></div>');
    breakSlider(breakCount);
    breakCount++;
}

function removeBreak(n) {
    $('#break-div-' + n).remove();
    breaks.splice(n, 1);
    //breakCount--;
}

function breakSlider(n) {
    var dummy = new Date();
    dummy.setHours(0);
    dummy.setMinutes(0);
    var sub = new Date();
    sub.setHours(0);
    sub.setMinutes(0);
    $(function () {
        $("#slider-range-" + n).slider({
            range: true,
            min: dummy.setHours(0),
            max: dummy.setHours(23),
            step: 300000,
            values: [dummy.setHours(12), dummy.setHours(13)],
            slide: function (event, ui) {
                $("#break-" + n + "-value").val(formatAMPM(new Date(ui.values[0])) + " - " + formatAMPM(new Date(ui.values[1])));
                breakPeriod = {
                    breakName: document.getElementById("break-" + n + "-name").value,
                    breakStart: (ui.values[0] - sub.getTime()) / 60000,
                    breakStop: (ui.values[1] - sub.getTime()) / 60000
                }
                breaks[n] = breakPeriod;
                //console.log(ui.values[0]);
            }
        });
    });
    $("#break-" + n + "-value").val(formatAMPM(new Date(dummy.setHours(12))) + " - " + formatAMPM(new Date(dummy.setHours(13))));
    breakPeriod = {
        breakName: document.getElementById("break-" + n + "-name").value,
        breakStart: (dummy.setHours(12) - sub.getTime()) / 60000,
        breakStop: (dummy.setHours(13) - sub.getTime()) / 60000
    }
    breaks[n] = breakPeriod;
}

function formatAMPM(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12;
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}

function formatMilliseconds(time) {
    var temp = time.split(':');
    var time = ((60 * 60 * parseInt(temp[0])) + (60 * parseInt(temp[1]))) * 1000;
    return time;
}

function sendData() {
    $('#schedule-table').empty();
    $('#schedule-table').append('<div class="col" align="center"><div class="spinner-border text-secondary" role="status"><span class="sr-only">Loading...</span></div></div>');
    for (var i = 0; i < tourns.length; i++) {
        var tourn = {
            tournID: Number(tourns[i].tournID),
            tournName: tourns[i].tournName,
            startTime: formatMilliseconds(document.getElementById(tourns[i].tournID + "-timePicker").value) / 60000,
            matchTime: Number(document.getElementById(tourns[i].tournID + "-matchTime").value),
            stations: Number(document.getElementById(tourns[i].tournID + "-stations").value)
        }
        tourns[i] = tourn;
    }
    breaks = breaks.filter(Boolean);
    var data = {
        breaks: breaks,
        event: Number(document.getElementById("EventID").value),
        tourns: tourns
    }
    //console.log(data);
    //console.log(JSON.stringify(data));

    $.ajax({
        type: 'POST',
        url: '/Events/GenerateSchedule?json=' + encodeURIComponent(JSON.stringify(data)),
        data: { __RequestVerificationToken: token },
        success: delayToAnimate
    });
}

function delayToAnimate(data) {
    //$('#schedule-table').append('<div class="col" align="center"><div class="spinner-border text-secondary" role="status"><span class="sr-only">Loading...</span></div></div>');
    setTimeout(function () {
        printGraph(data);
    }, 1000);
}

function printGraph(schedule) {
    console.log(schedule);
    var tournIDs = [schedule[0].TournamentID];
    var tournNames = [schedule[0].TournamentName];
    var tournStations = [schedule[0].Station]
    var matchInterval = [schedule[0].MatchInterval];
    var smallestInterval = schedule[0].MatchInterval
    var tournColumns = [[]];
    for (var i = 0; i < schedule.length; i++) {
        if (!tournIDs.includes(schedule[i].TournamentID)) {
            tournIDs.push(schedule[i].TournamentID);
            tournNames.push(schedule[i].TournamentName);
            tournStations.push(schedule[i].Station)
            matchInterval.push(schedule[i].MatchInterval);
            tournColumns.push([]);
        }
        else {
            var j = tournIDs.indexOf(schedule[i].TournamentID);
            var stations = tournStations[j];
            if (stations < schedule[i].Station) {
                tournStations[j] = schedule[i].Station;
            }
        }

        if (schedule[i].MatchInterval < smallestInterval) {
            smallestInterval = schedule[i].MatchInterval;
        }
    }
    //console.log(tournColumns);
    var htmlChunk = '<div class="row" align="center" style="padding-left:50px; padding-right:50px;">';
    var htmlChunkEnd = '</div>';

    for (var i = 0; i < tournIDs.length; i++) {
        var colHeight = (matchInterval[i] / smallestInterval) * 28;
        for (var j = 1; j <= tournStations[i]; j++) {
            htmlChunk += '<div class="col" style="padding:0px;"><table class="col"><tr align="center"><th>' + tournNames[i] + ' Station ' + j + '</th></tr></table></div>';
        }
    }
    htmlChunk += htmlChunkEnd + '<div class="row" align="center" style="padding-left:50px; padding-right:50px;">';

    for (var i = 0; i < tournIDs.length; i++) {
        var colHeight = (matchInterval[i] / smallestInterval) * 56;
        for (var j = 1; j <= tournStations[i]; j++) {
            htmlChunk += '<div class="col" style="padding:0px;"><table class="col"><tr align="center"></tr>';
            var breakCount = [];
            for (var k = 0; k < schedule.length; k++) {
                
                if ((schedule[k].TournamentID == tournIDs[i]) && (schedule[k].Station == j)) {
                    for (var l = 0; l < breaks.length; l++) {
                        if (((schedule[k].StartTime + matchInterval[i]) >= breaks[l].breakStart) && (schedule[k].StartTime <= breaks[l].breakStop) && (!breakCount.includes(tournNames[i] + j + breaks[l].breakName))) {
                            var start = new Date((breaks[l].breakStart + 480) * 60000);
                            var stop = new Date((breaks[l].breakStop + 480) * 60000);
                            var breakHeight = ((breaks[l].breakStop - breaks[l].breakStart) / smallestInterval) * 56;
                            htmlChunk += '<tr><td class="card border-info" align="center" style="height:' + breakHeight + 'px;">' + formatAMPM(start) + ' - ' + formatAMPM(stop) + '<br>' + breaks[l].breakName + '</td></tr>';
                            breakCount.push(tournNames[i] + j + breaks[l].breakName);
                            //htmlChunk += '</table></div></div><div class="row" align="center" style="padding-left:50px; padding-right:50px;"><div class="col card border-info" style="height:' + breakHeight + 'px;">10:00 - Lunch break</div></div>';
                            //htmlChunk += '<div class="row" align="center" style="padding-left:50px; padding-right:50px;"><div class="col" style="padding:0px;"><table class="col"><tr align="center"></tr>';
                        }
                    }

                    var start = new Date((schedule[k].StartTime + 480) * 60000);
                    htmlChunk += '<tr><td class="card border-info" align="center" style="height:' + colHeight + 'px;">' + formatAMPM(start) + ' - Round ' + schedule[k].Round + ' - Match ' + schedule[k].Identifier + '<br>' + schedule[k].Competitor1Name + ' vs ' + schedule[k].Competitor2Name + '</td></tr>';
                }
            }
            htmlChunk += '</table></div>';
        }
    }
    htmlChunk += htmlChunkEnd;

    $('#schedule-table').empty();
    $('#schedule-table').append(htmlChunk);
}

function roundToFive(n) {
    return Math.ceil(n / 5) * 5;
}