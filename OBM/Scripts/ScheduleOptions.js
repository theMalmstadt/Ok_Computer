$('input.timepicker').each(function () {
    $(this).timepicker({
        uiLibrary: 'bootstrap4'
    })
});

function breakSlider(n) {
    $(function () {
        var dummy = new Date();
        dummy.setMinutes(-1);
        console.log(dummy);
        dummy.setMinutes(1);
        console.log(dummy.getMinutes());
        $("#slider-range").slider({
            range: true,
            min: dummy.setHours(0),
            max: dummy.setHours(23),
            step: 15,
            values: [dummy.setHours(12), dummy.setHours(13)],
            slide: function (event, ui) {
                var l = formatAMPM(new Date(ui.values[0]));//.getHours()) + ":" + formatAMPM(new Date(ui.values[0]).getMinutes());
                var r = formatAMPM(new Date(ui.values[1]));//.getHours()) + ":" + formatAMPM(new Date(ui.values[1]).getMinutes());
                $("#amount").val(l + " - " + r);
            }
        });
    });
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

window.onload = breakSlider(1);