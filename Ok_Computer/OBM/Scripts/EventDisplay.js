var ajax_call_events = function () {
    
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/EventList?location='+$('#location_search').val()+"&name="+$('#name_search').val(),
        success: EventList,
        error: errorOnAjax
    });

    $('#next_page').click(function () {
        if (current_page < number_of_pages)
            current_page = current_page + 1;
    });
    $('#previous_page').click(function () {
        if (current_page > 0)
            current_page = current_page - 1;
    });


}


var ajax_call_tournaments = function (ID) {

    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/Events/TournamentList/' + ID,
        success: MoreDetails,
        error: errorOnAjax
    });


}









var location_search_string=null;

var eventlist;
var index_key = new Array();
var current_page=0;
var number_of_pages;
var number_per_page = 10;



function EventList(data) {
    $('#Events').empty();
    console.log(data);
    eventlist=data = JSON.parse(data);

    //console.log(data);
    number_of_pages = data.length / 10;
    //console.log(number_of_pages);
    //console.log(data.length);
    console.log("Current page:" + current_page);
    for (var i = current_page*10; i < current_page*10+10 && i<data.length  ;i++)
    {

        
        //console.log(i);
        $('#Events').append("<tr id=" + i + "><td>" + data[i].EventName + "</td></tr>");
        $('#' + i).click(data[i], EventDetails);
        index_key[i] = data[i].EventID;
        
    }

    
}


function EventDetails(i)
{   
    $('#Selected_Description').empty();
    $('#Selected_Title').html(i.data.EventName);
    $('#Selected_Description').html(i.data.EventDetails);
    $('#Selected_Location').html(i.data.Location);
    ajax_call_tournaments(i.data.EventID);

}

function MoreDetails(data)
{
    $('#Tournament_List').empty();

    data = JSON.parse(data);
   // console.log(data);
    


    for (var i in data) {
        // console.log(i); //i is  the index of a event in the data object and is not a key.
        $('#Tournament_List').append("<tr id=" + i + "><td>" + data[i].TournamentName + "</td></tr>");
        //$('#' + i).click(data[i], TournamentDetails);
        index_key[i] = data[i].EventID;

    }


}

function errorOnAjax() {
    console.log("ERROR in ajax request.");
}





function next()
{
    if(current_page<number_of_pages)
        current_page=current_page+1;
}

function prev()
{
    if(current_page>0)
        current_page=current_page-1;
}







var interval = 100 * 9;

window.setInterval(ajax_call_events, interval);