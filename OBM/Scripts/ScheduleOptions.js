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
}

function breakSlider(n) {
    var dummy = new Date();
    dummy.setMinutes(0);
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
                    breakStart: ui.values[0],       //formatAMPM(new Date(ui.values[0])),
                    breakStop: ui.values[1]         //formatAMPM(new Date(ui.values[1]))
                }
                breaks[n] = breakPeriod;
            }
        });
    });
    $("#break-" + n + "-value").val(formatAMPM(new Date(dummy.setHours(0))) + " - " + formatAMPM(new Date(dummy.setHours(23))));
    breakPeriod = {
        breakName: document.getElementById("break-" + n + "-name").value,
        breakStart: dummy.setHours(12),
        breakStop: dummy.setHours(13)
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

function sendData() {
    for (var i = 0; i < tourns.length; i++) {
        var tourn = {
            tournID: tourns[i].tournID,
            tournName: tourns[i].tournName,
            startTime: document.getElementById(tourns[i].tournID + "-timePicker").value,
            matchTime: Number(document.getElementById(tourns[i].tournID + "-matchTime").value),
            stations: Number(document.getElementById(tourns[i].tournID + "-stations").value)
        }
        tourns[i] = tourn;
    } 
    var data = {
        breaks: breaks,
        tourns: tourns
    }
    console.log(data);

    $.ajax({
        type: 'POST',
        url: '/Events/GenerateSchedule?json=' + encodeURIComponent(JSON.stringify(data)),
        data: { __RequestVerificationToken: token },
        success: function (response) {
            console.log(response);
        }
    });
}