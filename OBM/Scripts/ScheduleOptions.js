var data = [];

$('input.timepicker').each(function () {
    var tourn = {
        tournID : this.getAttribute('tournid'),
        tournName: this.name,
        startTime: 0,
        matchTime: 10,
        stations: 1
    }
    data[data.length] = tourn;
    $(this).timepicker({
        uiLibrary: 'bootstrap4'
    })
});

function breakSlider(n) {
    $(function () {
        var dummy = new Date();
        dummy.setMinutes(0);
        $("#slider-range").slider({
            range: true,
            min: dummy.setHours(0),
            max: dummy.setHours(23),
            step: 15,
            values: [dummy.setHours(12), dummy.setHours(13)],
            slide: function (event, ui) {
                data.breakStart = formatAMPM(new Date(ui.values[0]));//.getHours()) + ":" + formatAMPM(new Date(ui.values[0]).getMinutes());
                data.breakStop = formatAMPM(new Date(ui.values[1]));//.getHours()) + ":" + formatAMPM(new Date(ui.values[1]).getMinutes());
                $("#break-1-value").val(data.breakStart + " - " + data.breakStop);
            }
        });
    });
    var dummy = new Date();
    dummy.setMinutes(0);
    $("#break-1-value").val(formatAMPM(new Date(dummy.setHours(0))) + " - " + formatAMPM(new Date(dummy.setHours(23))));
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
    for (var i = 0; i < data.length; i++) {
        data[i].startTime = document.getElementById(data[i].tournID + "-timePicker").value;
        data[i].matchTime = Number(document.getElementById(data[i].tournID + "-matchTime").value);
        data[i].stations = Number(document.getElementById(data[i].tournID + "-stations").value);
    }
    console.log(data);
}

window.onload = breakSlider(1);