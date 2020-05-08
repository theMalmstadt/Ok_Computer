function sendSMS(compID, eventID) {
    $.ajax({
        type: "GET",
        dataType: "json",
        url: "/Competitor/SMSContact?compID=" + compID + "&eventID=" + eventID,
        success: SendingResponse,
        error: errorOnAjax
    })
}

function SendingResponse(data) {
    if (data[0]["statusMSG"] == "Competitor Contacted")
        $('td:contains(' + data[0]["contactName"] + ')').append("<p class=\"text-success\">" + data[0]["statusMSG"] + "</p>");
    else {
        $('td:contains(' + data[0]["contactName"] + ')').append("<p class=\"text-danger\">" + data[0]["statusMSG"] + "</p>");
    }
}

function errorOnAjax() {
    console.log("Error in Ajax request.");
}