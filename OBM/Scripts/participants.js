$(document).ready(function () {
    $("select").change(function () {
        $(this).find("option:selected").each(function () {
            var optionvalue = $(this).attr("value");
            if (optionvalue) {
                $(".add").not("." + optionvalue).hide();
                $("." + optionvalue).show();
            }
            else {
                $(".add").hide();
            }
        });
    }).change();

    $('#CSV').on('change', function () {                    // CITATION https://www.javascripture.com/FileReader

        var file = $('#CSV').prop('files')[0];

        const reader = new FileReader();

        reader.addEventListener("load", function () {
            // convert image file to base64 string
            var result = reader.result;
            console.log(result);
            drawCSV(result);

        }, false);

        if (file) {
            var result=reader.readAsText(file);
            console.log(result);

        }


    });
});

    
    
function drawCSV(result)
{
    $("#participantsParsedList").empty();
    var resultarray = result.split("\n");
    console.log(resultarray);
    var requestString;
    $(resultarray).each(function (index) {
        $("#participantsParsedList").append("<tr><td>" + resultarray[index] + "</tr></td>");
        requestString = requestString + (resultarray[index] + ',');

    });
    requestString.slice(0, -1)
    $("#bulkadd").val(requestString);
    
}

