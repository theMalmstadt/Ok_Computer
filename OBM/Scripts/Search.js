var search = $("#searchterm").val();
$("#searchterm").html("");
var myTable;
$(document).ready(function () {
    console.log("hihihihihi");

   
    if($("#table").val()=="Events")
        drawEvents();
    $("#table").change(redraw);


});

function redraw()
{
    $("#searchResults").empty();
    myTable.fnDestroy();
    if ($("#table").val() == "Events") drawEvents();

    else  drawTournaments();
}

function drawTournaments() {
    var result = $.ajax({
        type: 'GET',
        dataType: "json",
        contentType: "application/json",
        url: '/Tournaments/PublicTournaments',
        error: errorOnAjax,
        success: function (response) {
            drawTableTournaments(response);
        }
    });

}







function drawEvents()
{   
    var result = $.ajax({
        type: 'GET',
        dataType: "json",
        contentType:"application/json",
        url: '/Events/PublicEvents',
        error: errorOnAjax,
        success: function (response) {
            drawTableEvents(response);
        }
    });
    
}
function drawTableEvents(data) {
    console.log(data);
   
myTable=$('#searchResults').dataTable({
   "data": data,
   "columns": [
       { "data": "EventName" },
       { "data": "Location" },
       { "data": "UserName" },

       { 
         "data": "url",
         "render": function(data, type, row, meta){
            if(type === 'display'){
                data = '<a href="' + data + '">' + "Details" + '</a>';
            }
            
            return data;
         }
      } 
   ]
});
    myTable.fnFilter(search);

    
}





function drawTableTournaments(data) {
    console.log(data);

    myTable = $('#searchResults').dataTable({
        "data": data,
        "columns": [
            { "data": "TournamentName" },

            { "data": "Game" },
{
                "data": "url",
                "render": function (data, type, row, meta) {
                    if (type === 'display') {
                        data = '<a href="' + data + '">' + "Details" + '</a>';
                    }

                    return data;
                }
            }
        ]
    });
    myTable.fnFilter(search);


}






function errorOnAjax()
{
    console.log("error on search load ajax");
}